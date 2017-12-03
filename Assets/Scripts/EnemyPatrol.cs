using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour {

	public Vector2 home;
	public Vector2 range;
	public Vector2 max_dist;
	public Vector2 destination;
	public float check_time, start_timer;
	public float patrol_speed;

	public bool enabled;

	private EnemyMovement body;

	// Use this for initialization
	void Awake () {
		body = GetComponent<EnemyMovement> ();
	}
	
	// Update is called once per frame
	void Update () {

		if (enabled) {
			body.speed = patrol_speed;
			float elapsed = Time.time - start_timer;

			if (elapsed >= check_time) {
				randomizeDestination ();
				start_timer = Time.time;
			}

			if (Mathf.Floor(destination.x) == Mathf.Floor(transform.position.x)) {
				body.direction = new Vector2 (0, body.direction.y);
			} else if (destination.x < transform.position.x) {
				body.direction = new Vector2 (-1, body.direction.y);
			} else if (destination.x > transform.position.x) {
				body.direction = new Vector2 (1, body.direction.y);
			}
			if (Mathf.Floor(destination.y) == Mathf.Floor(transform.position.y)) {
				body.direction = new Vector2 (body.direction.x, 0);

			} else if (destination.y > transform.position.y) {
				body.direction = new Vector2 (body.direction.x, 1);
			} else if (destination.y < transform.position.y) {
				body.direction = new Vector2 (body.direction.x, -1);
			}
		}


	}


	Vector2 dir(){
		return Random.insideUnitCircle;
	}

	void randomizeDestination(){
		float travel = Mathf.Max(0, Random.Range (range.x, range.y));
		Vector2 d = dir ();

		Vector2 dist = d * travel;


		Vector2 pos = new Vector2 (transform.position.x, transform.position.y);

		destination = pos + dist;



		if (destination != pos) {
			body.direction = d;

		} else {
			body.direction = Vector2.zero;
		}
	}


}
