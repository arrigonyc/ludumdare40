using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			other.GetComponent<Player> ().health = other.GetComponent<Player> ().maxHealth;
			GetComponentInChildren<KeyLight> ().clearEnemies ();
			FindObjectOfType<AudioManager> ().playSound ("potion");
			Destroy (gameObject);
		}
	}
}
