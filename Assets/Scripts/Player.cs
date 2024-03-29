﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	public Canvas pause, message, game_over;

	public float speed;
	private Rigidbody2D body;
	private Light light;
	private int current_light;

	public float light_range_max, light_intensity_max, light_range_min, light_intensity_min;

	public bool hasKey;

	public float health, maxHealth;

	private bool flickering;
	private float flicker_start;

	public float flicker_duration, flicker_delay;

	private float max_light_start, temp_range, temp_intensity;
	private Color temp_color;

	public Color onHit_color;

	private bool knockedBack;
	private Vector2 knock_destination;

	private SpriteRenderer sprite;

	public Animator anim;

	private float message_duration = 3;
	private float message_start;
	private bool shown_message = false;

	private Vector2 dir_normal;

	bool playSound = true;

	public void gameOver(){
		if (playSound) {
			FindObjectOfType<AudioManager> ().playSound ("lose");
			playSound = false;
		}
		Time.timeScale = 0;
		game_over.gameObject.SetActive (true);
	}

	public void noKey(){
		
		if (!shown_message) {
			Debug.Log ("!");
			Debug.Log (message == null);
			message.gameObject.SetActive (true);
			shown_message = true;
			message_start = Time.time;
		} 
	}

	// Use this for initialization
	void Awake () {
		sprite = GetComponent<SpriteRenderer> ();
		body = GetComponent<Rigidbody2D> ();
		light = GetComponentInChildren<Light> ();
		body.gravityScale = 0;
		light.range = 3.5f;
		light.intensity = 20;
	}

	public void pauseScreen(){
		if (!pause.gameObject.activeInHierarchy) {
			pause.gameObject.SetActive (true);
			Time.timeScale = 0;
		} else {
			pause.gameObject.SetActive (false);
			Time.timeScale = 1;
		}
	}

	public void reset(){
		transform.position = Vector3.zero;
		light.range = 3.5f;
		light.intensity = 20;
		health = maxHealth;
		GetComponent<Timer> ().resetTime ();
		if (game_over.gameObject.activeInHierarchy) {
			game_over.gameObject.SetActive (false);

		}
		if (pause.gameObject.activeInHierarchy) {
			pause.gameObject.SetActive (false);

		}
		if (message.gameObject.activeInHierarchy) {
			message.gameObject.SetActive (false);
		}

		shown_message = false;
		floors_traversed = 0;
		Time.timeScale = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {

			pauseScreen ();
		}

		if (!knockedBack) {



			float horizontal = Input.GetAxisRaw ("Horizontal");
			float vertical = Input.GetAxisRaw ("Vertical");
			float lt = Input.GetAxisRaw ("Light");

			if (lt > 0) {
				light.range = Mathf.Min (light.range + .1f, light_range_max);
				light.intensity = Mathf.Min (light.intensity + 1f, light_intensity_max);

			} else if (lt < 0) {
				light.range = Mathf.Max (light.range - .1f, light_range_min);
				light.intensity = Mathf.Max (light.intensity - 1f, light_intensity_min);
	

			}

			bool left, right, up, down;

			if (horizontal > 0) {
				right = true;
				left = false;
				dir_normal.x = 1;
			} else if (horizontal < 0) {
				left = true;
				right = false;
				dir_normal.x = -1;
			} else {
				left = right = false;

			}

			if (vertical > 0) {
				up = true;
				down = false;
				dir_normal.y = 1;
			} else if (vertical < 0) {
				up = false;
				down = true;
				dir_normal.y = -1;
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

			bool move = body.velocity != Vector2.zero;

			if (dir_normal.y < 0) {
				transform.localScale = new Vector3 (1, 1, 1);
				setDirectionAnimation (0, move);

			} else if (dir_normal.y > 0) {
				transform.localScale = new Vector3 (1, 1, 1);
				setDirectionAnimation (1, move);

			} else if (dir_normal.x < 0) {
				transform.localScale = new Vector3 (-1, 1, 1);
				setDirectionAnimation (2, move);
			}else if (dir_normal.x > 0) {
				transform.localScale = new Vector3 (1, 1, 1);
				setDirectionAnimation (2, move);
			} 
				
		} else {
			if (Mathf.Floor (transform.position.x) == Mathf.Floor (knock_destination.x)) {
				body.velocity = new Vector2 (0, body.velocity.y);
			}
			if (Mathf.Floor (transform.position.y) == Mathf.Floor (knock_destination.y)) {
				body.velocity = new Vector2 (body.velocity.x, 0);
			}

			float elapsed = Time.time - max_light_start;
			if (elapsed >= 1) {
				light.intensity = temp_intensity;
				light.range = temp_range;
				light.color = temp_color;
				max_light_start = temp_range = temp_intensity = 0;
			}

			if (body.velocity == Vector2.zero) {
				knockedBack = false;
				knock_destination = Vector2.zero;
				light.intensity = temp_intensity;
				light.color = temp_color;
				light.range = temp_range;
				max_light_start = temp_range = temp_intensity = 0;
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

		if (shown_message) {
			float elapsed = Time.time - message_start;
			if (elapsed >= message_duration) {
				shown_message = false;
				message.gameObject.SetActive (false);
			}
		}

		if (health <= 0) {
			gameOver ();
		}
	}

	void setDirectionAnimation(int i, bool moving){
		anim.SetInteger ("direction", i);
		anim.SetBool ("moving", moving);
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
			GetComponent<CameraShake> ().startShake ();
			StartCoroutine (Flicker ());
		}
	}

	public void knockBack(Vector2 dir, float knock_strength, float knock_modifier){
		if (!flickering) {
			FindObjectOfType<AudioManager> ().playSound ("hit");
			knockedBack = true;
			body.velocity = dir * speed * knock_modifier;
			knock_destination = new Vector2 (transform.position.x, transform.position.y) + (dir * (knock_strength * knock_modifier));

			temp_intensity = light.intensity;
			temp_range = light.range;
			temp_color = light.color;
			light.color = onHit_color;
			light.intensity = light_intensity_max;
			light.range = light_range_max;
			max_light_start = Time.time;
		}
	}

	private int floors_traversed = 0;
	public Text game_over_score;

	public void nextLevel(){
		floors_traversed++;
		game_over_score.text = "SCORE: " + floors_traversed.ToString ();

	}


}
