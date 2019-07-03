using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class TurretController : MonoBehaviour {
    public GameObject turret;
    public GameObject gun;
    public float turretSpeed = 50f;
    public float gunSpeed = 20f;
    
    public GameObject explosion;
    private bool isFiring = false;
    public List<Button> fireButtons;
    public GameObject turretDir;

    // Update is called once per frame
    void FixedUpdate() {

        float delta = Time.fixedDeltaTime;
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetButton("Left"))
        {
            turret.transform.Rotate(0, -Time.fixedDeltaTime * turretSpeed, 0);
        }
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetButton("Right"))
        {
            turret.transform.Rotate(0, Time.fixedDeltaTime * turretSpeed, 0);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetButton("Down"))
        {
            gun.transform.Rotate(Time.fixedDeltaTime * gunSpeed, 0, 0);

        } else if (Input.GetKey(KeyCode.DownArrow) || Input.GetButton("Up"))
        {
            gun.transform.Rotate(-Time.fixedDeltaTime * gunSpeed, 0, 0);
        }
        
        // for joystick aim
        turret.transform.Rotate(0, Time.fixedDeltaTime * Input.GetAxis("Horizontal2") * gunSpeed * 2, 0);
        turretDir.transform.Rotate(0, 0, -Time.fixedDeltaTime * Input.GetAxis("Horizontal2") * gunSpeed * 2);
        gun.transform.Rotate(Time.fixedDeltaTime * -Input.GetAxis("Vertical2") * gunSpeed, 0, 0);

        gun.transform.localEulerAngles = new Vector3(
                ClampAngle(gun.transform.localEulerAngles.x, 270, 285),
                gun.transform.localEulerAngles.y,
                gun.transform.localEulerAngles.z);
       
        if (!isFiring && (Input.GetKeyDown(KeyCode.Space)))
        {
            Fire();
        }
        
    }
    
    public void Fire()
    {
        StartCoroutine(FireRoutine());
    }

    // Creates an explosion effect and animate a projectile along the current trajectory
    IEnumerator FireRoutine()
    {
        TrajectoryDrawer td = gun.transform.Find("Trajectory").GetComponent<TrajectoryDrawer>();
        if (td.currentProjectile == null) td.AnimateProjectile();
        isFiring = true;
        foreach (Button fireButton in fireButtons)
        {
            fireButton.interactable = false;
        }
        Light eLight = explosion.GetComponent<Light>();
        eLight.intensity = 30;
        explosion.SetActive(true);
        StartCoroutine(resetExplosionLight(eLight));
        StartCoroutine(resetExplosion());
        this.GetComponent<Rigidbody>().AddForceAtPosition(
            explosion.transform.position,
            explosion.transform.up * 500);
        
        yield return new WaitForSeconds(3);
        isFiring = false;
        foreach (Button fireButton in fireButtons)
        {
            fireButton.interactable = true;
            fireButton.StopAllCoroutines();
        }
    }

    IEnumerator resetExplosionLight(Light eLight)
    {
        yield return new WaitForSeconds(0.05f);
        eLight.intensity = 0;  
    }

    IEnumerator resetExplosion()
    {
        yield return new WaitForSeconds(0.5f);
        explosion.SetActive(false);
    }

    float ClampAngle(float angle, float min, float max) {
         if (angle<90 || angle>270){       // if angle in the critic region...
             if (angle>180) angle -= 360;  // convert all angles to -180..+180
             if (max>180) max -= 360;
             if (min>180) min -= 360;
         }
         angle = Mathf.Clamp(angle, min, max);
         if (angle<0) angle += 360;  // if angle negative, convert to 0..360
         return angle;
     }
}
