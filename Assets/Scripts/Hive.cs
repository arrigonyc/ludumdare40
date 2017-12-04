using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hive : MonoBehaviour {

	public GameObject spawner;

	void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Player" && spawner != null) {
			GameObject clone = Instantiate (spawner, gameObject.transform.position, Quaternion.identity) as GameObject;
			clone.transform.localScale = gameObject.transform.localScale;
			clone.GetComponent<Spawner> ().startSpawn ();
			//setAnimation
			FindObjectOfType<AudioManager>().playSound("hive");
			Destroy(gameObject, 2);
		}
	}
}
