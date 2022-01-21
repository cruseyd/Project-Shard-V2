using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public interface IRightClickable
{
    public GameObject gameObject { get; }

    public void RightClick(PointerEventData eventData);
}
