using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

	public AudioClip[] audioClips;

	private AudioSource audioSource;

	public void PlayRandomMusic()
	{
		int audioClipIndex = Random.Range(0, audioClips.Length-1);

		audioSource.clip = audioClips[audioClipIndex];

		audioSource.Play();
	}

	public void PlayLevelMusic(int level)
	{
		if (level >= audioClips.Length) {
			PlayRandomMusic ();
			return;
		}

		audioSource.clip = audioClips[level-1];

		audioSource.Play();
	}

	void Awake () {
		audioSource = GetComponent<AudioSource> ();
	}
}
