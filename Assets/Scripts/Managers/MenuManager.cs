using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour {

	private Animator cameraAnimator;

	public GameObject areYouSurePanel;

	private void Start () {
		cameraAnimator = GetComponent<Animator> ();
	}

	public void Position2 () {
		cameraAnimator.SetFloat ("Animate", 1);
	}

	public void Position1 () {
		cameraAnimator.SetFloat ("Animate", 0);
	}

	public void PlayHover () {
		AudioManager.instance.PlaySound ("Hover");
	}

	public void PlaySelect () {
		AudioManager.instance.PlaySound ("Select");
	}

	public void OnLoadGame () {
		StartCoroutine (LoadGame ());
	}

	public IEnumerator LoadGame () {
		AsyncOperation scene = SceneManager.LoadSceneAsync (1, LoadSceneMode.Single);
		scene.allowSceneActivation = true;
		while (!scene.isDone) {
			yield return null;
		}
	}

	public void PlayWindow () {
		AudioManager.instance.PlaySound ("Window");
	}

	public void PlayBack () {
		AudioManager.instance.PlaySound ("Back");
	}

	public void AreYouSure () {
		areYouSurePanel.SetActive (true);
	}

	public void No () {
		areYouSurePanel.SetActive (false);
	}

	public void Yes () {
		Application.Quit ();
	}
}