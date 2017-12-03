using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

	public Player hp;

	public Image img;

	public float time_modifier;
	public bool lerp;


	// Use this for initialization
	void Awake () {
		img = GetComponent<Image> ();

	}
	
	// Update is called once per frame
	void Update () {

		if (lerp) {
			img.fillAmount = Mathf.Lerp (img.fillAmount, map (hp.health, hp.maxHealth, 0, 1, 0), Time.deltaTime * Mathf.Max (1, time_modifier));
		} else {
			img.fillAmount = map (hp.health, hp.maxHealth, 0, 1, 0);
		}
	}

	private float map(float val, float in_min, float in_max, float out_min, float out_max){
		return (val - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
}
