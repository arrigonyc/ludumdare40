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

		float fillAmount = map (hp.health, hp.maxHealth, 0, 150, 0);
		img.rectTransform.sizeDelta = new Vector2 (fillAmount, img.rectTransform.sizeDelta.y);

	}

	private float map(float val, float in_min, float in_max, float out_min, float out_max){
		return (val - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
	}
}
