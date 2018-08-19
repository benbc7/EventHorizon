using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public delegate void PauseDelegate ();

	public event PauseDelegate PauseEvent;

	public static GameManager instance;

	public GameObject inGamePanel;
	public GameObject pausePanel;
	public GameObject deathPanel;
	public GameObject winPanel;

	private bool gameOver;
	private bool paused;

	private void Awake () {

		//DontDestroyOnLoad (gameObject);
		if (instance == null) {
			instance = this;
		} else {
			Destroy (gameObject);
		}
	}

	private void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Pause ();
		}
	}

	public void OnPlayerDeath () {
		if (!gameOver) {
			inGamePanel.SetActive (false);
			pausePanel.SetActive (false);
			deathPanel.SetActive (true);
			gameOver = true;
		}
	}

	public void OnBossDeath () {
		if (!gameOver) {
			inGamePanel.SetActive (false);
			pausePanel.SetActive (false);
			winPanel.SetActive (true);
			gameOver = true;
		}
	}

	public void Pause () {
		if (PauseEvent != null && !gameOver) {
			paused = !paused;
			PauseEvent ();
			Time.timeScale = (paused) ? 0f : 1f;
			inGamePanel.SetActive (!paused);
			pausePanel.SetActive (paused);
		}
	}

	public void Restart () {
		Time.timeScale = 1f;
		SceneManager.LoadSceneAsync (1);
	}

	public void MainMenu () {
		Time.timeScale = 1f;
		SceneManager.LoadSceneAsync (0);
	}

	public void Quit () {
		Application.Quit ();
	}
}