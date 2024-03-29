﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Priority_Queue;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct Line {
	public Vector2 start;
	public Vector2 end;
	public int vh; //0 is h, 1 is v
	public Line(Vector2 s, Vector2 e, int bleh) {
		start = s;
		end = e;
		vh = bleh;
	}
	public override string ToString()
	{
		return "Start: " + start + " End: " + end;
	}
}

public class RandomDungeonGenerator : MonoBehaviour {
	public Vector2Int boundOrigin;
	public int minBoundWidth;
	public int maxBoundWidth;
	public int minRoomWidth;
	public int maxRoomWidth;
	public int minRoomCount;
	public int maxRoomCount;
	[Range(0,1)]
	public float obstacleFrequency;
	public Tilemap walkableMap;
	public Tilemap blockedMap;
	public Tile[] t;
	public Tile[] obstacles;
	public Tile wall;

	[Range(1,5)]
	public int minHallWidth;
	[Range(1,5)]
	public int maxHallWidth;
	[Range(0,1)]
	public float CONNECTEDNESS; //int from 0 to 1

	public Canvas floors;
	int floor;
	bool isShowingFloor;

	int boundWidth;
	int roomCount;

	public Vector3Int getBounds(){
		return new Vector3Int (boundOrigin.x, boundOrigin.y, boundWidth);
	}

	Rect[] rects;
	float[,] fullAdjacencies;
	HashSet<Rect>[] finalAdjacencies; //Array of HashSets holding Rect objects
	List<Rect> lines;

	void generateNewDungeon(){
		ShowFloorText ();
		ChooseParams ();
		GenerateRects ();
		GenerateHalls ();
		FillTiles ();
		FillWalls ();
		Refresh ();

		GetComponent<GoalGenerator> ().generate ();
		GetComponent<EnemyGenerator> ().generate ();
	
	}

	void generateBossDungeon() {
		boundWidth = 50;
		minRoomCount = 1;
		maxRoomCount = 1;
		minRoomWidth = 30;
		maxRoomWidth = 30;

		ShowFloorText ();
		ChooseParams ();
		GenerateRects ();
		GenerateHalls ();
		FillTiles ();
		FillWalls ();
		Refresh ();

		GetComponent<GoalGenerator> ().generatePlayer ();
	}

		
	void Start () {
		floor = 1;
		Clear ();
		generateNewDungeon ();
	}

	void incrementDifficulty(float mult, ref int x, bool debug = false){
		if (debug) {
			Debug.Log (x + ", " + mult + ", " + floor);
		}
		x = Mathf.FloorToInt (x + (mult * floor));
	}

	public void nextFloor(){
		Clear ();
		floor++;
		incrementDifficulty (1.2f, ref minRoomCount);
		incrementDifficulty (1.2f, ref maxRoomCount);
		incrementDifficulty (1.2f, ref minBoundWidth);
		incrementDifficulty (1.2f, ref maxBoundWidth);




		for (int i = 0; i < GetComponent<EnemyGenerator> ().amount.Length; i++) {
			
			incrementDifficulty(GetComponent<EnemyGenerator>().multipliers[i], ref GetComponent<EnemyGenerator>().amount[i], i == 8);
		}

		GetComponent<GoalGenerator> ().minDist += 1.2f * floor;
		GetComponent<GoalGenerator> ().maxDist += 1.2f * floor;

//		if (floor == 4)
//			generateBossDungeon ();
//		else
			generateNewDungeon ();

		GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ().nextLevel ();
	}

	public void regenerate(){
		Clear ();
		generateNewDungeon ();
	}

	void Clear(){
		GameObject[] enemies = GameObject.FindGameObjectsWithTag ("Enemy");
		GameObject[] goals = GameObject.FindGameObjectsWithTag ("Exit");
		GameObject[] items = GameObject.FindGameObjectsWithTag ("Item");
		for (int i = enemies.Length - 1; i >= 0; i--) {
			Destroy (enemies [i]);
		}
		for (int i = goals.Length - 1; i >= 0; i--) {
			Destroy (goals [i]);
		}
		for (int i = items.Length - 1; i >= 0; i--) {
			Destroy (items [i]);
		}
		walkableMap.ClearAllTiles();
		blockedMap.ClearAllTiles ();

		Time.timeScale = 1;

	}

	void Refresh(){
		blockedMap.RefreshAllTiles ();
		blockedMap.GetComponent<TilemapCollider2D> ().enabled = false;
		blockedMap.GetComponent<TilemapCollider2D> ().enabled = true;
		blockedMap.GetComponent<CompositeCollider2D> ().enabled = false;
		blockedMap.GetComponent<CompositeCollider2D> ().enabled = true;
		blockedMap.gameObject.SetActive (false);
		blockedMap.gameObject.SetActive (true);
	}

