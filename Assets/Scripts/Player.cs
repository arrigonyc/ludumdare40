using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed;
	private Rigidbody2D body;
	private Light light;
	private int current_light;

	private float lerpSpeed = 5;


	public LightMode[] modes;

	[System.Serializable]
	public struct LightMode{

		public float intensity;
		public float range;

		public LightMode(float i, float r){
			intensity = i;
			range = r;
		}

	}

	public void setLight(LightMode mode){
		light.intensity = Mathf.Lerp (light.intensity, mode.intensity, lerpSpeed * Time.deltaTime);
		light.range = Mathf.Lerp (light.range, mode.range, lerpSpeed * Time.deltaTime);
	}

	// Use this for initialization
	void Awake () {
		body = GetComponent<Rigidbody2D> ();
		light = GetComponentInChildren<Light> ();
		body.gravityScale = 0;
		light.range = 3.5f;
		light.intensity = 30;
	}
	
	// Update is called once per frame
	void Update () {
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		float lt = Input.GetAxisRaw ("Light");

		if (lt > 0) {
			light.range = Mathf.Min (light.range + .1f, 4.5f);
			light.intensity = Mathf.Min (light.intensity + 1f, 40);

		} else if (lt < 0) {
			light.range = Mathf.Max (light.range - .1f, 3);
			light.intensity = Mathf.Max (light.intensity - .1f, 30);
	

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
