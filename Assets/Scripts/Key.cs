using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){


		if (other.tag == "Player") {
			other.GetComponent<Player> ().hasKey = true;
			GameObject.FindGameObjectWithTag ("Exit").GetComponent<Collider2D> ().isTrigger = true;
			GetComponent<KeyLight> ().clearEnemies ();
			Destroy (gameObject);
		}
	}


}
