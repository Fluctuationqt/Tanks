using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileAnimation : MonoBehaviour {
    private Animation anim;
    private AnimationClip clip;
    private LineRenderer ShotLine;
    private LineRenderer TrajectoryLine;
    private bool isAnimating;
    private GameObject joystickCanvas;

    private void Start()
    {
        // Play animation clip on this projectile when it spawns
        anim = GetComponent<Animation>();
        anim.AddClip(clip, "projectileAnim");
        anim.Play("projectileAnim");
        isAnimating = true;
        StartCoroutine(ShowCamDelayed(0.1f));
        joystickCanvas = GameObject.Find("Joystick canvas").gameObject;
        joystickCanvas.SetActive(false);
    }

    IEnumerator ShowCamDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.transform.Find("ProjectileCam").gameObject.SetActive(true);
    }
    
    // Destroys the Projectile game object and reactivates the fire button
    public void OnAnimationEnded(int i)
    {
        TrajectoryLine.enabled = true;
        ShotLine.enabled = false;
        joystickCanvas.SetActive(true);
        Destroy(this.gameObject);
    }

    // This method creates an animation clip with given trajectory for the projectile
    // it is called by the Trajectory Drawer on shoot request.
    public void CreateAnimation(List<Vector3> shotTrajectory, LineRenderer lineRenderer, float maxTime)
    {
        // set private variables used in Start()
        TrajectoryLine = lineRenderer;
        ShotLine = this.GetComponent<LineRenderer>();
        
        // Show Shot LineRenderer;
        ShotLine.positionCount = shotTrajectory.Count;
        ShotLine.SetPositions(shotTrajectory.ToArray());
        TrajectoryLine.enabled = false;
        ShotLine.enabled = true;

        // Create Keyframes for curves
        Keyframe[] tX = new Keyframe[shotTrajectory.Count]; // x translation keyframes
        Keyframe[] tY = new Keyframe[shotTrajectory.Count]; // y translation keyframes
        Keyframe[] tZ = new Keyframe[shotTrajectory.Count]; // z translation keyframes
        Keyframe[] rX = new Keyframe[shotTrajectory.Count]; // x rotation keyframes
        Keyframe[] rY = new Keyframe[shotTrajectory.Count]; // y rotation keyframes
        Keyframe[] rZ = new Keyframe[shotTrajectory.Count]; // z rotation keyframes
        Keyframe[] rW = new Keyframe[shotTrajectory.Count]; // w rotation keyframes

        float t = 0; int index = 0; // key time and key index
        float timeRes = maxTime / shotTrajectory.Count; // time between keys
        Quaternion keyrotation = new Quaternion();
        Vector3 transformUp = this.transform.up;
        foreach (Vector3 point in shotTrajectory)
        {
            // Set translation keyframe values
            tX[index] = new Keyframe(t, shotTrajectory[index].x, 0, 0);
            tY[index] = new Keyframe(t, shotTrajectory[index].y, 0, 0);
            tZ[index] = new Keyframe(t, shotTrajectory[index].z, 0, 0);

            // Calculate rotation keyframe values
            
            if (index == 0)
            {
                // for first keyframe
                keyrotation = this.transform.rotation;
            }else if(index == (shotTrajectory.Count - 1))
            {
                // for last keyframe
                // do not modify keyrotation
            }
            else 
            {
                // calculate for rest of the keyframes 
                Vector3 direction = shotTrajectory[index] - shotTrajectory[index - 1];
                Debug.DrawRay(shotTrajectory[index - 1], direction.normalized, Color.green, 10);
                keyrotation = Quaternion.LookRotation(direction.normalized, transformUp);
            }

            // Set rotation keyframes
            rX[index] = new Keyframe(t, keyrotation.x, 0, 0);
            rY[index] = new Keyframe(t, keyrotation.y, 0, 0);
            rZ[index] = new Keyframe(t, keyrotation.z, 0, 0);
            rW[index] = new Keyframe(t, keyrotation.w, 0, 0);

            // increment time and keyframe index
            t += timeRes;
            index++;
        }

        // Create translation and rotation curves
        AnimationCurve txCurve = new AnimationCurve(tX);
        AnimationCurve tyCurve = new AnimationCurve(tY);
        AnimationCurve tzCurve = new AnimationCurve(tZ);
        AnimationCurve rxCurve = new AnimationCurve(rX);
        AnimationCurve ryCurve = new AnimationCurve(rY);
        AnimationCurve rzCurve = new AnimationCurve(rZ);
        AnimationCurve rwCurve = new AnimationCurve(rW);

        // Create clip
        clip = new AnimationClip();
        clip.legacy = true;
        clip.SetCurve("", typeof(Transform), "localPosition.x", txCurve);
        clip.SetCurve("", typeof(Transform), "localPosition.y", tyCurve);
        clip.SetCurve("", typeof(Transform), "localPosition.z", tzCurve);
        clip.SetCurve("", typeof(Transform), "localRotation.x", rxCurve);
        clip.SetCurve("", typeof(Transform), "localRotation.y", ryCurve);
        clip.SetCurve("", typeof(Transform), "localRotation.z", rzCurve);
        clip.SetCurve("", typeof(Transform), "localRotation.w", rwCurve);

        // Animation end event
        AnimationEvent endEvt = new AnimationEvent();
        endEvt.intParameter = 1;
        endEvt.time = maxTime;
        endEvt.functionName = "OnAnimationEnded";
        clip.AddEvent(endEvt);
    }
}
