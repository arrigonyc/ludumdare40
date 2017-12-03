using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyLight : MonoBehaviour {

	private Light light;

	public Vector2 range_range, intensity_range;

	public float step_range, step_intensity;

	void Awake () {
		light = GetComponent<Light> ();
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

	}
}
