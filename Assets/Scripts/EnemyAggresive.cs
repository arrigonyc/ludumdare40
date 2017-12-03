using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAggresive : MonoBehaviour {

	public float aggressionLevel;

	public Transform target;

	public bool inRange, pursuing, counting;

	private float start_time;
	private EnemyMovement body;



	// Use this for initialization
	void Awake () {
		if (aggressionLevel > 5) {
			aggressionLevel = 5;
		}
		body = GetComponent<EnemyMovement> ();
	}

	float getDenom(float val){
		return val == 0 ? 1 : val;
	}
	

	void Update () {
		
		if (pursuing) {
			if (aggressionLevel != 5 && !inRange) {
				if (counting) {
					float elapsed = Time.time - start_time;
					if (elapsed >= aggressionLevel) {
						pursuing = false;
						counting = false;
						start_time = 0;
						target = null;
						body.direction = Vector2.zero;
						if (GetComponent<EnemyPatrol> () != null) {
							GetComponent<EnemyPatrol> ().enabled = true;
						}
						GetComponent<EnemyMovement> ().light_modifier = 1;
					}
				} else {
					counting = true;
					start_time = Time.time;
				}
			} else if (target != null) {
				Vector2 diff = new Vector2 (target.position.x - transform.position.x, target.position.y - transform.position.y);
				Vector2 normal = diff.normalized;
				body.direction = normal;
			} else {
				pursuing = false;
				counting = false;
				start_time = 0;
				target = null;
				body.direction = Vector2.zero;
				if (GetComponent<EnemyPatrol> () != null) {
					GetComponent<EnemyPatrol> ().enabled = true;
				}
				GetComponent<EnemyMovement> ().light_modifier = 1;
			}
		}

		if (!pursuing && inRange && target != null) {
			if (GetComponent<EnemyPatrol>() != null) {
				GetComponent<EnemyPatrol> ().enabled = false;
			}
			pursuing = true;
			counting = false;
			Vector2 diff = new Vector2 (target.position.x - transform.position.x, target.position.y - transform.position.y);
			Vector2 normal = diff.normalized;
			body.direction = normal;
		}


	}

}
