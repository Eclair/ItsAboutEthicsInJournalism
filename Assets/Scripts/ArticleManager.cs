using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ArticleManager : MonoBehaviour {

	public string article;

	public Article loadedArticle;

	// Use this for initialization
	void Start () {
		LoadArticle(article);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void LoadArticle(string article) {
		string filePath = Path.Combine("Assets/Articles", article);

		if (File.Exists(filePath)) {
			loadedArticle = JsonUtility.FromJson<Article>(File.ReadAllText(filePath));
		} else {
			Debug.LogError("Cannot load game data!");
		}
	}

	private void DebugPrint(Article article) {
		Debug.Log("======= Article =========");
		Debug.Log("Headline:");
		Debug.Log("\"" + article.headline + "\"");
		Debug.Log("Context:");

		foreach (string paragraph in article.context) {
			Debug.Log(" > Paragraph: \"" + paragraph + "\"");
		}

		Debug.Log("score.truth: " + article.score.truth);
		Debug.Log("score.catchy: " + article.score.catchy);

		foreach (Article.Spot spot in article.spots) {
			Debug.Log(" > Spot: length: " + spot.length + " lines: " + spot.fromLine + "-" + spot.toLine + "; ");
			foreach (Article.Phrase phrase in spot.phrases) {
				Debug.Log(" >>> Phrase: truth: " + phrase.truth + " catchy: " + phrase.catchy + " text: " + phrase.text + ";");
			}
		}

		Debug.Log("=========================");
	}
}
