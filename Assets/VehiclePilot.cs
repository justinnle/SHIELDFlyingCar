using UnityEngine;
using System.Collections;

public class VehiclePilot : MonoBehaviour {
	public Rigidbody vehicleRigidBody;
	public GameObject thisObject;
	public GameObject lowestObject;

	//movement speeds
	public float speed;
	public float groundSpeed;
	public float flySpeed;
	public float slowSpeed;

	//turning speeds
	public float turning;
	public float groundTurn;
	public float flyTurn;

	//other stuff
	public float strafeRot;
	public bool carRot;

	//states
	public bool flying;
	public bool transition;	
	public bool onGround;
	public bool slowingDown;
	public bool straightening;
	public string direction;

	//temporary states
	public float initFly;
	public Vector3 strafeVec;

	// Use this for initialization
	void Start () {
		thisObject = GameObject.Find ("Vehicle");
		groundSpeed = 5.0f;
		groundTurn = 80.0f;
		flySpeed = 10.0f;
		flyTurn = 120.0f;
		speed = 0;
		slowSpeed = 8f;
		turning = groundTurn;
		flying = false;
		transition = false;
		vehicleRigidBody = GetComponent<Rigidbody> ();
		lowestObject = GameObject.Find ("Ground");
		slowingDown = false;
		direction = "none";
		strafeRot = 0;
		strafeVec = Vector3.zero;
		carRot = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.R)){
			ResetCar();
		}
		float bias = .9f;
		Vector3 newPositionToTrackToward = transform.position - transform.forward * 6.0f + Vector3.up * 3.0f;//ten m behind and 6 m up of car
		Camera.main.transform.position = Camera.main.transform.position * bias + newPositionToTrackToward * (1.0f - bias);//move 10% 
		Camera.main.transform.LookAt (transform.position);//pivot to follow car
		print (transform.position.y + " " + slowingDown+ " " + strafeVec + " " + straightening);
		if (Input.GetKeyDown (KeyCode.Space)) {
			transition = true;
			initFly = transform.position.y + 1f;
			if(!flying){
				vehicleRigidBody.useGravity = false;
				flying = true;
				print ("RBOff");
			}else{
				vehicleRigidBody.useGravity = true;
				flying = false;
				transition = false;
				print ("RBOn");
			}
		}
		//loop transition
		if (transition) {
			if(transform.position.y<initFly){
				transform.position += Vector3.up * 1f * Time.deltaTime;
			}
			else{
				transition = false;
			}
		}
		else{
			if(flying){
				turning = flyTurn;
			} else{
				turning = groundTurn;
			}
			if(slowingDown){//loop for slow down
				if(speed>0){
					if(direction.Equals ("down")){
						slowSpeed = 9f;
					}
					else{
						slowSpeed = 5f;
					}
					speed -= slowSpeed * Time.deltaTime;
					if(direction.Equals("forward")){
						transform.position += transform.forward * speed * Time.deltaTime;
					}
					else if(direction.Equals("backward")){
						transform.position -= transform.forward * speed * Time.deltaTime;
					}
					else if(direction.Equals("right")){
						transform.position += transform.right * speed * Time.deltaTime;
					}
					else if(direction.Equals ("left")){
						transform.position -= transform.right * speed * Time.deltaTime;
					}
					else if(direction.Equals("up")){
						transform.position += Vector3.up * speed * Time.deltaTime;
					}
					else if(direction.Equals ("down")){
						transform.position -= Vector3.up * speed * Time.deltaTime;
					}
				}
				else if(speed<0){
					speed = 0;
					slowingDown = false;
				}
			}

			//loop for straighten from strafe rotation
			if(straightening){
				if(direction.Equals("right")){
					if(strafeRot>0){
						strafeRot -= 30f * Time.deltaTime;
						transform.Rotate(0,0,30f*Time.deltaTime);
					}
					else{
						//reset rotation
						strafeRot = 0;
						strafeVec = Vector3.zero;
						straightening = false;
					}
				}
				else if(direction.Equals("left")){
					if(strafeRot<0){
						strafeRot += 30f * Time.deltaTime;
						transform.Rotate(0,0,30f*-Time.deltaTime);
					}
					else{
						//reset rotation
						strafeRot = 0;
						strafeVec = Vector3.zero;
						straightening = false;
					}
				}
				else if(direction.Equals ("forward")){
				}
				else if(direction.Equals("backward")){
				}
			}

			//move forward/back
			if(Input.GetKey (KeyCode.W)){
				slowingDown = false;
				direction = "forward";
				if(flying){
					if(speed<flySpeed){//xlr8
						speed+=4f*Time.deltaTime;
					}
				}
				else{
					if(speed<groundSpeed){
						speed+=2f*Time.deltaTime;
					}
				}
				transform.position += transform.forward * speed * Time.deltaTime;
			} else if(Input.GetKeyUp (KeyCode.W)){
				slowingDown = true;
			} 
			else if(Input.GetKey(KeyCode.S)){
				slowingDown = false;
				direction = "backward";
				if(flying){
					if(speed<flySpeed*.75){
						speed+=3f *Time.deltaTime;
					}
				}
				else{
					speed+=.75f *Time.deltaTime;
				}
				transform.position -= transform.forward * speed * .75f * Time.deltaTime;
			}
			else if(Input.GetKeyUp (KeyCode.S)){
				slowingDown=true;
			}

			//turn
			if (Input.GetKey (KeyCode.A)) {
				transform.Rotate (0,-turning * Time.deltaTime,0);
			} else if(Input.GetKey (KeyCode.D)) {
				transform.Rotate (0,turning * Time.deltaTime,0);
			}
			//flying controls
			if(flying){
				if(Input.GetKey(KeyCode.LeftShift)){
					if(Input.GetKey(KeyCode.RightArrow)){
						transform.Rotate (0,0,-80f*Time.deltaTime);
					}
					else if(Input.GetKey (KeyCode.LeftArrow)){
						transform.Rotate (0,0,80f*Time.deltaTime);
					}
					else if(Input.GetKey (KeyCode.UpArrow)){
						transform.Rotate (80f*Time.deltaTime,0,0);
					}
					else if(Input.GetKey (KeyCode.DownArrow)){
						transform.Rotate(-80f*Time.deltaTime,0,0);
					}
				}
				else{
					//up/down or rotate
				if (Input.GetKey (KeyCode.UpArrow)) {
					if(speed<flySpeed){//xlr8
						speed+=6f*Time.deltaTime;
					}
					direction = "up";
					transform.position += Vector3.up * speed * Time.deltaTime;
				} else if(Input.GetKeyUp (KeyCode.UpArrow)){
					slowingDown=true;
				}else if (Input.GetKey (KeyCode.DownArrow)) {
					if(speed<flySpeed){//xlr8
						speed+=6f*Time.deltaTime;
					}
					direction = "down";
					transform.position -= Vector3.up * speed * Time.deltaTime;
				}else if(Input.GetKeyUp (KeyCode.DownArrow)){
					slowingDown = true;
				}
					//strafe or rotate
					if (Input.GetKey (KeyCode.RightArrow)) {
						if(speed<flySpeed){//xlr8
							speed+=6f*Time.deltaTime;
						}
						if(strafeVec == Vector3.zero){
							strafeVec = transform.right;
						}
						direction = "right";
						if(strafeRot<15f){
							transform.Rotate (0,0,30f*-Time.deltaTime);
							strafeRot += 30f * Time.deltaTime;
						}
						transform.position += strafeVec * speed * Time.deltaTime;
					}else if(Input.GetKeyUp (KeyCode.RightArrow)){
						slowingDown = true;
						straightening = true;
					}else if(Input.GetKey (KeyCode.LeftArrow)) {
						if(speed<flySpeed){//xlr8
							speed+=6f*Time.deltaTime;
						}
						if(strafeVec == Vector3.zero){
							strafeVec = transform.right;
						}
						if(strafeRot>-15f){
							transform.Rotate (0,0,30f*Time.deltaTime);
							strafeRot -= 30f * Time.deltaTime;
						}
						direction = "left";
						transform.position -= strafeVec * speed * Time.deltaTime;
					}else if(Input.GetKeyUp(KeyCode.LeftArrow)){
						slowingDown = true;
						straightening = true;
					}
				}
			}

		}
	}
	public void ResetCar(){
		transform.position = new Vector3 (4f, 1f, .5f);
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity=Vector3.zero;
		transform.rotation=Quaternion.identity;
	}
	
}
