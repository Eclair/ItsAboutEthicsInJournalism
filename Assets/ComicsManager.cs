using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicsManager : MonoBehaviour {

	public List<GameObject> comicsImages;
	private int currentImage = 0;

	// Use this for initialization
	void Start () {
		for (int comicsIndex = 0; comicsIndex < comicsImages.Count; comicsIndex++) {
			comicsImages[comicsIndex].SetActive(false);
		}
		showComics(comicsImages[0]);
	}

	public void showComics(GameObject comics) {
		comics.SetActive(true);
		if (comics.GetComponent<ComicsSlide>() != null) {
			comics.GetComponent<ComicsSlide>().PlaySound();
		}
	}

	public void hideComics(GameObject comics) {
		comics.SetActive(false);
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void onComicsClick() {
		if (currentImage != comicsImages.Count - 1) {
			hideComics(comicsImages[currentImage]);
		}
		currentImage++;
		if (currentImage < comicsImages.Count) {
			showComics(comicsImages[currentImage]);
		} else {
			SceneManager.LoadScene("ArticleWriting");
		}
	}
}
