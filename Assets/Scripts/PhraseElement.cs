using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhraseElement : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public static GameObject itemBeingDragged;
	Vector3 startPosition;
	Vector3 startOffset;

	public GameObject ArticleHeadline;
	public GameObject ArticleContent;
	public GameObject NewspaperLayout;

	public int spotIndex = -1;
	public int phraseIndex = -1;

	#region IBeginDragHandler implementation

	public void OnBeginDrag(PointerEventData eventData) {
		itemBeingDragged = gameObject;
		startPosition = transform.position;
		startOffset = Input.mousePosition - transform.position;
		ArticleManager.instance.StartDraggingForSpotType(this.spotIndex);
		this.gameObject.GetComponent<Image>().color = Color.cyan;
	}

	#endregion

	#region IDragHandler implementation

	public void OnDrag(PointerEventData eventData) {
		Input.multiTouchEnabled = false;
		transform.position = Input.mousePosition - startOffset;
		this.gameObject.GetComponent<Image>().color = Color.green;
	}

	#endregion

	#region IEndDragHandler implementation

	public void OnEndDrag(PointerEventData eventData) {
		this.gameObject.GetComponent<Image>().color = Color.red;

		itemBeingDragged = null;

		// Check if we collide with Headline

		ArticleManager.instance.StopDraggingForSpotType();

		Vector3 elementPosition = transform.position + startOffset;
		Rect articleHeadlineRect = ToWorldCoordinate(ArticleHeadline.GetComponent<RectTransform>());
		Rect articleContentRect = ToWorldCoordinate(ArticleContent.GetComponent<RectTransform>());
		Rect realNewspaperRect = ToWorldCoordinate(NewspaperLayout.GetComponent<RectTransform>());

		if (isInsideRect(elementPosition, realNewspaperRect)) {
			ArticleManager.instance.tryInsertPhraseIntoContextSpot(this, 0);
		}

//		if (isInsideRect(elementPosition, articleHeadlineRect)) {
//			Vector2 position = getPositionInsideRect(elementPosition, articleHeadlineRect);
//			// Try to understand what line it is
//			int line = Mathf.RoundToInt(position.y / ArticleHeadline.GetComponent<Text>().fontSize);
//			ArticleManager.instance.tryInsertPhraseIntoHeadlineSpot(this, line);
//		}
//		if (isInsideRect(elementPosition, articleContentRect)) {
//			Vector2 position = getPositionInsideRect(elementPosition, articleContentRect);
//			// Try to understand what line it is
//			int line = Mathf.RoundToInt(position.y / (ArticleContent.GetComponent<Text>().fontSize + 3));
//			ArticleManager.instance.tryInsertPhraseIntoContextSpot(this, line);
//		}

		transform.position = startPosition;
	}

	#endregion

	private Rect ToWorldCoordinate(RectTransform rectTransform) {
		Vector3[] corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		return new Rect(corners[0], new Vector2(rectTransform.rect.size.x, rectTransform.rect.size.y));
	}

	private bool isInsideRect(Vector3 position, Rect rect) {
		return rect.Contains(position);
	}

	private Vector2 getPositionInsideRect(Vector3 position, Rect rect) {
		Vector2 positionInsideRect = new Vector2(position.x, position.y) - rect.position;
		return new Vector2(positionInsideRect.x, rect.height - positionInsideRect.y);
	}
}
