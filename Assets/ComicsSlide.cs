using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComicsSlide : MonoBehaviour {

	public AudioSource sound;
	public AudioSource voice;
	public float soundDelay = 0.2f;
	public float voiceDelay = 0.6f;

	public void PlaySound() {
		if (sound != null) {
			sound.PlayDelayed(soundDelay);
		}
		if (voice != null) {
			voice.PlayDelayed(voiceDelay);
		}
	}

	public void StopSound() {
		if (sound != null) {
			sound.Stop();
		}
		if (voice != null) {
			voice.Stop();
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