	void ChooseParams() {
		boundWidth = Random.Range (minBoundWidth, maxBoundWidth);
		roomCount = Random.Range (minRoomCount, maxRoomCount);
	}

	void ShowFloorText() {
		floors.GetComponentInChildren<Text>().text = "Floor: " + floor.ToString();
		floors.gameObject.SetActive (true);
		isShowingFloor = true;
	}

	void GenerateRects(){
		rects = new Rect[roomCount];
		for (int i = 0; i < roomCount; i++) {
			Rect newRect;
			float width = Random.Range (minRoomWidth, maxRoomWidth);
			float posx = Random.Range (boundOrigin.x, boundOrigin.x + boundWidth-width);
			float posy = Random.Range (boundOrigin.y, boundOrigin.y + boundWidth-width);
			newRect = new Rect(posx, posy, width, width);
			rects [i] = newRect;
		}
	}

	void GenerateHalls() {
		//Generate full graph
		int numVertices = rects.Length;
		fullAdjacencies = new float[numVertices, numVertices];
		for (int i = 0; i < numVertices; i++) {
			for (int j = i + 1; j < numVertices; j++) {
				float dist = Vector2.Distance (rects [i].center, rects [j].center);
				fullAdjacencies [i,j] = fullAdjacencies[j,i] = dist;
			}
		}
		//Minimum Spanning Tree
		Prims(rects);

		//Add back a few edges
		int numEdges = (numVertices * numVertices - numVertices) / 2;
		int numEdgesAddBack = (int)(numEdges * CONNECTEDNESS);
		//Debug.Log (numEdgesAddBack);

		for (int edge = 0; edge < numEdgesAddBack; edge++) {
			int node = Random.Range (0, numVertices);
			int closestNode = Random.Range(0, numVertices);
			finalAdjacencies [node].Add(rects[closestNode]);
		}

		//Add Hallways
		lines = new List<Rect>();
		int index = 0;
		foreach (HashSet<Rect> h in finalAdjacencies) {
			foreach (Rect r in h) {
				Rect room_i = rects[index];
				Rect room_j = r;
				if (!room_i.Overlaps(room_j)) {
					Vector2 joint = new Vector2 (room_i.center.x, room_j.center.y);
					Rect r1;
					Rect r2;
					float hallWidth = Random.Range (minHallWidth, maxHallWidth);
					float height_r1 = Mathf.Abs (joint.y - room_i.center.y);
					float width_r2 = Mathf.Abs (joint.x - room_j.center.x);
					if (room_j.center.y > room_i.center.y) {
						if (room_j.center.x > room_i.center.x) {
							r1 = new Rect (room_i.center.x, room_i.center.y, hallWidth, height_r1);
							r2 = new Rect (joint.x, joint.y, width_r2+hallWidth, hallWidth);
						} else {
							r1 = new Rect (room_i.center.x, room_i.center.y, hallWidth, height_r1);
							r2 = new Rect (room_j.center.x, room_j.center.y, width_r2+hallWidth, hallWidth);
						}
					}
					else {
						if (room_j.center.x > room_i.center.x) {
							r1 = new Rect (joint.x, joint.y, hallWidth, height_r1);
							r2 = new Rect (joint.x, joint.y, width_r2, hallWidth);
						} else {
							r1 = new Rect (joint.x, joint.y, hallWidth, height_r1);
							r2 = new Rect (room_j.center.x, room_j.center.y, width_r2, hallWidth);
						}
					}
					lines.Add (r1);
					lines.Add (r2);
				}
			}
			index++;
		}
	}

