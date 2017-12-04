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

		bool randomizedPlayer = false;
		bool randomizedGoal = false;
		bool randomizedKey = false;

		while (!randomizedPlayer) {
			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);

			TileBase walkable_tile = walkable.GetTile(randomized);
			TileBase blocked_tile = blocked.GetTile (randomized);

			if (walkable_tile != null && blocked_tile == null) {
				Vector3Int up_left = randomized + new Vector3Int (-1, 1, 0);
				Vector3Int left = randomized + new Vector3Int (-1, 0, 0);
				Vector3Int down_left = randomized + new Vector3Int (-1, -1, 0);
				Vector3Int down = randomized + new Vector3Int (0, -1, 0);
				Vector3Int down_right = randomized + new Vector3Int (1, -1, 0);
				Vector3Int right = randomized + new Vector3Int (1, 0, 0);
				Vector3Int up_right = randomized + new Vector3Int (1, 1, 0);
				Vector3Int up = randomized + new Vector3Int (0, 1, 0);
				if (blocked.GetTile (up_left) == null && blocked.GetTile (left) == null && blocked.GetTile (down_left) == null && blocked.GetTile (down) == null
				    && blocked.GetTile (down_right) == null && blocked.GetTile (right) == null && blocked.GetTile (up_right) == null && blocked.GetTile (up) == null) {
					gen_player_location = new Vector2Int (randomized.x, randomized.y);
					Vector3 location = walkable.CellToWorld (randomized);

					player.transform.position = location;
					randomizedPlayer = true;
				}
			}
		}

		while (!randomizedGoal) {

			Vector3Int randomized = new Vector3Int( Random.Range(bounds_x.x, bounds_x.y) ,Random.Range (bounds_y.x, bounds_y.y), 0);
			if (withinDistance (new Vector2Int (randomized.x, randomized.y), gen_player_location)) {

				TileBase walkable_tile = walkable.GetTile (randomized);
				TileBase blocked_tile = blocked.GetTile (randomized);

				if (walkable_tile != null && blocked_tile == null) {
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


	bool withinDistance(Vector2Int start, Vector2Int target){
		float dist = Vector2Int.Distance (start, target);

		return (dist >= minDist && dist <= maxDist);
	}

}
