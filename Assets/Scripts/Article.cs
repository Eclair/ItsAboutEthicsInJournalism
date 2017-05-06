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
		public Phrase[] phrases;
	}

	[System.Serializable]
	public class Phrase {
		public string text;
		public int truth;
		public int catchy;
	}
}