	void FillTiles() {
		foreach (Rect r in rects) {
			float startx = r.x;
			while (startx < r.x + r.width) {
				float starty = r.y;
				while (starty < r.y + r.height) {
					int rand_tindex = Random.Range(0,t.Length);
					int rand_obstaclesindex = Random.Range(0,obstacles.Length);
					float putObstacle = Random.Range(0f,1f);
					walkableMap.SetTile (new Vector3Int((int)startx,(int)starty,0), t[rand_tindex]);
					//below is to make sure you don't block the halls
					if (putObstacle < obstacleFrequency &&
						startx > r.x && starty > r.y &&
						startx < r.x + r.width - 1 && starty < r.y + r.height - 1)
						blockedMap.SetTile (new Vector3Int((int)startx,(int)starty,0), obstacles[rand_obstaclesindex]);
					starty++;
				}
				startx++;
			}
		}

		foreach (Rect l in lines) {
			float startx = l.x;
			while (startx < l.x + l.width) {
				float starty = l.y;
				while (starty < l.y + l.height) {
					int rand_tindex = Random.Range(0,t.Length);
					walkableMap.SetTile (new Vector3Int((int)startx,(int)starty,0), t[rand_tindex]);
					starty++;
				}
				startx++;
			}
		}
	}
	void FillWalls() {
		for (int x = boundOrigin.x; x < boundOrigin.x + boundWidth; x++) {
			for (int y = boundOrigin.y; y < boundOrigin.y + boundWidth; y++) {
				TileBase tileAtPos = walkableMap.GetTile (new Vector3Int (x, y, 0));
				if (tileAtPos != null && t.Contains(tileAtPos)) {
					//Debug.Log ("hi");
					if (walkableMap.GetTile (new Vector3Int(x - 1, y - 1, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x - 1, y - 1, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x, y - 1, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x, y - 1, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x + 1, y - 1, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x + 1, y - 1, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x - 1, y, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x - 1, y, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x + 1, y, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x + 1, y, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x - 1, y + 1, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x - 1, y - 1, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x, y + 1, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x, y + 1, 0), wall);
					if (walkableMap.GetTile (new Vector3Int(x + 1, y + 1, 0)) == null)
						blockedMap.SetTile (new Vector3Int (x + 1, y + 1, 0), wall);
				}
			}
		}
	}
		
	void Prims(Rect[] rects){
		SimplePriorityQueue<int> pq = new SimplePriorityQueue<int> ();

		bool[] visited = new bool[rects.Length];
		for (int i = 0; i < rects.Length; i++) {
			visited [i] = false;
		}
		float[] costs = new float[rects.Length];
		for (int i = 0; i < rects.Length; i++) {
			costs [i] = float.PositiveInfinity;
		}
		int[] parents = new int[rects.Length];

		parents [0] = -1;
		costs [0] = 0;
		pq.Enqueue (0, 0);

		while (pq.Count > 0) {
			int currNode = pq.Dequeue ();
			if (!visited [currNode]){
				visited [currNode] = true;
				for (int adjNode = 0; adjNode < rects.Length; adjNode++) {
					//no null checks because full graph
					if (!visited [adjNode]) {
						if (fullAdjacencies[currNode, adjNode] < costs [adjNode]) {
							costs[adjNode] = fullAdjacencies [currNode, adjNode];
							parents [adjNode] = currNode;
							pq.Enqueue (adjNode, costs [adjNode]);
						}
					}
				}
			}
		}

		finalAdjacencies = new HashSet<Rect>[rects.Length];
		finalAdjacencies [0] = new HashSet<Rect> ();
		for (int i = 1; i < parents.Length; i++) {
			finalAdjacencies [i] = new HashSet<Rect> ();
			finalAdjacencies [i].Add (rects[parents[i]]);
		}
	}

	// Update is called once per frame
	void Update () {
		if (isShowingFloor) {
			Color c = floors.GetComponentInChildren<Text> ().color ;

			floors.GetComponentInChildren<Text>().color = new Color(c.r, c.g, c.b, c.a - (.2f * Time.deltaTime));
			if (c.a <= 0) {
				floors.gameObject.SetActive (false);

				isShowingFloor = false;

				floors.GetComponentInChildren<Text> ().color = new Color (c.r, c.g, c.b, 1);
			}
		}
	}

	public bool walkAble(Vector3Int spot){
		return blockedMap.GetTile(spot) == null && walkableMap.GetTile(spot) != null;
	}

	public bool openSpace(Vector3Int spot){
		if (!(walkAble(spot))) {
			return false;
		}

		Vector3 irl = walkableMap.CellToLocal (spot);
			
		Vector3Int up_left = spot + new Vector3Int (-1, 1, 0);
		Vector3Int left = spot + new Vector3Int (-1, 0, 0);
		Vector3Int down_left = spot + new Vector3Int (-1, -1, 0);
		Vector3Int down = spot + new Vector3Int (0, -1, 0);
		Vector3Int down_right = spot + new Vector3Int (1, -1, 0);
		Vector3Int right = spot + new Vector3Int (1, 0, 0);
		Vector3Int up_right = spot + new Vector3Int (1, 1, 0);
		Vector3Int up = spot + new Vector3Int (0, 1, 0);

		Vector3 top_left = walkableMap.CellToWorld (up_left) * 5;
		Vector3 bottom_right = walkableMap.CellToWorld (down_right) * 5;

		//Debug.DrawLine (top_left, bottom_right);


		Collider2D[] mobs = Physics2D.OverlapAreaAll (new Vector2 (top_left.x, top_left.y), new Vector2 (bottom_right.x, bottom_right.y));
		bool mobbed = (mobs.Length > 0);

//		Debug.Log (mobs.Length);

		return (walkAble(spot) && !mobbed && walkAble (up_left) && walkAble (left)  && walkAble (down_left)  && walkAble (down) 
			&& walkAble (down_right) && walkAble (right)  && walkAble(up_right)  && walkAble (up) );

	}



}
