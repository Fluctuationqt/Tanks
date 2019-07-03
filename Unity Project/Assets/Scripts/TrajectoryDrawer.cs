using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class TrajectoryDrawer : MonoBehaviour {
    public bool active = true;
    public float initialVelocity = 50.0f;  // Starting projectile velocity
    public float timeResolution = 0.04f;   // Time step of trajectory point calculation
    public float maxTime = 2.0f;           // Max lenght of trajectory (cutout length)
    public Vector3 windVelocity = new Vector3(0, 0, 0); // Velocity vector for wind

    public GameObject ProjectilePrefab; 
    private LineRenderer lineRenderer;
    public List<Vector3> trajectoryPoints; // Holds the current calculated trajectory
    public GameObject startPoint; // Point from which the Trajectory Line starts
    public GameObject endPoint;   // Point where the trajectory line ends (it is moved by this script)
    public GameObject currentProjectile; // Holds the currently spawned projectile

    void Start() {
        lineRenderer = this.GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (active)
        {
            CalculateTrajectory();
            DrawTrajectory();
        }
    }    
        
	void CalculateTrajectory()
    {
        Vector3 velocityVector = transform.forward * initialVelocity;
        Vector3 currentPosition = transform.position;
        // TODO: CalculateDestructionArea();

        trajectoryPoints.Clear();
        int index = 0;
        for (float t = 0; t < maxTime; t += timeResolution)
        {
            trajectoryPoints.Insert(index, currentPosition);

            // Raycast between each point in the trajectory line
            RaycastHit hit;
            if (Physics.Raycast(currentPosition, velocityVector, out hit, velocityVector.magnitude * timeResolution))
            {
                if (hit.transform.name != "DestructionAreaOutside" && hit.transform.name != "DestructionAreaInside")
                {
                    // End the line at the hit point 
                    trajectoryPoints.Insert(index + 1, currentPosition);
                    this.transform.Find("endPoint").transform.position = hit.point;
                    break;
                }
            }

            // velocity and gravity calculations for dy 
            currentPosition += velocityVector * timeResolution;
            velocityVector += Physics.gravity * timeResolution;
            velocityVector += windVelocity * timeResolution;
            index++;
        }
    }

    // Set line renderer points
    void DrawTrajectory()
    {
        lineRenderer.positionCount = trajectoryPoints.Count;
        lineRenderer.SetPositions(trajectoryPoints.ToArray());
    }

    // Enable trajectory drawing
    public void Activate()
    {
        active = true;
        lineRenderer.enabled = true;
    }

    // Disable trajectory drawing
    public void Deactivate()
    {
        active = false;
        lineRenderer.enabled = false;
    }

    // Spawn a projectile and create an animation for it that follows the current trajectory
    // This method is called by the FireRoutine on the Turret Controller
    public void AnimateProjectile()
    {
        if (trajectoryPoints.Count > 4)
        {
            currentProjectile = (GameObject)Instantiate(ProjectilePrefab, startPoint.transform.position, startPoint.transform.rotation);
            currentProjectile.name = "PlayerProjectile";
            List<Vector3> shotTrajectory = new List<Vector3>(trajectoryPoints);
            currentProjectile.GetComponent<ProjectileAnimation>().CreateAnimation(shotTrajectory, lineRenderer, maxTime);
        }
    }
 }
   

