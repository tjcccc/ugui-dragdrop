using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class DragDrop_Container : MonoBehaviour
{
    // Drag and Drop Objects
    [HideInInspector]
    public GameObject[] dragDropObject;
//    [HideInInspector]
    public Vector3[] dragDropObjectPosition;
    [HideInInspector]
	public int column;
    [HideInInspector]
	public int row;

    public float autoMoveSpeed = 0.2f;

	private RectTransform _dragDropContainerRectTransform;
	private GridLayoutGroup _dragDropContainerGridLayoutGroup;
    private int _objectAmount;

    void Awake ()
	{
        _objectAmount = transform.childCount;
        dragDropObject = new GameObject[_objectAmount];
        dragDropObjectPosition = new Vector3[_objectAmount];

        _dragDropContainerRectTransform = GetComponent<RectTransform>();
        _dragDropContainerGridLayoutGroup = GetComponent<GridLayoutGroup>();

		column = (int)(_dragDropContainerRectTransform.sizeDelta.x + _dragDropContainerGridLayoutGroup.spacing.x) / (int)(_dragDropContainerGridLayoutGroup.cellSize.x + _dragDropContainerGridLayoutGroup.spacing.x);
		row = ((float)_objectAmount / column) > (_objectAmount / column) ? (_objectAmount / column) + 1 : (_objectAmount / column);
    }

	IEnumerator Start ()
	{
		yield return new WaitForFixedUpdate ();
		GetAllObjects ();
        GetObjectPosition ();
		SetAllObjectsOrder ();
		InitializeContainer ();
	}

	/// <summary>
	/// Initializes the container.
	/// </summary>
	void InitializeContainer ()
	{
//		float containerX = - _dragDropContainerRectTransform.sizeDelta.x / 2;
//		float containerY = _dragDropContainerRectTransform.sizeDelta.y / 2;

		_dragDropContainerRectTransform.anchorMin = new Vector2 (0, 1);
		_dragDropContainerRectTransform.anchorMax = new Vector2 (0, 1);
		_dragDropContainerRectTransform.pivot = new Vector2 (0, 1);
//		_dragDropContainerRectTransform.localPosition = new Vector3 (containerX, containerY, 0);

		_dragDropContainerGridLayoutGroup.enabled = false;
	}

	/// <summary>
	/// Gets all objects.
	/// </summary>
	void GetAllObjects ()
	{
		for (int i = 0; i < dragDropObject.Length; i += 1)
		{
			dragDropObject [i] = transform.GetChild (i).gameObject;
		}
	}

	/// <summary>
	/// Sets all objects order.
	/// </summary>
	void SetAllObjectsOrder ()
	{
		for (int i = 0; i < dragDropObject.Length; i += 1)
		{
			dragDropObject [i].GetComponent<DragDrop_Object> ().objectOrder = i;
		}
	}

	/// <summary>
	/// Gets the object order.
	/// </summary>
	/// <returns>The object order.</returns>
	/// <param name="dragDropObject">Drag drop object.</param>
	int GetObjectOrder (GameObject dragDropObject)
	{
		return dragDropObject.GetComponent<DragDrop_Object> ().objectOrder;
	}

	/// <summary>
	/// Gets the object by order.
	/// </summary>
	/// <returns>The object by order.</returns>
	/// <param name="order">Order.</param>
	public GameObject GetObjectByOrder (int order)
	{
		return dragDropObject [order];
	}

	/// <summary>
	/// Gets the object position.
	/// </summary>
	public void GetObjectPosition ()
	{
		for (int i = 0; i < dragDropObject.Length; i += 1)
		{
			dragDropObjectPosition [i] = dragDropObject [i].GetComponent<RectTransform> ().localPosition;
		}
	}

	/// <summary>
	/// Sets the object hierarchy.
	/// </summary>
	void SetObjectHierarchy (GameObject dragDropObject, int order)
	{
		dragDropObject.transform.SetSiblingIndex (order);
	}

	/// <summary>
	/// Changes the object by order.
	/// </summary>
	/// <param name="dragObjectOrder">Drag object order.</param>
	/// <param name="replacedObjectOrder">Replaced object order.</param>
	public void ChangeObjectByOrder (int dragObjectOrder, int replacedObjectOrder)
	{
		GameObject temp = dragDropObject [dragObjectOrder];
		dragDropObject [dragObjectOrder] = dragDropObject [replacedObjectOrder];
		dragDropObject [replacedObjectOrder] = temp;
	}

	/// <summary>
	/// Changes the object position.
	/// </summary>
	/// <param name="dragObject">Drag object.</param>
	/// <param name="replacedObject">Replaced object.</param>
	public void ChangeObjectPosition (GameObject dragObject, GameObject replacedObject)
	{
		int dragObjectOrder = GetObjectOrder (dragObject);
		int replacedObjectOrder = GetObjectOrder (replacedObject);

		dragObject.transform.DOLocalMove (dragDropObjectPosition [dragObjectOrder], autoMoveSpeed);
		for (int i = 0; i < dragObject.transform.childCount; i += 1)
		{
			dragObject.transform.GetChild (i).DOLocalMove (Vector3.zero, autoMoveSpeed);
		}

		replacedObject.transform.DOLocalMove (dragDropObjectPosition [replacedObjectOrder], autoMoveSpeed);
		for (int i = 0; i < replacedObject.transform.childCount; i += 1)
		{
			replacedObject.transform.GetChild (i).DOLocalMove (Vector3.zero, autoMoveSpeed);
		}

		SetObjectHierarchy (dragObject, dragObjectOrder);
		SetObjectHierarchy (replacedObject, replacedObjectOrder);
	}
}
