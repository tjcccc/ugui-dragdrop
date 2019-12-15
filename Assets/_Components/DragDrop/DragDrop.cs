using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DragDrop : MonoBehaviour
{
    // If you want it looks like only one dragging object, check it.
    public bool isObjectInvisible;
    
    // DragDrop Group
    [HideInInspector] public GameObject dragDropContainer;
    [HideInInspector] public GameObject dragDropObject;

    // DragDrop Data
    [HideInInspector] public Vector3 dragDropOriginalPosition;

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

    private Canvas _canvas;
    private float _scaleFactor;

    private void Awake()
    {
        _dragDropRectTransform = GetComponent<RectTransform>();
        dragDropOriginalPosition = _dragDropRectTransform.anchoredPosition;
    }

    public void ConnectRelatives()
    {
        var transformParent = transform.parent;
        dragDropContainer = transformParent.parent.gameObject;
        dragDropObject = transformParent.gameObject;
        _canvas = dragDropContainer.GetComponent<DragDropContainer>().canvas;

        _objectRectSize = dragDropContainer.GetComponent<GridLayoutGroup>().cellSize;
        _objectRectTransform = dragDropObject.GetComponent<RectTransform>();
    }

    private void GetContainerStructure()
    {
        _spacingX = dragDropContainer.GetComponent<GridLayoutGroup>().spacing.x;
        _spacingY = dragDropContainer.GetComponent<GridLayoutGroup>().spacing.y;
        _column = dragDropContainer.GetComponent<DragDropContainer>().column;
        _row = dragDropContainer.GetComponent<DragDropContainer>().row;
    }

    private Vector3 GetDragEndPosition()
    {
        return transform.localPosition;
    }

    private int GetObjectOrder()
    {
        return dragDropObject.GetComponent<DragDropObject>().objectOrder;
    }

    private static int GetClosetInteger(float floatNumber, int limit)
    {
        if (floatNumber < 0)
        {
            return 0;
        }
        
        if (floatNumber > limit - 1)
        {
            return limit - 1;
        }
        
        if (floatNumber >= 0 && floatNumber - (int)floatNumber >= 0.5f)
        {
            return (int)floatNumber + 1;
        }
        
        return (int)floatNumber;
    }

    private int GetDragEndOrder()
    {
        GetContainerStructure();

        // Get Drag Object's DragEnd Position
        _dragEndPosition = GetDragEndPosition();

        // Get arrange order (row, column)
        // [(final + rectW/2) - spaceX(x-1)] / rectW = x
        var objectAnchoredPosition = _objectRectTransform.anchoredPosition;
        var rowX = ((0 - (objectAnchoredPosition.y + _dragEndPosition.y)) + (_objectRectSize.y * 0.5f) + _spacingY) / (_objectRectSize.y + _spacingY) - 1;
        var columnY = (objectAnchoredPosition.x + _dragEndPosition.x + (_objectRectSize.x * 0.5f) + _spacingX) / (_objectRectSize.x + _spacingX) - 1;

//        Debug.Log("finalX: " + (_objectRectTransform.localPosition.x + _dragEndPosition.x).ToString());

//        Debug.Log("rowX & columnY: " + rowX.ToString() + ", " + columnY.ToString());

        var orderX = GetClosetInteger(rowX, _row);
        var orderY = GetClosetInteger(columnY, _column);

//        Debug.Log("order x & y: " + orderX.ToString() + ", " + orderY.ToString());

        _dragDropAmount = dragDropContainer.GetComponent<DragDropContainer>().dragDropObjects.Length;
        var order = (_column * orderX + orderY) <= (_dragDropAmount - 1) ? (_column * orderX + orderY) : (_dragDropAmount - 1);

//        Debug.Log("order: " + order);

        return order;
    }

    /// <summary>
    /// Raises the drag begin event.
    /// </summary>
    public void OnDragBegin()
    {
        dragDropContainer.GetComponent<GridLayoutGroup>().enabled = false;

        // Save current object Order
        _originalOrder = GetObjectOrder();

        // Display in the top layer
        dragDropObject.transform.SetAsLastSibling();

        _dragBeginPosition = _dragDropRectTransform.anchoredPosition;
        _dragDeltaBeginPosition = Input.mousePosition;
        _scaleFactor = _canvas.scaleFactor;
    }

    /// <summary>
    /// Raises the drag event.
    /// </summary>
    public void OnDrag()
    {
        _realtimeDragPosition = Input.mousePosition;
        _dragDeltaPosition = (_realtimeDragPosition - _dragDeltaBeginPosition) / _scaleFactor;
        _dragDropRectTransform.anchoredPosition = _dragBeginPosition + new Vector3(_dragDeltaPosition.x, _dragDeltaPosition.y, 0);
        
        // TODO: Fix position deviation in World Space.
    }

    /// <summary>
    /// Raises the drag end event.
    /// </summary>
    public void OnDragEnd()
    {
        StartCoroutine(OnDragEndCoroutine());
    }

    private IEnumerator OnDragEndCoroutine()
    {
        // Get Drag Object's DragEnd Order
        _dragEndOrder = GetDragEndOrder();

        // Get Replaced Object by DragEnd Order
        _replacedObject = dragDropContainer.GetComponent<DragDropContainer>().GetObjectByOrder(_dragEndOrder);

        // Change _replacedObject's order to _originalOrder
        _replacedObject.GetComponent<DragDropObject>().ChangeObjectOrder(_originalOrder);

        // Change DragObject (this Object) 's order to dragEndOrder
        dragDropObject.GetComponent<DragDropObject>().ChangeObjectOrder(_dragEndOrder);

        // Change GameObject of _replaceObject and DragObject
        dragDropContainer.GetComponent<DragDropContainer>().ChangeObjectByOrder(_dragEndOrder, _originalOrder);

        // Change Position of _replacedObject and DragObject
        dragDropContainer.GetComponent<DragDropContainer>().ChangeObjectPosition(dragDropObject, _replacedObject, isObjectInvisible ? gameObject : null);

        // Reset DragDrop handler's position
        _dragDropRectTransform.anchoredPosition = dragDropOriginalPosition;

        yield return new WaitForSeconds(dragDropContainer.GetComponent<DragDropContainer>().autoMoveSpeed);

        dragDropContainer.GetComponent<GridLayoutGroup>().enabled = true;
    }
}
