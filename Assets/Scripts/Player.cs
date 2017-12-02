using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed;
	private Rigidbody2D body;
	private Light light;
	private int current_light;

	public static LightMode[] modes = new LightMode[3] {new LightMode(30, 3.5f), new LightMode(25, 4), new LightMode(20, 4.5f) };

	public class LightMode{

		public float intensity;
		public float range;

		public LightMode(float i, float r){
			intensity = i;
			range = r;
		}

	}

	public void setLight(LightMode mode){
		light.intensity = mode.intensity;
		light.range = mode.range;
	}

	// Use this for initialization
	void Awake () {
		body = GetComponent<Rigidbody2D> ();
		light = GetComponentInChildren<Light> ();
		body.gravityScale = 0;
		current_light = 0;
		setLight (modes [current_light]);
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		float l1 = Input.GetAxisRaw ("Light1");
		float l2 = Input.GetAxisRaw("Light2");
		float l3 = Input.GetAxisRaw("Light3");

		int new_light = -1;
		if (l3 > 0) {
			new_light = 2;
		} else if (l2 > 0) {
			new_light = 1;
		} else if (l1 > 0) {
			new_light = 0;
		}

		if (new_light != -1 && new_light != current_light) {
			setLight (modes[new_light]);
			current_light = new_light;
		}


//		Debug.Log (horizontal);

		bool left, right, up, down;

		if (horizontal > 0) {
			right = true;
			left = false;
		} else if (horizontal < 0) {
			left = true;
			right = false;
		} else {
			left = right = false;
		}

		if (vertical > 0) {
			up = true;
			down = false;
		} else if (vertical < 0) {
			up = false;
			down = true;
		} else {
			up = down = false;
		}

		Vector2 new_vel = Vector2.zero;

		if (left) {
			new_vel.x -= speed;
		} else if (right) {
			new_vel.x += speed;
		} 

		if (up) {
			new_vel.y += speed;
		} else if (down) {
			new_vel.y -= speed;
		}

//		Debug.Log (new_vel);

		body.velocity = new_vel;


	}
}
