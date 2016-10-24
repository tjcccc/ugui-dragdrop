using UnityEngine;
using System.Collections;

public class DragDrop_Object : MonoBehaviour
{
	public int objectOrder;

	/// <summary>
	/// Changes the object order.
	/// </summary>
	/// <param name="order">Order.</param>
	public void ChangeObjectOrder (int order)
	{
		objectOrder = order;
	}

}
