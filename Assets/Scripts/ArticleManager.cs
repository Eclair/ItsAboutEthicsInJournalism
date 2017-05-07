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
	public Animator newspaperAnimator;
	public GameObject hand;
	public GameObject handWithStamp;
	public GameObject passStamp;
	public GameObject rejectStamp;

	public GameObject buttonsPanel;
	public GameObject resultsPanel;
	public Slider completeSlider;
	public Slider truthSlider;
	public Slider cachySlider;

	private Article article;
	private bool wasUpdated = false;
	private bool wasUpdatedPanel = false;
	private List<int> spotsInHeader;
	private List<List<int>> spotsInContent;

	public bool isWaitingForResults = false;
	private List<int> highlightedResultRedSpots = new List<int>();
	private List<int> highlightedResultOrangeSpots = new List<int>();
	private float resultCompletion = 0;
	private float resultTruthfulness = 0;
	private float resultCachyness = 0;

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
		if (wasUpdatedPanel) {
			wasUpdatedPanel = false;
			updatePhrases();
		}
	}

	private int highlightedSpotId = -1;

	public void StartDraggingForSpotType(int spotId) {
		highlightedSpotId = spotId;
		wasUpdated = true;
	}

	public void StopDraggingForSpotType() {
		highlightedSpotId = -1;
		wasUpdated = true;
	}

	public void OnRotateArticlePressed() {
		GameManager.instance.RotateArticles();
		newspaperAnimator.SetBool("NewspaperIsShown", false);
		buttonsPanel.SetActive(false);
		phrasesPanel.gameObject.SetActive(false);
		StartCoroutine(resetArticleAfterDelay());
	}

	private IEnumerator resetArticleAfterDelay() {
		yield return new WaitForSeconds(1.5f);

		buttonsPanel.SetActive(true);
		LoadArticle(GameManager.instance.currentLoadedArticle);
	}

	public void OnSubmitForApprovalPressed() {
		isWaitingForResults = true;
		phrasesPanel.gameObject.SetActive(false);
		buttonsPanel.SetActive(false);

		resultCompletion = article.getTotalCompleteness();
		resultTruthfulness = article.getTotalTruthfulness();
		resultCachyness = article.getTotalCachyness();

		hand.SetActive(true);
		hand.GetComponent<Animator>().SetTrigger("StartScan");

		StartCoroutine(OnHandAnimationEnded());
	}

	private IEnumerator OnHandAnimationEnded() {
		yield return new WaitForSeconds(1.6f);

		hand.SetActive(false);

		highlightResults();

		StampIT(); // STAMP IT BEBE !!!!!!!!!!
	}

	private void StampIT() {
		handWithStamp.SetActive(true);
		handWithStamp.GetComponent<Animator>().SetTrigger("StampIt");

		StartCoroutine(OnStampAnimationEnded());
	}

	private IEnumerator OnStampAnimationEnded() {
		yield return new WaitForSeconds(0.5f);

		showStamp();

		yield return new WaitForSeconds(0.5f);

		handWithStamp.SetActive(false);

		completeSlider.value = resultCompletion;
		truthSlider.value = resultTruthfulness;
		cachySlider.value = resultCachyness;

		resultsPanel.SetActive(true);

		// TODO: SHOW RESULTS! (Images?)
	}

	public void OnResultsRetryClick() {
		resultsPanel.SetActive(false);

		newspaperAnimator.SetBool("NewspaperIsShown", false);

		StartCoroutine(OnNewsPaperHide(false));
	}

	public void OnResultsNextArticleClick() {
		resultsPanel.SetActive(false);

		newspaperAnimator.SetBool("NewspaperIsShown", false);

		StartCoroutine(OnNewsPaperHide(true));
	}

	private IEnumerator OnNewsPaperHide(bool rotate) {
		yield return new WaitForSeconds(1.8f);

		isWaitingForResults = false;

		if (rotate) {
			GameManager.instance.RotateArticles();
		}
		LoadArticle(GameManager.instance.currentLoadedArticle);
	}

	private void showStamp() {
		bool pass = resultTruthfulness >= 0.5f;
		passStamp.SetActive(pass);
		rejectStamp.SetActive(!pass);
	}

	private void highlightResults() {
		highlightedResultRedSpots.Clear();
		highlightedResultOrangeSpots.Clear();

		for (int spotIndex = 0; spotIndex < article.spots.Length; spotIndex++) {
			Article.Spot spot = article.spots[spotIndex];
			if (spot._selectedPhrase < 0) {
				// Nothing selected
				highlightedResultRedSpots.Add(spotIndex);
			} else {
				Article.Phrase phrase = spot.phrases[spot._selectedPhrase];
				if (phrase.truth == 0) {
					highlightedResultOrangeSpots.Add(spotIndex);
				}
			}
		}

		wasUpdated = true;
	}

	private void LoadArticle(Article article) {
		this.article = article;
		this.article.resetSelection();
		this.spotsInHeader = getSpots(article.headline);
		this.spotsInContent = new List<List<int>>();
		foreach (string context in article.context) {
			this.spotsInContent.Add(getSpots(context));
		}
		wasUpdated = true;
		wasUpdatedPanel = true;

		buttonsPanel.SetActive(false);
		resultsPanel.SetActive(false);
		phrasesPanel.gameObject.SetActive(false);
		passStamp.SetActive(false);
		rejectStamp.SetActive(false);

		newspaperAnimator.SetBool("NewspaperIsShown", true);

		StartCoroutine(OnArticleLoaded());
	}

	private IEnumerator OnArticleLoaded() {
		yield return new WaitForSeconds(1.7f);

		phrasesPanel.gameObject.SetActive(true);
		buttonsPanel.SetActive(true);
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
		wasUpdatedPanel = true;
	}

	private int getHeadlineSpotByLine(int line) {
		for (int spotIndex = 0; spotIndex < spotsInHeader.Count; spotIndex++) {
			int spotId = spotsInHeader[spotIndex];
//			if (line >= article.spots[spotId].fromLine && line <= article.spots[spotId].toLine) {
				return spotId;
//			}
		}
		return -1;
	}

	public void tryInsertPhraseIntoContextSpot(PhraseElement phraseElement, int line) {
//		int spotId = getContextSpotByLine(line);
//		if (spotId < 0) {
//			// No Spot in this line
//			return;
//		}
//		if (phraseElement.spotIndex != spotId) {
//			// This phrase is not for this spot!
//			return;
//		}
		int spotId = phraseElement.spotIndex;
		article.spots[spotId]._selectedPhrase = phraseElement.phraseIndex;
		wasUpdated = true;
		wasUpdatedPanel = true;
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
		updateHeadline();
		updateContext();
	}

	private void updateHeadline() {
		string headlineText = this.article.headline;
		for (int spotIndex = 0; spotIndex < spotsInHeader.Count; spotIndex++) {
			int spotId = spotsInHeader[spotIndex];
			Article.Spot spot = article.spots[spotId];
			if (spot._selectedPhrase < 0) {
				// Put empty space there:
				string color = 
					isWaitingForResults && spotHasHighlight(spotId) 
					? spotHighlight(spotId) 
					: (spotId == highlightedSpotId ? "orange" : "olive");
				headlineText = headlineText.Replace("$" + spotId, getEmptySpaceForSpot(spot.length, color));
			} else {
				// Put selected phrase:
				headlineText = headlineText.Replace("$" + spotId, printHeadlineSpot(spot.phrases[spot._selectedPhrase].text, spotId));
			}
		}
		this.articleHeadline.GetComponent<Text>().text = headlineText;
	}

	private string printHeadlineSpot(string spotText, int spotId) {
		return "<b><i><size=25>" 
			+ (
				isWaitingForResults
				? (spotHasHighlight(spotId) ? "<color=" + spotHighlight(spotId) + ">" : "")
				: (spotId == highlightedSpotId ? "<color=orange>" : "")
			)
			+ spotText 
			+ (
				isWaitingForResults
				? (spotHasHighlight(spotId) ? "</color>" : "")
				: (spotId == highlightedSpotId ? "</color>" : "")
			)
			+ " </size></i></b>";
	}

	private bool spotHasHighlight(int spotId) {
		return highlightedResultRedSpots.Contains(spotId) || highlightedResultOrangeSpots.Contains(spotId);
	}

	private string spotHighlight(int spotId) {
		if (highlightedResultRedSpots.Contains(spotId)) {
			return "red";
		}
		return "orange";
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
					string color = 
						isWaitingForResults && spotHasHighlight(spotId) 
						? spotHighlight(spotId) 
						: (spotId == highlightedSpotId ? "orange" : "olive");
					contextText = contextText.Replace("$" + spotId, getEmptySpaceForSpot(spot.length, color));
				} else {
					// Put selected phrase:
					contextText = contextText.Replace("$" + spotId, printContextSpot(spot.phrases[spot._selectedPhrase].text.Replace("\n", " "), spotId));
				}
			}
			resultContext += contextText;
		}
		this.articleContent.GetComponent<Text>().text = resultContext;
	}

	private string printContextSpot(string spotText, int spotId) {
		return "<b><i><size=15>"
			+ (
				isWaitingForResults
				? (spotHasHighlight(spotId) ? "<color=" + spotHighlight(spotId) + ">" : "")
				: (spotId == highlightedSpotId ? "<color=orange>" : "")
			)
			+ spotText
			+ (
				isWaitingForResults
				? (spotHasHighlight(spotId) ? "</color>" : "")
				: (spotId == highlightedSpotId ? "</color>" : "")
			)
			+ " </size></i></b>";
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

	private string getEmptySpaceForSpot(int length, string color) {
		string emptySpace = "<color=" + color + ">";
		for (int i = 0; i < length; i++) {
			string maskSymbols = "%@#$!+=";
			emptySpace += maskSymbols[Random.Range(0, maskSymbols.Length)];//"█";
		}
		return emptySpace + "</color>";
	}

	private List<int> getSpots(string paragraph) {
		List<int> indexes = new List<int>();
		foreach (Match match in new Regex(@"\$(\d+)").Matches(paragraph)) {
			indexes.Add(int.Parse("" + match.Groups[1]));
		}
		return indexes;
	}
}
