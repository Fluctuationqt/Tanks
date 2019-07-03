using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  Disables current tank's control scripts. 
///  Used by networking to disable control of non-local user owned tanks.
/// </summary>
public class ScriptManager : MonoBehaviour {
    public bool isPlaying = true;
	void Start () {
        if (!isPlaying)
        {
            this.GetComponent<PhysicsTank>().isPlaying = false;
            this.GetComponent<TurretController>().enabled = false;
            this.GetComponentInChildren<TrajectoryDrawer>().enabled = false;
            this.transform.Find("Main Camera").gameObject.SetActive(false);
        }
	}
}
