using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Article {
	public string headline;
	public string[] context;
	public ArticleScore score;
	public Spot[] spots;

	[System.Serializable]
	public class ArticleScore {
		public int truth;
		public int catchy;
	}

	[System.Serializable]
	public class Spot {
		public int length;
		public int fromLine;
		public int toLine;
		public int _selectedPhrase = -1;
		public Phrase[] phrases;
	}

	[System.Serializable]
	public class Phrase {
		public string text;
		public int truth;
		public int catchy;
		public int _spotIndex = -1;
		public int _phraseIndex = -1;
	}

	public void resetSelection() {
		for (int spotIndex = 0; spotIndex < spots.Length; spotIndex++) {
			spots[spotIndex]._selectedPhrase = -1;
		}
	}

	public float getTotalCompleteness() {
		float filledSpots = 0;
		for (int spotIndex = 0; spotIndex < spots.Length; spotIndex++) {
			if (spots[spotIndex]._selectedPhrase >= 0) {
				filledSpots++;
			}
		}
		return filledSpots / spots.Length;
	}

	public float getTotalTruthfulness() {
		float totalTruth = 0;
		for (int spotIndex = 0; spotIndex < spots.Length; spotIndex++) {
			Spot spot = spots[spotIndex];
			if (spot._selectedPhrase >= 0) {
				totalTruth += spot.phrases[spot._selectedPhrase].truth;
			}
		}
		return totalTruth / score.truth;
	}

	public float getTotalCachyness() {
		float totalCachyness = 0;
		for (int spotIndex = 0; spotIndex < spots.Length; spotIndex++) {
			Spot spot = spots[spotIndex];
			if (spot._selectedPhrase >= 0) {
				totalCachyness += spot.phrases[spot._selectedPhrase].catchy;
			}
		}
		return totalCachyness / score.catchy;
	}
}
