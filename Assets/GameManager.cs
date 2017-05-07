using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	void Awake() {
		if (instance == null) {
			DontDestroyOnLoad (gameObject);
			instance = this;
		}  else if (instance != this) {
			Destroy(gameObject);
		}
	}

	public List<string> Articles;
	private int currentArticle = 0;
	public Article currentLoadedArticle;

	public void onStartButtonClicked() {
		currentArticle = 0;
		LoadArticle(Articles[currentArticle]);
		SceneManager.LoadScene("InitialComics");
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void LoadArticle(string article) {
		TextAsset targetFile = Resources.Load<TextAsset>(article);

		if (targetFile != null) {
			currentLoadedArticle = JsonUtility.FromJson<Article>(targetFile.text);
		} else {
			Debug.LogError("Cannot load game data!");
		}
	}
}
