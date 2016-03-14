﻿using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

	public bool playerActive = true;
	PlayerPower playerPower;
	GameObject player;

	[Header("Speed variables")]
	public float minSpeed = 0.1f;
	public float maxSpeed = 2f;
	public float slowDown = 0.02f;

	private float speed;

	[Header("Jump variables")]
	public GameObject model;
	public float jumpSwitchThreshold = 1.5f;
	public float jumpHeight = 2.5f;
	public float jumpForce = 2f; 
	public float gravity = 2.5f;

	bool grounded = true;

	[Header("GUI variables")]
	public GameObject trackIndicator;

	private Vector3 enterPoint;
	private Vector3 exitPoint;


	void Start(){
		player = GameObject.Find ("player1");
		playerPower = player.GetComponent<PlayerPower> ();

		enterPoint = gameObject.transform.position;
		exitPoint = new Vector3 (gameObject.transform.position.x + 1, gameObject.transform.position.y, gameObject.transform.position.z);
	}

	void Update(){
		//Check if exitPoint reached : extend exitPoint
		//Vector3 point = new Vector3(exitPoint.x+1f,exitPoint.y,exitPoint.z);
		if (gameObject.transform.position == exitPoint) {
			ExtendExitPoint (2f);
		}

	}

	void FixedUpdate(){
		CorrectSpeed ();
		Move ();
		CorrectPosition ();
	}

	void CorrectSpeed(){
		if (speed > minSpeed) {
			if (grounded) {
				speed -= speed * slowDown;
			} else {
				speed -= speed * slowDown * 0.3f;
			}
		}
		if (speed < minSpeed) {
			speed = minSpeed;
		}
		if (speed > maxSpeed && maxSpeed != 0f) {
			speed = maxSpeed;
		}
	}

	void Move(){
		
		float step = speed * Time.deltaTime;
		transform.position = Vector3.MoveTowards(transform.position, exitPoint, step);

	}

	void CorrectPosition(){

	}



	// SPEED //-------------------------//
	public void SetPathPoints(Vector3 newEnterPoint, Vector3 newExitPoint){
		enterPoint = newEnterPoint;

		float zCorrection = 0;
		if (newExitPoint.z == newEnterPoint.z) {
			zCorrection = 0;
		} else {
			if (newExitPoint.z < newEnterPoint.z) {
				zCorrection = -0.05f;
			}
			if (newExitPoint.z > newEnterPoint.z) {
				zCorrection = 0.05f;
			}
		}

		exitPoint = new Vector3(newExitPoint.x+0.05f,newExitPoint.y,newExitPoint.z-zCorrection);
		//Debug.Log ("Recieved new path points");
	}

	public void ExtendExitPoint(float extension){
		float zCorrection = 0;
		if (exitPoint.z == enterPoint.z) {
			zCorrection = 0;
		} else {
			if (exitPoint.z < enterPoint.z) {
				zCorrection = -(extension * 0.5f);
			}
			if (exitPoint.z > enterPoint.z) {
				zCorrection = (extension * 0.5f);
			}
		}
		enterPoint = exitPoint;
		exitPoint = new Vector3 (exitPoint.x + extension, exitPoint.y, (exitPoint.z + zCorrection));
		Debug.Log ("Extending exitPoint");
	}

	public Vector3 GetEnterPoint(){
		return enterPoint;
	}

	public Vector3 GetExitPoint(){
		return exitPoint;
	}

	public void ChangeSpeed(float speedChange){
		if (grounded) {
			speed += speedChange;
		}
	}

	public void SetSpeed(float newSpeed){
		speed = newSpeed;
	}

	public float GetSpeed(){
		return speed;
	}


	//JUMP//-------------------------//
	public bool isGrounded(){
		return grounded;
	}

	public void Jump(){
		if (grounded) {
			
			StartCoroutine (Jumping ());

		}
	}

	public void JumpSwitch(int state){
		Debug.Log ("JUMP Switch!");
	}

	IEnumerator Jumping(){

		grounded = false;


		while (model.transform.position.y < (jumpHeight - 0.15f)) {
			float step = jumpForce * Time.deltaTime * (jumpHeight - model.transform.position.y);
			model.transform.position = Vector3.MoveTowards (model.transform.position, new Vector3 (transform.position.x, transform.position.y + jumpHeight, transform.position.z), step);
			yield return null;
		}
		while (model.transform.position.y > transform.position.y) {
			float step = gravity * Time.deltaTime  * (jumpHeight - model.transform.position.y);
			model.transform.position = Vector3.MoveTowards (model.transform.position, transform.position, step);
			yield return null;
		}
			
		grounded = true;

	}


	public void Crash(string type){

		GameObject cart = model.transform.GetChild (0).gameObject;
		Rigidbody rb = cart.GetComponent<Rigidbody> ();
		playerActive = false;

		//model.transform.parent = null;
		cart.transform.parent = null;



		rb.useGravity = true;
		rb.isKinematic = false;
		if (type == "ground") {
			StartCoroutine (GroundCrash (cart));
		}
		if (type == "wall") {
			StartCoroutine (WallCrash (cart));
		}

	}

	IEnumerator GroundCrash(GameObject go){
		go.GetComponent<Rigidbody>().AddForce (Vector3.down * 300f);
		go.GetComponent<Rigidbody>().AddForce (Vector3.right * 100f);
		while (go.transform.position.y > -0.3f) {
			Debug.Log (go.transform.position.y);
			yield return null;
		
		}
		go.GetComponent<Rigidbody>().AddForce (Vector3.up * 400f);
		go.GetComponent<Rigidbody>().AddForce (Vector3.right * 100f);
		go.GetComponent<Rigidbody>().AddForce (Vector3.forward * 60f);
		Destroy (gameObject);
	}

	IEnumerator WallCrash(GameObject go){
		go.GetComponent<Rigidbody>().AddForce (Vector3.left * 200f);
		while (go.transform.position.y > -0.2f) {
			Debug.Log (go.transform.position.y);
			yield return null;

		}
		go.GetComponent<Rigidbody>().AddForce (Vector3.up * 140f);
		go.GetComponent<Rigidbody>().AddForce (Vector3.forward * 100f);
		Destroy (gameObject);
	}
}
