using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {

	public Transform target;
	public float shake_duration, shake_amount;

	private float shake_start;

	private bool shaking;

	private Vector3 original_pos;

	// Use this for initialization
	public void startShake () {
		original_pos = target.localPosition;
		shake_start = Time.time;
		shaking = true;

	}
	
	// Update is called once per frame
	void Update () {
		if (shaking) {
			float elapsed = Time.time - shake_start;
			if (elapsed >= shake_duration) {
				target.localPosition = original_pos;
				shake_start = 0;
				shaking = false;
			} else {
				Vector2 offset = Random.insideUnitCircle * shake_amount;
				target.localPosition = new Vector3 (original_pos.x + offset.x, original_pos.y + offset.y, target.localPosition.z);
			}
		}
	}
}
