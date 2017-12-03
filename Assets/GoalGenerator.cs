using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoalGenerator : MonoBehaviour {

	public Tilemap blocked, walkable;
	public RandomDungeonGenerator rng;
	public GameObject keyPrefab, goalPrefab;
	public float minDist, maxDist;

	private Vector2Int bounds_x, bounds_y;

	private Vector2Int gen_goal_location;

	// Use this for initialization
	void Start () {
		Vector3Int rng_range = rng.getBounds ();
		bounds_x = new Vector2Int (rng_range.x, rng_range.x + rng_range.z);
		bounds_y = new Vector2Int (rng_range.y, rng_range.y + rng_range.z);

		bool randomizedGoal = false;
		bool randomizedKey = false;

//		while (!spawned) {
//			Vector2 randomized = Random.insideUnitCircle * range;
//
//			Vector2 temp = new Vector2 (gameObject.transform.position.x + randomized.x, gameObject.transform.position.y + randomized.y);
//
//			Collider2D[] colliders = Physics2D.OverlapPointAll(temp);
//
//			if (colliders.Length <= 0) {
//
//				location = new Vector3 (temp.x, temp.y, location.z);
//				spawned = true;
//			}
//		}
//
//
//
//
//		GameObject clone = Instantiate (prefab, location, Quaternion.identity) as GameObject;
//		clone.transform.localScale = transform.localScale;

		while (!randomizedGoal) {


//			float pos_x = Random.Range(bounds_x.x, bounds_x.y);
//
//			float pos_y = Random.Range (bounds_y.x, bounds_y.y);

			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);


			TileBase walkable_tile = walkable.GetTile(randomized);
			TileBase blocked_tile = blocked.GetTile (randomized);

			if (walkable_tile != null && blocked_tile == null) {
				gen_goal_location = new Vector2Int(randomized.x, randomized.y);
				Vector3 location = walkable.CellToWorld (randomized);

				GameObject goal = Instantiate (goalPrefab, location, Quaternion.identity) as GameObject;
				goal.transform.localScale = transform.localScale;
				randomizedGoal = true;
			}

//			

		}

		while (!randomizedKey) {
			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);

			if(withinDistance(new Vector2Int(randomized.x, randomized.y))){
				TileBase walkable_tile = walkable.GetTile(randomized);
				TileBase blocked_tile = blocked.GetTile (randomized);

				if (walkable_tile != null && blocked_tile == null) {
					Vector3 location = walkable.CellToWorld (randomized);

					GameObject key = Instantiate (keyPrefab, location, Quaternion.identity) as GameObject;
					key.transform.localScale = transform.localScale;
					randomizedKey = true;
				}
			}


		}
	}


	bool withinDistance(Vector2Int target){
		float dist = Vector2Int.Distance (target, gen_goal_location);

		return (dist >= minDist && dist <= maxDist);
	}

}
