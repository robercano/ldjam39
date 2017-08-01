using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMainMenuController : MonoBehaviour {

	private AudioSource audioSource;
	public AudioClip UISoundBreak;
	public AudioClip UIInitialSound;
	public AudioClip LogoPower;
	public AudioClip LogoBrawler;
	public AudioClip LogoLeague;

	void Start () {
		audioSource = gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlayUISound() {
		audioSource.PlayOneShot (UISoundBreak, 0.05f);
	}

	public void PlayInitialSound() {
		audioSource.PlayOneShot (UIInitialSound);
	}

	public void PlayLogoPower()
	{
		AudioSource.PlayClipAtPoint (LogoPower, Camera.main.transform.position);
	}

	public void PlayLogoBrawler()
	{
		AudioSource.PlayClipAtPoint (LogoBrawler, Camera.main.transform.position);
	}

	public void PlayLogoLeague()
	{
		AudioSource.PlayClipAtPoint (LogoLeague, Camera.main.transform.position, 0.6f);
	}

}
