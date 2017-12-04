using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exit : MonoBehaviour {

	public RandomDungeonGenerator rng;

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player") {
			if (other.GetComponent<Player> ().hasKey) {
				other.GetComponent<Player> ().hasKey = false;
				other.GetComponentInChildren<Wisp> ().clear ();
				rng.regenerate ();	
			}
		}
	}
}
