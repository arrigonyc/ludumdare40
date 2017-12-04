using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class EnemyGenerator : MonoBehaviour {

	public GameObject[] prefabs;
	public int[] amount;
	public float[] multipliers;
	public int max_attempts;
	public float distance;

	public Tilemap walkable, blocked;
	public RandomDungeonGenerator rng;

	private Vector2Int bounds_x, bounds_y;

	// Use this for initialization
	public void generate() {
		Vector3Int rng_range = rng.getBounds ();
		//Debug.Log (rng_range);
		bounds_x = new Vector2Int (rng_range.x, rng_range.x + rng_range.z);
		bounds_y = new Vector2Int (rng_range.y, rng_range.y + rng_range.z);
//
//		if (prefabs.Length != amount.Length) {
//			if (prefabs.Length > amount.Length) {
//				int[] temp = amount;
//
//				amount = new int[prefabs.Length];
//
//				for (int i = 0; i < temp.Length; i++) {
//					amount [i] = temp [i];
//				}
//
//				for (int i = temp.Length; i < amount.Length; i++) {
//					amount [i] = temp [temp.Length - 1];
//				}
//
//
//			} else if (prefabs.Length < amount.Length) {
//				int[] temp = amount;
//
//				amount = new int[prefabs.Length];
//
//				for (int i = 0; i < amount.Length; i++) {
//					amount [i] = temp [i];
//				}
//			}
//		}
//

		for (int i = 0; i < prefabs.Length; i++) {
			

			for(int j = 0; j < amount[i]; j++){
				bool spawned = false;


				int attempts = 0;

				while (!spawned) {
					Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);

			
					if (rng.openSpace (randomized)) {
						Vector3 location = walkable.CellToWorld (randomized);

						GameObject clone = Instantiate (prefabs [i], location, Quaternion.identity) as GameObject;
						clone.transform.localScale = transform.localScale;
						spawned = true;
					}else {
						attempts++;
						if (attempts >= max_attempts) {
							spawned = true;
							Debug.Log (prefabs[i] + " failure, " + attempts);
					
						}

					}
				}
			}

		}
	}
}
	


