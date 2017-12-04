using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour {

	public Player hp;

	public Image img;

	public float time_modifier;
	public bool lerp;


	void Update () {

		float fillAmount = hp.health / hp.maxHealth;

		float tileAmount = map (fillAmount, 0, 1, 0, 150);

		img.rectTransform.sizeDelta = new Vector2 (tileAmount, img.rectTransform.sizeDelta.y);

	}

	private float map(float val, float in_min, float in_max, float out_min, float out_max){
		return (val - in_min) * (out_max - out_min) / (in_max - in_min);
	}
}
