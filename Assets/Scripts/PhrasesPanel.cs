using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhrasesPanel : MonoBehaviour {

	public List<GameObject> phrasesPlaceholders;

	// Use this for initialization
	void Start () {
		updateWithPhrases(new string[] {
			"Play Video Games in Their Underwear",
			"Cover Today's Top Issues",
			"Fake News",
			"Skeletor",
			"training dancing circus bears",
			"reporting with top-tier newspapers",
			"winning hot-dog eating contests"
		});
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateWithPhrases(string[] phrases) {
		for (int phraseIndex = 0; phraseIndex < phrasesPlaceholders.Count; phraseIndex++) {
			phrasesPlaceholders[phraseIndex].SetActive(phraseIndex < phrases.Length);
			if (phraseIndex < phrases.Length) {
				phrasesPlaceholders[phraseIndex].GetComponentInChildren<Text>().text = phrases[phraseIndex];
			}
		}
	}
}
