using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {

	private Rigidbody2D body;
	public float speed, light_modifier;
	public Vector2 direction;

	private Animator anim;

	private bool colliding;

	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D> ();
		light_modifier = 1;
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame

	void Update(){
		if (anim != null && anim.GetBool ("spawning")) {
			AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
			if (animationState.normalizedTime >= 1) {
				anim.SetBool ("spawning", false);
			}
		} else if(anim!=null){
			bool move = body.velocity != Vector2.zero;



			//			Debug.Log (body.velocity);
			bool left = direction.x < -0.1f;
			bool down = direction.y < -0.1f;
			bool up = direction.y > 0.1f;
			bool right = direction.x > 0.1f;

//		if (left) {
//			Debug.DrawLine(transform.position, new Vector3(transform.position.x - 50, transform.position.y, transform.position.z));
//		}
//		if (right) {
//			Debug.DrawLine(transform.position, new Vector3(transform.position.x + 50, transform.position.y, transform.position.z));
//		}
//
//		if (up) {
//			Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y + 50, transform.position.z));
//		}
//
//		if (down) {
//
//			Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 50, transform.position.z));
//
//		}

			//		Debug.Log (face_dir);

			if (down) {
				setDirectionAnimation (0, move);
				if (left) {
					transform.localRotation = Quaternion.Euler (0, 0, -30);
					transform.localScale = new Vector3 (-1, 1, 1);
				} else if (right) {
					transform.localRotation = Quaternion.Euler (0, 0, 30);
					transform.localScale = new Vector3 (1, 1, 1);
				} else {
					transform.localRotation = Quaternion.identity;
					transform.localScale = new Vector3 (1, 1, 1);
				}

			} else if (up) {
				setDirectionAnimation (1, move);
				if (left) {
					transform.localRotation = Quaternion.Euler (0, 0, 30);
					transform.localScale = new Vector3 (-1, 1, 1);
				} else if (right) {
					transform.localRotation = Quaternion.Euler (0, 0, -30);
					transform.localScale = new Vector3 (1, 1, 1);
				} else {
					transform.localRotation = Quaternion.identity;
					transform.localScale = new Vector3 (1, 1, 1);
				}
			} else {
				setDirectionAnimation (2, move);
		

				if (left) {
					transform.localScale = new Vector3 (-1, 1, 1);
				} else if (right) {
					transform.localScale = new Vector3 (1, 1, 1);
				} 
				transform.localRotation = Quaternion.identity;

			}

		}


		
	}


	void LateUpdate () {
		if (anim != null && anim.GetBool ("spawning")) {
			AnimatorStateInfo animationState = anim.GetCurrentAnimatorStateInfo(0);
			if (animationState.normalizedTime >= 1) {
				anim.SetBool ("spawning", false);
			}
		} else {
			colliding = false;

			Vector2 face_dir = direction;

			if (direction != Vector2.zero) {
				Collider2D c = GetComponent<Collider2D> ();

				RaycastHit2D[] hits = Physics2D.RaycastAll (new Vector2 (transform.position.x, transform.position.y), direction, c.bounds.extents.magnitude);

				for (int i = 0; i < hits.Length; i++) {
					GameObject other = hits [i].collider.gameObject;
					if (other != this.gameObject && other.tag != "Light") {
						direction = Vector2.zero;
						if (other.tag == "Player" && GetComponent<EnemyDamage> () != null) {
							Vector2 normal = new Vector2 (other.transform.position.x - transform.position.x, other.transform.position.y - transform.position.y);
							normal = normal.normalized;
							GetComponent<EnemyDamage> ().applyKnock (other, normal);
						}
					}
				}
			}

			body.velocity = new Vector2 (speed * Mathf.Min (light_modifier, 1) * direction.x, speed * Mathf.Min (1, light_modifier) * direction.y);


		}

	}

	void setDirectionAnimation(int i, bool moving){
		anim.SetInteger ("direction", i);
		anim.SetBool ("moving", moving);
	}

	Vector2 vel(){
		return body.velocity;
	}

	public void setVel(Vector2 new_v){
		body.velocity = new_v;
	}
}
