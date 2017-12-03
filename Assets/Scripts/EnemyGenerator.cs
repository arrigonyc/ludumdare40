using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Tilemaps;

public class EnemyGenerator : MonoBehaviour {

	public GameObject[] prefabs;
	public int[] amount;
	public float distance;

	public Tilemap walkable, blocked;
	public RandomDungeonGenerator rng;

	private Vector2Int bounds_x, bounds_y;

	// Use this for initialization
	void Start() {
		Vector3Int rng_range = rng.getBounds ();
		bounds_x = new Vector2Int (rng_range.x, rng_range.x + rng_range.z);
		bounds_y = new Vector2Int (rng_range.y, rng_range.y + rng_range.z);

		if (prefabs.Length != amount.Length) {
			if (prefabs.Length > amount.Length) {
				int[] temp = amount;

				amount = new int[prefabs.Length];

				for (int i = 0; i < temp.Length; i++) {
					amount [i] = temp [i];
				}

				for (int i = temp.Length; i < amount.Length; i++) {
					amount [i] = temp [temp.Length - 1];
				}


			} else if (prefabs.Length < amount.Length) {
				int[] temp = amount;

				amount = new int[prefabs.Length];

				for (int i = 0; i < amount.Length; i++) {
					amount [i] = temp [i];
				}
			}
		}


		for (int i = 0; i < prefabs.Length; i++) {
			for(int j = 0; j < amount[i]; j++){
				bool spawned = false;
				Collider2D c = prefabs [i].GetComponent<Collider2D> ();
				Vector3 max = c.bounds.max;
				Vector3 extents = c.bounds.extents;

				while (!spawned) {
					Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);
					TileBase walkable_tile = walkable.GetTile(randomized);
					TileBase blocked_tile = blocked.GetTile (randomized);



					if (walkable_tile != null && blocked_tile == null) {
						Vector3Int up_left = new Vector3Int (-1, 1, 0);
						Vector3Int left = new Vector3Int (-1, 0, 0);
						Vector3Int down_left = new Vector3Int (-1, -1, 0);
						Vector3Int down = new Vector3Int (0, -1, 0);
						Vector3Int down_right = new Vector3Int (1, -1, 0);
						Vector3Int right = new Vector3Int (1, 0, 0);
						Vector3Int up_right = new Vector3Int (1, 1, 0);
						Vector3Int up = new Vector3Int (0, 1, 0);
						if (randomized + up_left == null && randomized + left == null && randomized + down_left == null && randomized + down == null && randomized + down_right == null
						   && randomized + right == null && randomized + up_right == null && randomized + up == null) {
							Vector3 location = walkable.CellToWorld (randomized);
							Collider2D[] colliders = Physics2D.OverlapAreaAll (new Vector2(location.x - extents.x, location.y + extents.y), new Vector2(max.x, max.y ));

							if (colliders.Length <= 0) {
								GameObject clone = Instantiate (prefabs [i], location, Quaternion.identity) as GameObject;
								clone.transform.localScale = transform.localScale;
								spawned = true;
							}

						}

					}
				}
			}

		}
	}
}
	


