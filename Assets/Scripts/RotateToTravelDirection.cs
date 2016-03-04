﻿using UnityEngine;
using System.Collections;

public class RotateToTravelDirection : MonoBehaviour {

	public PlayerMovement pm;

	void FixedUpdate () {
		transform.LookAt (new Vector3(pm.GetExitPoint().x,transform.position.y,pm.GetExitPoint().z));
	}
}
