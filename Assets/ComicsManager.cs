using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ComicsManager : MonoBehaviour {

	public List<GameObject> comicsImages;
	private int currentImage = 0;

	// Use this for initialization
	void Start () {
		foreach (GameObject comics in comicsImages) {
			comics.SetActive(false);
		}

		comicsImages[0].SetActive(true);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onComicsClick() {
		comicsImages[currentImage].SetActive(false);
		currentImage++;
		if (currentImage < comicsImages.Count) {
			comicsImages[currentImage].SetActive(true);
		} else {
			// TODO: GO TO ARTICLE
			SceneManager.LoadScene("ArticleWriting");
		}
	}
}
