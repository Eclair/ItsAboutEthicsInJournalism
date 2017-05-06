using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArticleContent : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnMouseDown() {
		Debug.Log("MOUSE DOWN");
	}

	public void OnBeginDrag(PointerEventData eventData) {
		Debug.Log("Drag Begin Over Article Content");
	}

	public void OnDrag(PointerEventData eventData) {
		Debug.Log("Drag Over Article Content");
	}

	public void OnEndDrag(PointerEventData eventData) {
		Debug.Log("Drag End Over Article Content");
	}
}
