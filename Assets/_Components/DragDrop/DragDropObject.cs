using UnityEngine;

public class DragDropObject : MonoBehaviour
{
    [HideInInspector] public int ObjectOrder;

    /// <summary>
    /// Changes the object order.
    /// </summary>
    /// <param name="order">Order.</param>
    public void ChangeObjectOrder(int order)
    {
        ObjectOrder = order;
    }

}
