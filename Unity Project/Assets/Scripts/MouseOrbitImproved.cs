using UnityEngine;
using System.Collections;
using UnityEngine.PostProcessing;
using UnityEngine.UI;
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class MouseOrbitImproved : MonoBehaviour
{
    
    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = 8f;
    public float distanceMax = 8f;
    public PostProcessingProfile camProfile;
    private Rigidbody rigidbody;

    float x = 0.0f;
    float y = 0.0f;
    private Slider camSlider;
    private bool isOrbiting;
    public Transform TurretCamPoint;
    int onCamera = 1;
    private Camera thisCamera;
    public float turretCamFoV = 45.0f;
    public float orbitCamFoV = 75.0f;
    private Toggle camToggle;
    private Vector3 startAngles;

    public GameObject trajectory;
    public MeshRenderer gunMesh; // hidden when in scope mode
    public MeshRenderer barrelMesh; // hidden when in scope mode
    public GameObject scope;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        startAngles = angles;
        rigidbody = GetComponent<Rigidbody>();

        // Make the rigid body not change rotation
        if (rigidbody != null)
        {
            rigidbody.freezeRotation = true;
        }
        camSlider = GameObject.Find("Slider").GetComponent<Slider>();
        thisCamera = this.GetComponent<Camera>();
        camToggle = GameObject.Find("camToggle").GetComponent<Toggle>();
     }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1) || camToggle.isOn == false)
        {
            onCamera = 1;
            thisCamera.fieldOfView = orbitCamFoV;
            barrelMesh.enabled = true;
            gunMesh.enabled = true;
            scope.SetActive(false);
            thisCamera.fieldOfView = 75;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) || camToggle.isOn == true)
        {
            onCamera = 2;
            thisCamera.fieldOfView = turretCamFoV;
            barrelMesh.enabled = false;
            gunMesh.enabled = false;
            scope.SetActive(true);
            scope.transform.SetAsFirstSibling();
            thisCamera.fieldOfView = 35;
        }
    }

    void LateUpdate()
    {
         

        if (target && onCamera == 1)
        {
            if (!isOrbiting)
            {
                camSlider.value = 0.5f;
               
            }
            x = target.transform.eulerAngles.y - (camSlider.value - 0.5f) * 360;
            
            // y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            RaycastHit hit;
            Vector3 directionToCam = (transform.position - target.position).normalized;
            Vector3 camMaxDistancePoint = target.position + directionToCam * distanceMax;
            Debug.DrawLine(target.position,  camMaxDistancePoint, Color.green);
            if (Physics.Linecast(target.position, camMaxDistancePoint, out hit))
            {
                distance = hit.distance - 0.5f;
            }
            else
            {
                distance = distanceMax;
            }
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);
            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;

            transform.rotation = rotation;
            transform.position = position;
            DepthOfFieldModel.Settings dof = camProfile.depthOfField.settings;
            dof.focusDistance = distance;
            camProfile.depthOfField.settings = dof;
        }
        else
        {
            this.transform.position = TurretCamPoint.position;
            this.transform.rotation = TurretCamPoint.rotation;
           
            DepthOfFieldModel.Settings dof = camProfile.depthOfField.settings;
            dof.focusDistance = 100;
            camProfile.depthOfField.settings = dof;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }

    public void CameraSliderDown()
    {
        isOrbiting = true;
    }
    public void CameraSliderUp()
    {
        isOrbiting = false;
    }
}