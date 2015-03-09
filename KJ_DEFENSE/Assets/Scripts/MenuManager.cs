using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MenuManager : MonoBehaviour {

	public List<AudioClip> listAudioClip ;
	public float cdSound = 0.0f;
	public float cdSoundmax ;
	private int looptime =0;

	// Update is called once per frame
	void Update () {
		cdSound += Time.deltaTime;
		if (cdSound > cdSoundmax && looptime <= 1) {
			playSound(0);
			cdSound = 0;
			looptime++;
		}
	}
	private void playSound(int clip)
	{
		audio.clip = listAudioClip[clip];
		audio.Play();
	}
}
