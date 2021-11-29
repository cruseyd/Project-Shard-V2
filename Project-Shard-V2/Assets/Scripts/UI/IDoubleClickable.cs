using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IDoubleClickable
{
    public GameObject gameObject { get; }

    public void DoubleClick(PointerEventData eventData);
}
