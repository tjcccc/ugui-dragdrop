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

    // Drag and Drop Objects
    [HideInInspector] public GameObject[] dragDropObjects;
    [HideInInspector] public Vector3[] dragDropObjectPositions;
    [HideInInspector] public int column;
    [HideInInspector] public int row;

    public GridType type;
    public Canvas canvas;
    public float autoMoveSpeed = 0.2f;

    private RectTransform _dragDropContainerRectTransform;
    private GridLayoutGroup _dragDropContainerGridLayoutGroup;
    private int _objectAmount;

    private void Awake()
    {
        _dragDropContainerRectTransform = GetComponent<RectTransform>();
        _dragDropContainerGridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    private IEnumerator Start()
    {
        if (type != GridType.Static)
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
        dragDropObjects = new GameObject[_objectAmount];
        dragDropObjectPositions = new Vector3[_objectAmount];
    }

    /// <summary>
    /// Gets the column and row.
    /// </summary>
    private void GetColumnAndRow()
    {
        column = (int)(_dragDropContainerRectTransform.sizeDelta.x + _dragDropContainerGridLayoutGroup.spacing.x) / (int)(_dragDropContainerGridLayoutGroup.cellSize.x + _dragDropContainerGridLayoutGroup.spacing.x);
        // ReSharper disable once PossibleLossOfFraction
        row = ((float)_objectAmount / column) > (_objectAmount / column) ? (_objectAmount / column) + 1 : (_objectAmount / column);
    }

    /// <summary>
    /// Sets all objects with order.
    /// </summary>
    private void SetAllObjectsWithOrder()
    {
        for (var i = 0; i < dragDropObjects.Length; i += 1)
        {
            dragDropObjects[i] = transform.GetChild(i).gameObject;
            dragDropObjects[i].GetComponent<DragDropObject>().objectOrder = i;
        }
    }

    /// <summary>
    /// Sets all objects order.
    /// </summary>
    private void SetStaticRelatives()
    {
        if (type != GridType.Static)
        {
            return;
        }

        for (var i = 0; i < dragDropObjects.Length; i += 1)
        {
            dragDropObjects[i].GetComponentInChildren<DragDrop>().ConnectRelatives();
        }
    }

    /// <summary>
    /// Gets the object order.
    /// </summary>
    /// <returns>The object order.</returns>
    /// <param name="dragDropObject">Drag drop object.</param>
    private static int GetObjectOrder(GameObject dragDropObject)
    {
        return dragDropObject.GetComponent<DragDropObject>().objectOrder;
    }

    /// <summary>
    /// Gets the object by order.
    /// </summary>
    /// <returns>The object by order.</returns>
    /// <param name="order">Order.</param>
    public GameObject GetObjectByOrder(int order)
    {
        return dragDropObjects[order];
    }

    /// <summary>
    /// Gets the object position.
    /// </summary>
    private void GetObjectPosition()
    {
        var position = new Vector3[row, column];
        for (var i = 0; i < row; i += 1)
        {
            for (var j = 0; j < column; j += 1)
            {
                var cellSize = _dragDropContainerGridLayoutGroup.cellSize;
                var spacing = _dragDropContainerGridLayoutGroup.spacing;
                var x = cellSize.x / 2 + (cellSize.x + spacing.x) * j;
                var y = 0 - cellSize.y / 2 - (cellSize.y + spacing.y) * i;
                position[i, j] = new Vector3(x, y, 0);
            }
        }

        for (var i = 0; i < dragDropObjects.Length; i += 1)
        {
            dragDropObjectPositions[i] = position[i / column, i - (i / column) * column];
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
        // var temp = dragDropObjects[dragObjectOrder];
        // dragDropObjects[dragObjectOrder] = dragDropObjects[replacedObjectOrder];
        // dragDropObjects[replacedObjectOrder] = temp;
        (dragDropObjects[dragObjectOrder], dragDropObjects[replacedObjectOrder]) = (dragDropObjects[replacedObjectOrder], dragDropObjects[dragObjectOrder]);
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

        dragObject.transform.DOLocalMove(dragDropObjectPositions[dragObjectOrder], autoMoveSpeed);
        replacedObject.transform.DOLocalMove(dragDropObjectPositions[replacedObjectOrder], autoMoveSpeed);

        SetObjectHierarchy(dragObject, dragObjectOrder);
        SetObjectHierarchy(replacedObject, replacedObjectOrder);
    }
}
