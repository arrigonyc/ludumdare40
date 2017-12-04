using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject prefab;


	public float delay;
	public float max_amount;
	public float range;

	private float spawned_amount, timer_start;

	private bool spawning;


	public void startSpawn(){
		spawning = true;
		timer_start = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if (spawning) {
			float elapsed = Time.time - timer_start;
			if (elapsed >= delay) {

				bool spawned = false;

				Vector3 location = gameObject.transform.position;

				while (!spawned) {
					Vector2 randomized = Random.insideUnitCircle * range;
				
					Vector2 temp = new Vector2 (gameObject.transform.position.x + randomized.x, gameObject.transform.position.y + randomized.y);

					Collider2D[] colliders = Physics2D.OverlapPointAll(temp);

					if (colliders.Length <= 0) {

						location = new Vector3 (temp.x, temp.y, location.z);
						spawned = true;


					}
				}




				GameObject clone = Instantiate (prefab, location, Quaternion.identity) as GameObject;
				clone.transform.localScale = transform.localScale;

				spawned_amount++;

				if (spawned_amount >= max_amount) {
					spawning = false;
					timer_start = 0;
				} else {
					timer_start = Time.time;
				}

			}
		}

	}
}
