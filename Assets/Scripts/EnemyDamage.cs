using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour {

	public float damage, knockback, modifier;

	void OnTriggerEnter2D(Collider2D other){
		Vector2 normal = new Vector2(other.transform.position.x - transform.position.x, other.transform.position.y - transform.position.y);
		normal = normal.normalized;

		if (other.gameObject.tag == "Player") {
			applyKnock (other.gameObject, normal);
		}
	}

	public void applyKnock(GameObject other, Vector2 normal){
		other.GetComponent<Player> ().knockBack (new Vector2 (normal.x, normal.y), knockback, modifier);
		other.GetComponent<Player> ().damage (damage);
	}


	float denom(float val){
		return val == 0 ? 1 : val;
	}
}
