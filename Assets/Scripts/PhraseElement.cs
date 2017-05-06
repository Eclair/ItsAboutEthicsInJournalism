using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhraseElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	Vector3 startOffset;

	#region IBeginDragHandler implementation

	public void OnBeginDrag(PointerEventData eventData) {
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startOffset = Input.mousePosition - transform.position;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag(PointerEventData eventData) {
		transform.position = Input.mousePosition - startOffset;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag(PointerEventData eventData) {
		itemBeingDragged = null;
		transform.position = startPosition;
	}

	#endregion
}
