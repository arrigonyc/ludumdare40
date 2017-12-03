using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	public Transform target;
	public float speed = 0.125f;

	public Vector2 offset;

	void FixedUpdate(){
		Vector2 pref_pos = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
		Vector2 smooth_pos = Vector2.Lerp (transform.position, pref_pos, speed);
		transform.position = new Vector3(smooth_pos.x, smooth_pos.y, transform.position.z);
	}



}
