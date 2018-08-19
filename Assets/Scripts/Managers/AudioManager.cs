using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound {

	private AudioSource source;

	public string clipName;
	public AudioClip singleClip;
	public bool randomize = false;
	public AudioClip[] randomClips;

	public float volume;
	public float pitch;
	public AudioMixerGroup audioMixerGroup;

	public bool loop = false;
	public bool playOnAwake = false;

	public void SetSource (AudioSource _source) {

		source = _source;
		source.volume = volume;
		source.pitch = pitch;
		source.loop = loop;
		source.playOnAwake = playOnAwake;
		source.outputAudioMixerGroup = audioMixerGroup;

		if (randomize && randomClips.Length > 0) {
			source.clip = randomClips [Random.Range (0, randomClips.Length)];
			source.pitch = Random.Range (0.5f, 1.5f);
		} else {
			source.clip = singleClip;
		}

		if (playOnAwake) {
			source.Play ();
		}
	}

	public void Play () {
		if (randomize) {
			source.pitch = Random.Range (0.5f, 1.5f);
		}
		if (randomClips.Length > 0) {
			source.PlayOneShot (randomClips [Random.Range (0, randomClips.Length)]);
		} else {
			source.Play ();
		}
	}

	public void Stop () {
		source.Stop ();
	}
}

public class AudioManager : MonoBehaviour {

	public static AudioManager instance;

	[SerializeField]
	private Sound[] sound;

	private void Awake () {
		DontDestroyOnLoad (gameObject);
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy (gameObject);
	}

	private void Start () {

		for (int i = 0; i < sound.Length; i++) {
			GameObject _go = new GameObject ("Sound_" + i + "_" + sound [i].clipName);
			_go.transform.SetParent (this.transform);
			sound [i].SetSource (_go.AddComponent<AudioSource> ());
		}

		PlaySound ("Background");
	}

	public void PlaySound (string _name) {
		for (int i = 0; i < sound.Length; i++) {
			if (sound [i].clipName == _name) {
				sound [i].Play ();
				return;
			}
		}
	}

	public void StopSound (string _name) {
		for (int i = 0; i < sound.Length; i++) {
			if (sound [i].clipName == _name) {
				sound [i].Stop ();
				return;
			}
		}
	}
}