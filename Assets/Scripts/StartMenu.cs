using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour {

	public void startGame(string scene){
		SceneManager.LoadScene (scene);
	}

	public void exitGame(){
		Application.Quit ();
	}

	public void playSelect(){
		FindObjectOfType<AudioManager> ().playSound ("select");
	}
}
