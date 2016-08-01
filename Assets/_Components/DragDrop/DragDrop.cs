using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using DG.Tweening;

public class DragDrop : MonoBehaviour
{
	// DragDrop Group
	public GameObject dragDropContainer;
	public GameObject dragDropObject;

	// A certain distance to decide whether it is in the place or not  
	public Vector2 detectMargin;

	// Structure
	private RectTransform _dragDropRectTransform;
	private Vector2 _dragDropRectSize;

	// Drag Event Variables
	private int _originalOrder;
	private int _dragEndOrder;
	private GameObject _replacedObject;
	private Vector3 _realtimeDragPosition;
	private Vector3 _dragBeginPosition;
	private Vector3 _dragDeltaBeginPosition;
	private Vector3 _dragDeltaPosition;

	void Awake ()
	{
		_dragDropRectTransform = GetComponent<RectTransform> ();
		_dragDropRectSize = _dragDropRectTransform.sizeDelta;
	}

	void Start ()
	{
		
	}

	int GetObjectOrder ()
	{
		return dragDropObject.GetComponent<DragDrop_Object> ().objectOrder;
	}

	void GetDragEndOrder ()
	{
//		Debug.Log (_dragEndOrder);
	}

	public void OnDragBegin ()
	{
		// Save current Team Member Order
		_originalOrder = GetObjectOrder ();

		// Display in the top layer
		dragDropObject.transform.SetAsLastSibling ();

		_dragBeginPosition = _dragDropRectTransform.localPosition;
		_dragDeltaBeginPosition = Input.mousePosition;
	}

	public void OnDrag ()
	{
		_realtimeDragPosition = Input.mousePosition;

		_dragDeltaPosition = _realtimeDragPosition - _dragDeltaBeginPosition;

		_dragDropRectTransform.localPosition = new Vector3 (_dragDeltaPosition.x, _dragDeltaPosition.y, 0);

	}

	public void OnDragEnd ()
	{
		// Get Drag Member's DragEnd Order
		GetDragEndOrder ();

		// Get Replaced Member by DragEnd Order
//		_replacedObject = dragDropContainer.GetComponent<DragDrop_Container> ().GetObjectByOrder (_dragEndOrder);

		// Change _replacedMember's order to _originalOrder
//		_replacedObject.GetComponent<DragDrop_Object> ().ChangeObjectOrder (_originalOrder);

		// Change DragMember (this Member) 's order to dragEndOrder
//		dragDropObject.GetComponent<DragDrop_Object> ().ChangeObjectOrder (_dragEndOrder);

		// Change GameObject of _replaceMember and DragMember
//		dragDropContainer.GetComponent<DragDrop_Container> ().ChangeObjectByOrder (_dragEndOrder, _originalOrder);

		// Change Position of _replacedMember and DragMember
//		dragDropContainer.GetComponent<DragDrop_Container> ().ChangeObjectPosition (dragDropObject, _replacedObject);

	}
}
