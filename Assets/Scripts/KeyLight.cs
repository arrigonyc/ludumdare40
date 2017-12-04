using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLight : MonoBehaviour {

	private Light light;
	private List<GameObject> inRange;

	public Vector2 range_range, intensity_range;

	public float step_range, step_intensity;

	void Awake () {
		light = GetComponent<Light> ();
		inRange = new List<GameObject> ();
	}

	void Update () {

		light.range += step_range;
		light.intensity += step_intensity;

		if (light.range >= range_range.y || light.range <= range_range.x) {
			step_range = -step_range;
			step_intensity = -step_intensity;
		}else if (light.intensity >= intensity_range.y || light.intensity <= intensity_range.x) {
			step_range = -step_range;
			step_intensity = -step_intensity;
		}

		checkEnemies ();
	}

	void checkEnemies(){
		for (int i = 0; i < inRange.Count; i++) {
			inRange [i].GetComponent<EnemyAggresive>().inRange = false;
			inRange [i].GetComponent<EnemyMovement> ().light_modifier = 1;
		}

		inRange.Clear ();

		Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), light.range/2);

		for (int i = 0; i < colliders.Length; i++) {

			GameObject obj = colliders [i].gameObject;
			if (obj.tag == "Enemy") {
				EnemyAggresive enemy = obj.GetComponent<EnemyAggresive> ();
				EnemyMovement move = obj.GetComponent<EnemyMovement> ();
				if ((enemy != null && move != null) && (enemy.target == null)) {
					obj.GetComponent<EnemyAggresive> ().inRange = true;
					obj.GetComponent<EnemyAggresive> ().target = gameObject.transform;
					obj.GetComponent<EnemyMovement> ().light_modifier = 1;
					inRange.Add (obj);
				
				}
			}

		}


	}

	public void clearEnemies(){
		for (int i = 0; i < inRange.Count; i++) {
			inRange [i].GetComponent<EnemyAggresive>().inRange = false;
			inRange [i].GetComponent<EnemyMovement> ().light_modifier = 1;
			inRange [i].GetComponent<EnemyAggresive> ().target = null;
		}

		inRange.Clear ();
	}
}
