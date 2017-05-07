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
			comicsImages[comicsIndex].SetActive(comicsIndex == 0);
		}
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
			comicsImages[currentImage - 1].SetActive(true);
			SceneManager.LoadScene("ArticleWriting");
		}
	}
}
