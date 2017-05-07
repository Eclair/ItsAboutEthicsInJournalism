using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class ArticleManager : MonoBehaviour {

	public static ArticleManager instance;

	void Awake() {
		instance = this;
	}

	public GameObject articleHeadline;
	public GameObject articleContent;
	public PhrasesPanel phrasesPanel;

	public bool isDragging = false;

	private Article article;
	private bool wasUpdated = false;
	private List<int> spotsInHeader;
	private List<List<int>> spotsInContent;

	// Use this for initialization
	void Start () {
		LoadArticle(GameManager.instance.currentLoadedArticle);
	}
	
	// Update is called once per frame
	void Update () {
		if (wasUpdated) {
			wasUpdated = false;
			updateLayout();
		}
	}

	private void LoadArticle(Article article) {
		this.article = article;
		this.spotsInHeader = getSpots(article.headline);
		this.spotsInContent = new List<List<int>>();
		foreach (string context in article.context) {
			this.spotsInContent.Add(getSpots(context));
		}
		wasUpdated = true;
	}

	public void tryInsertPhraseIntoHeadlineSpot(PhraseElement phraseElement, int line) {
		int spotId = getHeadlineSpotByLine(line);
		if (spotId < 0) {
			// No Spot in this line
			return;
		}
		if (phraseElement.spotIndex != spotId) {
			// This phrase is not for this spot!
			return;
		}
		article.spots[spotId]._selectedPhrase = phraseElement.phraseIndex;
		wasUpdated = true;
	}

	private int getHeadlineSpotByLine(int line) {
		for (int spotIndex = 0; spotIndex < spotsInHeader.Count; spotIndex++) {
			int spotId = spotsInHeader[spotIndex];
			if (line >= article.spots[spotId].fromLine && line <= article.spots[spotId].toLine) {
				return spotId;
			}
		}
		return -1;
	}

	public void tryInsertPhraseIntoContextSpot(PhraseElement phraseElement, int line) {
		int spotId = getContextSpotByLine(line);
		if (spotId < 0) {
			// No Spot in this line
			return;
		}
		if (phraseElement.spotIndex != spotId) {
			// This phrase is not for this spot!
			return;
		}
		article.spots[spotId]._selectedPhrase = phraseElement.phraseIndex;
		wasUpdated = true;
	}

	private int getContextSpotByLine(int line) {
		for (int contextIndex = 0; contextIndex < spotsInContent.Count; contextIndex++) {
			List<int> contextSpots = spotsInContent[contextIndex];
			for (int spotIndex = 0; spotIndex < contextSpots.Count; spotIndex++) {
				int spotId = contextSpots[spotIndex];
				if (line >= article.spots[spotId].fromLine && line <= article.spots[spotId].toLine) {
					return spotId;
				}
			}
		}
		return -1;
	}

	private void updateLayout() {
//		article.spots[0]._selectedPhrase = 0;
//		article.spots[1]._selectedPhrase = 0;
//		article.spots[2]._selectedPhrase = 0;
		updateHeadline();
		updateContext();
		updatePhrases();
	}

	private void updateHeadline() {
		string headlineText = this.article.headline;
		for (int spotIndex = 0; spotIndex < spotsInHeader.Count; spotIndex++) {
			int spotId = spotsInHeader[spotIndex];
			Article.Spot spot = article.spots[spotId];
			if (spot._selectedPhrase < 0) {
				// Put empty space there:
				headlineText = headlineText.Replace("$" + spotId, getEmptySpaceForSpot(spot.length));
			} else {
				// Put selected phrase:
				headlineText = headlineText.Replace("$" + spotId, spot.phrases[spot._selectedPhrase].text);
			}
		}
		this.articleHeadline.GetComponent<Text>().text = headlineText;
	}

	private void updateContext() {
		string resultContext = "";
		for (int contextIndex = 0; contextIndex < article.context.Length; contextIndex++) {
			if (contextIndex > 0) {
				resultContext += "\n\n";
			}
			string contextText = article.context[contextIndex];
			List<int> spotsInContext = spotsInContent[contextIndex];
			for (int spotIndex = 0; spotIndex < spotsInContext.Count; spotIndex++) {
				int spotId = spotsInContext[spotIndex];
				Article.Spot spot = article.spots[spotId];
				if (spot._selectedPhrase < 0) {
					// Put empty space there:
					contextText = contextText.Replace("$" + spotId, getEmptySpaceForSpot(spot.length));
				} else {
					// Put selected phrase:
					contextText = contextText.Replace("$" + spotId, spot.phrases[spot._selectedPhrase].text.Replace("\n", " "));
				}
			}
			resultContext += contextText;
		}
		this.articleContent.GetComponent<Text>().text = resultContext;
	}

	private void updatePhrases() {
		List<Article.Phrase> phrases = new List<Article.Phrase>();
		for (int spotIndex = 0; spotIndex < article.spots.Length; spotIndex++) {
			for (int phraseIndex = 0; phraseIndex < article.spots[spotIndex].phrases.Length; phraseIndex++) {
				Article.Phrase phrase = article.spots[spotIndex].phrases[phraseIndex];
				phrase._spotIndex = spotIndex;
				phrase._phraseIndex = phraseIndex;
				phrases.Add(phrase);
			}
		}
		phrasesPanel.updateWithPhrases(phrases, article.spots);
	}

	private string getEmptySpaceForSpot(int length) {
		string emptySpace = "";
		for (int i = 0; i < length && i < 20; i++) {
			emptySpace += "_";
		}
		return emptySpace;
	}

	private List<int> getSpots(string paragraph) {
		List<int> indexes = new List<int>();
		foreach (Match match in new Regex(@"\$(\d+)").Matches(paragraph)) {
			indexes.Add(int.Parse("" + match.Groups[1]));
		}
		return indexes;
	}
}
