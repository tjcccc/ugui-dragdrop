using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{
	// DragDrop Group
	[HideInInspector] public GameObject DragDropContainer;
	[HideInInspector] public GameObject DragDropObject;

	// DragDrop Data
	[HideInInspector] public Vector3 DragDropOriginalPosition;

	// Structure
	private RectTransform _dragDropRectTransform;
	private RectTransform _objectRectTransform;
	private Vector2 _objectRectSize;

	// Drag Event Variables
	private int _dragDropAmount;
	private int _originalOrder;
	private int _dragEndOrder;
	private GameObject _replacedObject;
	private Vector3 _realtimeDragPosition;
	private Vector3 _dragBeginPosition;
	private Vector3 _dragEndPosition;
	private float _spacingX;
	private float _spacingY;
	private int _row;
	private int _column;
	private Vector3 _dragDeltaBeginPosition;
	private Vector3 _dragDeltaPosition;

	private void Awake()
	{
		_dragDropRectTransform = this.GetComponent<RectTransform>();
		DragDropOriginalPosition = _dragDropRectTransform.localPosition;
	}

	public void ConnectRelatives()
	{
		DragDropContainer = this.transform.parent.parent.gameObject;
		DragDropObject = this.transform.parent.gameObject;

		_objectRectSize = DragDropContainer.GetComponent<GridLayoutGroup>().cellSize;
		_objectRectTransform = DragDropObject.GetComponent<RectTransform>();
	}

	private void GetContainerStructure()
	{
		_spacingX = DragDropContainer.GetComponent<GridLayoutGroup>().spacing.x;
		_spacingY = DragDropContainer.GetComponent<GridLayoutGroup>().spacing.y;
		_column = DragDropContainer.GetComponent<DragDropContainer>().Column;
		_row = DragDropContainer.GetComponent<DragDropContainer>().Row;
	}

	private Vector3 GetDragEndPosition()
	{
		return transform.localPosition;
	}

	private int GetObjectOrder()
	{
		return DragDropObject.GetComponent<DragDropObject>().ObjectOrder;
	}

	private static int GetClosetInteger(float f, int limit)
	{
		if (f < 0)
		{
			return 0;
		}
		else if (f > (limit - 1))
		{
			return limit - 1;
		}
		else if (f >= 0 && f - (int)f >= 0.5f)
		{
			return (int)f + 1;
		}
		else
		{
			return (int)f;
		}
	}

	private int GetDragEndOrder()
	{
		GetContainerStructure();

		// Get Drag Object's DragEnd Position
		_dragEndPosition = GetDragEndPosition();

		// Get arrange order (row, column)
		// [(final + rectW/2) - spaceX(x-1)] / rectW = x
		var rowX = ((0 - (_objectRectTransform.localPosition.y + _dragEndPosition.y)) + (_objectRectSize.y * 0.5f) + _spacingY) / (_objectRectSize.y + _spacingY) - 1;
		var columnY = (_objectRectTransform.localPosition.x + _dragEndPosition.x + (_objectRectSize.x * 0.5f) + _spacingX) / (_objectRectSize.x + _spacingX) - 1;

//		Debug.Log("finalX: " + (_objectRectTransform.localPosition.x + _dragEndPosition.x).ToString());

//		Debug.Log("rowX & columnY: " + rowX.ToString() + ", " + columnY.ToString());

		var orderX = GetClosetInteger(rowX, _row);
		var orderY = GetClosetInteger(columnY, _column);

//		Debug.Log("order x & y: " + orderX.ToString() + ", " + orderY.ToString());

		_dragDropAmount = DragDropContainer.GetComponent<DragDropContainer>().DragDropObject.Length;
		var order = (_column * orderX + orderY) <= (_dragDropAmount - 1) ? (_column * orderX + orderY) : (_dragDropAmount - 1);

//		Debug.Log("order: " + order);

		return order;
	}

	/// <summary>
	/// Raises the drag begin event.
	/// </summary>
	public void OnDragBegin()
	{
		DragDropContainer.GetComponent<GridLayoutGroup>().enabled = false;

		// Save current Team Member Order
		_originalOrder = GetObjectOrder();

		// Display in the top layer
		DragDropObject.transform.SetAsLastSibling();

		_dragBeginPosition = _dragDropRectTransform.localPosition;
		_dragDeltaBeginPosition = Input.mousePosition;
	}

	/// <summary>
	/// Raises the drag event.
	/// </summary>
	public void OnDrag()
	{
		_realtimeDragPosition = Input.mousePosition;
		_dragDeltaPosition = _realtimeDragPosition - _dragDeltaBeginPosition;
		_dragDropRectTransform.localPosition = _dragBeginPosition + new Vector3(_dragDeltaPosition.x, _dragDeltaPosition.y, 0);
	}

	/// <summary>
	/// Raises the drag end event.
	/// </summary>
	public void OnDragEnd()
	{
		StartCoroutine(OnDragEndIEnumerator());
	}

	private IEnumerator OnDragEndIEnumerator()
	{
		// Get Drag Object's DragEnd Order
		_dragEndOrder = GetDragEndOrder();

		// Get Replaced Object by DragEnd Order
		_replacedObject = DragDropContainer.GetComponent<DragDropContainer>().GetObjectByOrder(_dragEndOrder);

		// Change _replacedMember's order to _originalOrder
		_replacedObject.GetComponent<DragDropObject>().ChangeObjectOrder(_originalOrder);

		// Change DragMember (this Member) 's order to dragEndOrder
		DragDropObject.GetComponent<DragDropObject>().ChangeObjectOrder(_dragEndOrder);

		// Change GameObject of _replaceMember and DragMember
		DragDropContainer.GetComponent<DragDropContainer>().ChangeObjectByOrder(_dragEndOrder, _originalOrder);

		// Change Position of _replacedMember and DragMember
		DragDropContainer.GetComponent<DragDropContainer>().ChangeObjectPosition(DragDropObject, _replacedObject);

		// Reset DragDrop handler's position
		_dragDropRectTransform.localPosition = DragDropOriginalPosition;

		yield return new WaitForSeconds(DragDropContainer.GetComponent<DragDropContainer>().AutoMoveSpeed);

		DragDropContainer.GetComponent<GridLayoutGroup>().enabled = true;
	}
}
