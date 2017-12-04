using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

	public RandomDungeonGenerator rng;

	void OnCollisionEnter2D(Collision2D other){
		if (other.collider.tag == "Player") {
			if (other.collider.GetComponent<Player> ().hasKey) {
				other.collider.GetComponent<Player> ().hasKey = false;
				other.collider.GetComponentInChildren<Wisp> ().clear ();
				rng.regenerate ();	
				other.collider.GetComponent<Timer> ().resetTime ();
			} else {
				other.collider.GetComponent<Player> ().noKey ();
			}
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			if (other.GetComponent<Player> ().hasKey) {
				other.GetComponent<Player> ().hasKey = false;
				other.GetComponentInChildren<Wisp> ().clear ();
				rng.regenerate ();	
				other.GetComponent<Timer> ().resetTime ();
			} else {
				other.GetComponent<Player> ().noKey ();
			}
		}
	}
}
