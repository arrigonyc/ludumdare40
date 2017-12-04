using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public Text timer;
	public float start_time;
	private float current_time;

	void Awake(){
		resetTime ();
	}

	public void resetTime(){
		current_time = start_time;
	}
	
	// Update is called once per frame
	void Update () {
		current_time -= Time.deltaTime;
		timer.text = current_time.ToString ("f0");

		if (current_time < 10 && timer.color != Color.red) {
			timer.color = Color.red;
		}

		if (current_time <= 0) {
			current_time = 0;
			GetComponent<Player> ().gameOver ();
		}
	}
}
