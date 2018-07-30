using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DragDropContainer : MonoBehaviour
{
	public enum GridType
	{
		Static,
		Dynamic
	}

	public GridType Type;

	// Drag and Drop Objects
	[HideInInspector] public GameObject[] DragDropObject;
	[HideInInspector] public Vector3[] DragDropObjectPosition;
	[HideInInspector] public int Column;
	[HideInInspector] public int Row;

	public float AutoMoveSpeed = 0.2f;

	private RectTransform _dragDropContainerRectTransform;
	private GridLayoutGroup _dragDropContainerGridLayoutGroup;
	private int _objectAmount;

	private void Awake()
	{
		_dragDropContainerRectTransform = this.GetComponent<RectTransform>();
		_dragDropContainerGridLayoutGroup = this.GetComponent<GridLayoutGroup>();
	}

	private IEnumerator Start()
	{
		if (Type != GridType.Static)
		{
			yield break;
		}

		yield return new WaitForFixedUpdate();
		InitializeDragDrop();
	}

	private void InitializeDragDrop()
	{
		SetObjectRelatedArrays();
		GetColumnAndRow();
		SetAllObjectsWithOrder();
		GetObjectPosition();
		SetStaticRelatives();
		InitializeContainer();
	}

	/// <summary>
	/// Initializes the container.
	/// </summary>
	private void InitializeContainer()
	{
		_dragDropContainerRectTransform.pivot = new Vector2(0, 1);
	}

	/// <summary>
	/// Sets the object related arrays.
	/// </summary>
	private void SetObjectRelatedArrays()
	{
		_objectAmount = transform.childCount;
		DragDropObject = new GameObject[_objectAmount];
		DragDropObjectPosition = new Vector3[_objectAmount];
	}

	/// <summary>
	/// Gets the column and row.
	/// </summary>
	private void GetColumnAndRow()
	{
		Column = (int)(_dragDropContainerRectTransform.sizeDelta.x + _dragDropContainerGridLayoutGroup.spacing.x) / (int)(_dragDropContainerGridLayoutGroup.cellSize.x + _dragDropContainerGridLayoutGroup.spacing.x);
		Row = ((float)_objectAmount / Column) > (_objectAmount / Column) ? (_objectAmount / Column) + 1 : (_objectAmount / Column);
	}

	/// <summary>
	/// Sets all objects with order.
	/// </summary>
	private void SetAllObjectsWithOrder()
	{
		for (var i = 0; i < DragDropObject.Length; i += 1)
		{
			DragDropObject[i] = transform.GetChild(i).gameObject;
			DragDropObject[i].GetComponent<DragDropObject>().ObjectOrder = i;
		}
	}

	/// <summary>
	/// Sets all objects order.
	/// </summary>
	private void SetStaticRelatives()
	{
		if (Type != GridType.Static)
		{
			return;
		}

		for (var i = 0; i < DragDropObject.Length; i += 1)
		{
			DragDropObject[i].GetComponentInChildren<DragDrop>().ConnectRelatives();
		}
	}

	/// <summary>
	/// Gets the object order.
	/// </summary>
	/// <returns>The object order.</returns>
	/// <param name="dragDropObject">Drag drop object.</param>
	private static int GetObjectOrder(GameObject dragDropObject)
	{
		return dragDropObject.GetComponent<DragDropObject>().ObjectOrder;
	}

	/// <summary>
	/// Gets the object by order.
	/// </summary>
	/// <returns>The object by order.</returns>
	/// <param name="order">Order.</param>
	public GameObject GetObjectByOrder(int order)
	{
		return DragDropObject[order];
	}

	/// <summary>
	/// Gets the object position.
	/// </summary>
	private void GetObjectPosition()
	{
		var position = new Vector3[Row, Column];
		for (var i = 0; i < Row; i += 1)
		{
			for (var j = 0; j < Column; j += 1)
			{
				var x = _dragDropContainerGridLayoutGroup.cellSize.x / 2 + (_dragDropContainerGridLayoutGroup.cellSize.x + _dragDropContainerGridLayoutGroup.spacing.x) * j;
				var y = 0 - _dragDropContainerGridLayoutGroup.cellSize.y / 2 - (_dragDropContainerGridLayoutGroup.cellSize.y + _dragDropContainerGridLayoutGroup.spacing.y) * i;
				position[i, j] = new Vector3(x, y, 0);
			}
		}

		for (var i = 0; i < DragDropObject.Length; i += 1)
		{
			DragDropObjectPosition[i] = position[i / Column, i - (i / Column) * Column];
		}
	}

	/// <summary>
	/// Sets the object hierarchy.
	/// </summary>
	private static void SetObjectHierarchy(GameObject dragDropObject, int order)
	{
		dragDropObject.transform.SetSiblingIndex(order);
	}

	/// <summary>
	/// Changes the object by order.
	/// </summary>
	/// <param name="dragObjectOrder">Drag object order.</param>
	/// <param name="replacedObjectOrder">Replaced object order.</param>
	public void ChangeObjectByOrder(int dragObjectOrder, int replacedObjectOrder)
	{
		var temp = DragDropObject[dragObjectOrder];
		DragDropObject[dragObjectOrder] = DragDropObject[replacedObjectOrder];
		DragDropObject[replacedObjectOrder] = temp;
	}

	/// <summary>
	/// Changes the object position.
	/// </summary>
	/// <param name="dragObject">Drag object.</param>
	/// <param name="replacedObject">Replaced object.</param>
	public void ChangeObjectPosition(GameObject dragObject, GameObject replacedObject)
	{
		var dragObjectOrder = GetObjectOrder(dragObject);
		var replacedObjectOrder = GetObjectOrder(replacedObject);

		dragObject.transform.DOLocalMove(DragDropObjectPosition[dragObjectOrder], AutoMoveSpeed);
		replacedObject.transform.DOLocalMove(DragDropObjectPosition[replacedObjectOrder], AutoMoveSpeed);

		SetObjectHierarchy(dragObject, dragObjectOrder);
		SetObjectHierarchy(replacedObject, replacedObjectOrder);
	}
}
