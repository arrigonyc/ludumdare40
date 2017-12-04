using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GoalGenerator : MonoBehaviour {

	public Tilemap blocked, walkable;
	public RandomDungeonGenerator rng;
	public GameObject keyPrefab, goalPrefab, player;
	public float minDist, maxDist;

	private Vector2Int bounds_x, bounds_y;

	private Vector2Int gen_goal_location, gen_player_location;

	// Use this for initialization
	public void generate () {
		Vector3Int rng_range = rng.getBounds ();
		bounds_x = new Vector2Int (rng_range.x, rng_range.x + rng_range.z);
		bounds_y = new Vector2Int (rng_range.y, rng_range.y + rng_range.z);

	
		bool randomizedGoal = false;
		bool randomizedKey = false;

		generatePlayer ();

		while (!randomizedGoal) {

			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);
			if (withinDistance (new Vector2Int (randomized.x, randomized.y), gen_player_location)) {


				if (rng.openSpace (randomized)) {
					gen_goal_location = new Vector2Int (randomized.x, randomized.y);
					Vector3 location = walkable.CellToWorld (randomized);

					GameObject goal = Instantiate (goalPrefab, location, Quaternion.identity) as GameObject;
					goal.transform.localScale = transform.localScale;
					goal.GetComponent<Exit> ().rng = this.rng;
					randomizedGoal = true;
				}
			}

		}

		while (!randomizedKey) {
			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);

			if(withinDistance(new Vector2Int(randomized.x, randomized.y), gen_goal_location)){
				if (rng.openSpace (randomized)) {
					Vector3 location = walkable.CellToWorld (randomized);

					GameObject key = Instantiate (keyPrefab, location, Quaternion.identity) as GameObject;
					key.transform.localScale = transform.localScale;
					randomizedKey = true;
				}
			}


		}
	}

	public void generatePlayer(){
		bool randomizedPlayer = false;
		while (!randomizedPlayer) {
			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);



			if (rng.openSpace (randomized)) {
				gen_player_location = new Vector2Int (randomized.x, randomized.y);
				Vector3 location = walkable.CellToWorld(randomized);


				player.transform.position = location;

				randomizedPlayer = true;
			}

		}
	}


	bool withinDistance(Vector2Int start, Vector2Int target){
		float dist = Vector2Int.Distance (start, target);

		return (dist >= minDist && dist <= maxDist);
	}



}
