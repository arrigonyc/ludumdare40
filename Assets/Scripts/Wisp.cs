using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wisp : MonoBehaviour {

	public Transform target;
	public float speed = 10;
	public float radius = 5;
	private float angle;

	private List<GameObject> inRange;


	private Light light;

	void Awake(){
		inRange = new List<GameObject> ();
		light = GetComponentInChildren<Light> ();
		transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);

	}

	void Update(){

		angle += speed * Time.deltaTime;

		Vector2 offset = new Vector2 (Mathf.Sin (angle), Mathf.Cos (angle)) * radius;

		Vector2 new_pos =  new Vector2 (target.position.x + offset.x, target.position.y + offset.y);
		transform.position = new_pos;
		checkEnemies ();

	}

	public void clear(){
		inRange.Clear ();
	}

	void checkEnemies(){
		for (int i = 0; i < inRange.Count; i++) {
			inRange [i].GetComponent<EnemyAggresive>().inRange = false;
			inRange [i].GetComponent<EnemyMovement> ().light_modifier = 1;

		}

		inRange.Clear ();

		Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), light.range);

		for (int i = 0; i < colliders.Length; i++) {
	
			GameObject obj = colliders [i].gameObject;
			if (obj.tag == "Enemy") {
				EnemyAggresive enemy = obj.GetComponent<EnemyAggresive> ();
				EnemyMovement move = obj.GetComponent<EnemyMovement> ();
				if (enemy != null) {
					if (enemy.target == null || move.light_modifier < light.intensity/25) {
						obj.GetComponent<EnemyAggresive> ().inRange = true;
						obj.GetComponent<EnemyAggresive> ().target = transform.parent;
						obj.GetComponent<EnemyMovement> ().light_modifier = light.intensity / 25;
						inRange.Add (obj);
					}

				}
			}

		}


	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Blocked Tile") {
			speed = -speed;
			transform.localScale = new Vector3 (transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
			
		}

	}
}
