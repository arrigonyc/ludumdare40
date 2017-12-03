﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public float speed;
	private Rigidbody2D body;
	private Light light;
	private int current_light;

	private List<GameObject> inRange;

	public float health, maxHealth;

	private bool flickering;
	private float flicker_start;

	public float flicker_duration, flicker_delay;

	private bool knockedBack;
	private Vector2 knock_destination;

	private SpriteRenderer sprite;

	// Use this for initialization
	void Awake () {
		sprite = GetComponent<SpriteRenderer> ();
		body = GetComponent<Rigidbody2D> ();
		light = GetComponentInChildren<Light> ();
		inRange = new List<GameObject> ();
		body.gravityScale = 0;
		light.range = 3.5f;
		light.intensity = 20;
	}
	
	// Update is called once per frame
	void Update () {
		if (!knockedBack) {
			float horizontal = Input.GetAxisRaw ("Horizontal");
			float vertical = Input.GetAxisRaw ("Vertical");
			float lt = Input.GetAxisRaw ("Light");

			if (lt > 0) {
				light.range = Mathf.Min (light.range + .1f, 4.5f);
				light.intensity = Mathf.Min (light.intensity + 1f, 30);

			} else if (lt < 0) {
				light.range = Mathf.Max (light.range - .1f, 3);
				light.intensity = Mathf.Max (light.intensity - 1f, 20);
	

			}

			bool left, right, up, down;

			if (horizontal > 0) {
				right = true;
				left = false;
			} else if (horizontal < 0) {
				left = true;
				right = false;
			} else {
				left = right = false;
			}

			if (vertical > 0) {
				up = true;
				down = false;
			} else if (vertical < 0) {
				up = false;
				down = true;
			} else {
				up = down = false;
			}

			Vector2 new_vel = Vector2.zero;

			if (left) {
				new_vel.x -= speed;
			} else if (right) {
				new_vel.x += speed;
			} 

			if (up) {
				new_vel.y += speed;
			} else if (down) {
				new_vel.y -= speed;
			}

			body.velocity = new_vel;
		} else {
			if (Mathf.Floor (transform.position.x) == Mathf.Floor (knock_destination.x)) {
				body.velocity = new Vector2 (0, body.velocity.y);
			}
			if (Mathf.Floor (transform.position.y) == Mathf.Floor (knock_destination.y)) {
				body.velocity = new Vector2 (body.velocity.x, 0);
			}

			if (body.velocity == Vector2.zero) {
				knockedBack = false;
				knock_destination = Vector2.zero;
			}
		}

		if (flickering) {
			float elapsed = Time.time - flicker_start;
			if (elapsed >= flicker_duration) {
				flickering = false;
				StopCoroutine (Flicker ());
				flicker_start = 0;
			}
		}

		checkEnemies ();
	}

	IEnumerator Flicker(){
		
		while (flickering) {
			sprite.color = Color.black;
			yield return new WaitForSeconds(flicker_delay);
			sprite.color = Color.white;
			yield return new WaitForSeconds(flicker_delay);
		}
	}

	public void damage(float dmg){
		if (!flickering) {
			health = Mathf.Max (0, health - dmg);
			flicker_start = Time.time;
			flickering = true;
			StartCoroutine (Flicker ());
		}
	}

	public void knockBack(Vector2 dir, float knock_strength, float knock_modifier){
		if (!flickering) {
			knockedBack = true;
			body.velocity = dir * speed * knock_modifier;
			knock_destination = new Vector2 (transform.position.x, transform.position.y) + (dir * (knock_strength * knock_modifier));
		}
	}

	void checkEnemies(){
		for (int i = 0; i < inRange.Count; i++) {
			inRange [i].GetComponent<EnemyAggresive>().inRange = false;
		}

		inRange.Clear ();

		Collider2D[] colliders = Physics2D.OverlapCircleAll(new Vector2(transform.position.x, transform.position.y), light.range/2);

		for (int i = 0; i < colliders.Length; i++) {
			GameObject obj = colliders [i].gameObject;
			if (obj.tag == "Enemy") {
				if (obj.GetComponent<EnemyAggresive> () != null) {
					obj.GetComponent<EnemyAggresive> ().inRange = true;
					obj.GetComponent<EnemyAggresive> ().target = gameObject;
					inRange.Add (obj);
				}
			}
			
		}

	
	}
}
