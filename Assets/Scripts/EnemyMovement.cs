using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	private Rigidbody2D body;
	public float speed;
	public Vector2 direction;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		body.velocity = new Vector2 (speed * direction.x, speed * direction.y);

	}

	void LateUpdate () {
		if (direction != Vector2.zero) {
			Collider2D c = GetComponent<Collider2D> ();

			RaycastHit2D[] hits = Physics2D.RaycastAll (new Vector2 (transform.position.x, transform.position.y), direction, c.bounds.extents.magnitude);

			for (int i = 0; i < hits.Length; i++) {
				GameObject other = hits [i].collider.gameObject;
				if(other != this.gameObject){
					direction = Vector2.zero;
					if (other.tag == "Player" && GetComponent<EnemyDamage>() != null) {
						Vector2 normal = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.y - transform.position.y);
						normal = normal.normalized;
						GetComponent<EnemyDamage>().applyKnock(other, normal);
					}

					break;
				}
			}
		}

	}


	Vector2 vel(){
		return body.velocity;
	}

	void setVel(Vector2 new_v){
		body.velocity = new_v;
	}
}
