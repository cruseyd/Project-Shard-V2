using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public enum ID
    {
        DEFAULT,
        PLAY,
        TRIBUTE,
        DECKLIST,
        CARD_POOL
    }

    [SerializeField] private ID _id;
    public ID id { get { return _id; } }
    public bool hovered { get; private set; }
    void Awake()
    {
        hovered = false;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hovered = false;
    }
}
