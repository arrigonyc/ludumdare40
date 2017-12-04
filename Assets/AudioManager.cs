using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour {

	public Sound[] sounds;

	public static AudioManager instance;

	// Use this for initialization
	void Awake () {

		if (instance == null) {
			instance = this;
			
		} else {
			Destroy (gameObject);
			return;
		}

		DontDestroyOnLoad (gameObject);

		for (int i = 0; i < sounds.Length; i++) {
			Sound s = sounds [i];
			s.source = gameObject.AddComponent<AudioSource> ();

			s.source.clip = s.file;
			s.source.volume = s.volume;
			s.source.pitch = s.pitch;
			s.source.loop = s.loop;
		}

		playSound ("music");
	}

	public void playSound(string name){

		Sound s = Array.Find (sounds, sound => sound.name == name);
		if (s == null)
			return;
		s.source.Play ();
	}
}
