using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class PhysicsTank : MonoBehaviour
{

    public bool isPlaying = true;
    [Tooltip("Top speed of the tank in m/s.")]
    public float topSpeed = 15.0f;
    [Tooltip("Top reverse speed of the tank in m/s.")]
    public float reverseTopSpeed = 5.0f;
    [Tooltip("Power of any wheel listed under powered wheels.")]
    public float motorTorque = 25.0f;
    [Tooltip("Brakes when forward input is released")]
    public float brakeTorque = 100f;
    [Tooltip("Sideways turn friction(rate) when standing in one place")]
    public float minSidewaysFriction = 0.1f;
    [Tooltip("Sideways turn friction(rate) coefficient for all wheels but the mid ones. This scales with speed(bigger = less turning at top speed, smaller = more turning while at top speed)")]
    public float maxSidewaysFriction = 0.5f;
    [Tooltip("Negative number, reverse speed at which to invert steering for example: -1")]
    public float invertSteeringReverseSpeed = -1f;
    [Tooltip("Assign this to override the center of mass. This can be useful to make the tank more stable and prevent it from flipping over. \n\nNOTE: THIS TRANSFORM MUST BE A CHILD OF THE ROOT TANK OBJECT.")]
    public Transform centerOfMass;

    [Tooltip("This prefab will be instantiated as a child of each wheel object and mimic the position/rotation of that wheel. If the prefab has a diameter of 1m, it will scale correct to match the wheel radius.")]
    public Transform wheelModelPrefab;

    [Tooltip("Front wheels used for steering by rotating the wheels left/right.")]
    public WheelCollider[] front;
    [Tooltip("Rear wheels for steering by rotating the wheels left/right.")]
    public WheelCollider[] rear;
    [Tooltip("Wheels that provide power and move the tank forwards/reverse.")]
    public WheelCollider[] poweredWheels;

    [Tooltip("Wheels on the left side of the tank that are used for differential steering.")]
    public WheelCollider[] left;
    [Tooltip("Wheels on the right side of the tank that are used for differential steering.")]
    public WheelCollider[] right;
    [Tooltip("The Wheel in the middle of left side.")]
    public WheelCollider left_mid;
    [Tooltip("The Wheel in the middle of right side.")]
    public WheelCollider right_mid;

    public GameObject[] bones_left;
    public GameObject[] bones_right;
    public float trackWidth;
    public float trackOffset;
    public float wheelSize;

    private Rigidbody rigid;
    private float forwardInput = 0.0f, turnInput = 0.0f;

    // Maps a wheel collider with a wheel prefabs.
    private Dictionary<WheelCollider, Transform> WheelToTransformMap;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        WheelToTransformMap = new Dictionary<WheelCollider, Transform>(poweredWheels.Length);
    }

    private void Start()
    {
        if (centerOfMass != null)
        {
            if (centerOfMass.parent == transform)
                rigid.centerOfMass = centerOfMass.localPosition;
            else
                Debug.LogWarning(name + ": Cannot override center of mass if it isn't a child of " + transform.name);

        }

        if (wheelModelPrefab != null)
        {
            InstantiateWheelModelsFromPrefab(front);
            InstantiateWheelModelsFromPrefab(rear);
            InstantiateWheelModelsFromPrefab(poweredWheels);
            InstantiateWheelModelsFromPrefab(left);
            InstantiateWheelModelsFromPrefab(right);
        }
    }

    private void Update()
    {
        if (isPlaying)
        {
            forwardInput = Input.GetAxis("Vertical");
            turnInput = Input.GetAxis("Horizontal");
        }
    }

    private void FixedUpdate()
    {
        Accelerate();
        DifferentialSteer();
        AttachTracks();
    }

    void AttachTracks()
    {
        Vector3 wheelPos;
        for (int i = 0; i < left.Length; i++)
        {
            wheelPos = WheelToTransformMap[left[i]].position;
            bones_left[i].transform.position
                  = wheelPos + bones_left[i].transform.TransformDirection(Vector3.forward * trackOffset);

            wheelPos = WheelToTransformMap[right[i]].position;
            bones_right[i].transform.position
                  = wheelPos + bones_right[i].transform.TransformDirection(Vector3.forward * trackOffset);
        }
    }

    private void Accelerate()
    {
        foreach (WheelCollider wheel in poweredWheels)
        {
            // Zero the torque if top speed is reached
            float localForwardVelocity = Mathf.Clamp(Vector3.Dot(rigid.velocity, rigid.transform.forward), -topSpeed, topSpeed);
            if (localForwardVelocity <= topSpeed && localForwardVelocity > -reverseTopSpeed)
                wheel.motorTorque = forwardInput * motorTorque;
            else
                wheel.motorTorque = 0.0f;

            // Brake on idle input
            if (forwardInput == 0)
                wheel.brakeTorque = 10.0f;
            else
                wheel.brakeTorque = 0.0f;

            // Update wheel meshes
            if (wheelModelPrefab != null && WheelToTransformMap.ContainsKey(wheel))
            {
                Vector3 position;
                Quaternion rotation;
                wheel.GetWorldPose(out position, out rotation);
                WheelToTransformMap[wheel].position = position;
                WheelToTransformMap[wheel].rotation = rotation;
            }
        }
    }

    private void DifferentialSteer()
    {
        // Invert steering when changing direction (forward/reverse)
        float localForwardVelocity = Mathf.Clamp(Vector3.Dot(rigid.velocity, rigid.transform.forward),-topSpeed, topSpeed);
        if (localForwardVelocity < invertSteeringReverseSpeed)
            turnInput =  -turnInput;

        // Add differential torque based on turn input
        foreach (WheelCollider wheel in left)
            wheel.motorTorque += motorTorque * turnInput;
        foreach (WheelCollider wheel in right)
            wheel.motorTorque -= motorTorque * turnInput;

        // Scale the WheelCollider stiffness linearly based on the value of the normalized local forward velocity
        float normLocalFwdVelocity = scale(0, topSpeed, 0, 1, Mathf.Abs(localForwardVelocity));
        foreach (WheelCollider wheel in left)
        {
            if(wheel != left_mid)
            {   
                WheelFrictionCurve sfc = wheel.sidewaysFriction;
                sfc.stiffness = minSidewaysFriction + normLocalFwdVelocity * maxSidewaysFriction; // for linear stiffness
                wheel.sidewaysFriction = sfc;
            }
        }

        foreach (WheelCollider wheel in right)
        {
            if (wheel != right_mid)
            {
                WheelFrictionCurve sfc = wheel.sidewaysFriction;
                sfc.stiffness = minSidewaysFriction + normLocalFwdVelocity * maxSidewaysFriction; // for linear stiffness
                wheel.sidewaysFriction = sfc;
            }
        }
    }

    private void InstantiateWheelModelsFromPrefab(WheelCollider[] wheels)
    {
        foreach (WheelCollider wheel in wheels)
        {
            if (WheelToTransformMap.ContainsKey(wheel) == false)
            {
                Transform temp = Instantiate(wheelModelPrefab, wheel.transform, false);
                // Rescale Wheels
                temp.localScale = new Vector3(trackWidth, wheel.radius * wheelSize, wheel.radius * wheelSize);
                WheelToTransformMap.Add(wheel, temp);
            }
        }
    }

    // Rescale a value from range to range
    public float scale(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);
        float NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        return (NewValue);
    }
}

/*
  Note: If you are not using the built in physics, but raycasting for track bone movement and 
        direct transform position changes you can use this method for simple steering

    public float turnRate = 45.0f;
    private void SteerSimple()
    {
        Quaternion rot = transform.rotation * Quaternion.AngleAxis(turnRate * turnInput * Time.deltaTime, transform.up);
        rigid.MoveRotation(rot);
    }
*/