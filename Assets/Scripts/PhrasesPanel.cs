using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhrasesPanel : MonoBehaviour {

	public List<GameObject> phrasesPlaceholders;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void updateWithPhrases(List<Article.Phrase> phrases, Article.Spot[] spots) {
		for (int phraseIndex = 0; phraseIndex < phrasesPlaceholders.Count; phraseIndex++) {
			if (phraseIndex < phrases.Count) {
				phrasesPlaceholders[phraseIndex].SetActive(true);
				PhraseElement phraseElement = phrasesPlaceholders[phraseIndex].GetComponentInChildren<PhraseElement>();
				phraseElement.phraseIndex = phrases[phraseIndex]._phraseIndex;
				phraseElement.spotIndex = phrases[phraseIndex]._spotIndex;
				phrasesPlaceholders[phraseIndex].GetComponentInChildren<Text>().text = phrases[phraseIndex].text;
				if (spots[phraseElement.spotIndex]._selectedPhrase == phraseElement.phraseIndex) {
					// Hide element because it's currently selected
					phrasesPlaceholders[phraseIndex].SetActive(false);
				}
			} else {
				// No such phrase
				phrasesPlaceholders[phraseIndex].SetActive(false);
			}
		}
	}
}
