using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CardZoneUI : MonoBehaviour
{
    public enum Alignment
    {
        SPREAD,
        STACK
    }
    [SerializeField] private Alignment _alignment;

    private RectTransform _rect;
    private float _scale = 1.0f;
    public Alignment alignment { get { return _alignment; } }

    public float scale
    {
        get
        {
            return _scale;
        }
    }

    void Awake()
    {
        _rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        _scale = _rect.rect.height / CardGameParams.cardHeight;
    }

    public float Position(int a_zoneIndex)
    {
        if (alignment == Alignment.SPREAD)
        {
            int numCards = transform.GetComponentsInChildren<CardUI>().Length;
            //int numCards = transform.childCount;
            float width = _rect.rect.width;
            float spacing = width / (1.0f * numCards);
            float xpos = -width / 2.0f + spacing / 2.0f;
            return xpos + a_zoneIndex * spacing;
        } 
        else
        {
            return 0;
        }
    }
    public void Organize()
    {
        StartCoroutine(DoOrganize());
    }

    public IEnumerator DoOrganize()
    {
        CardUI[] cards = GetComponentsInChildren<CardUI>();
        for (int ii = 0; ii < cards.Length; ii++)
        {
            cards[ii].zoneIndex = ii;
        }
        foreach (CardUI card in cards)
        {
            Vector2 dest = _rect.TransformPoint(0, 0, 0);
            dest = _rect.TransformPoint(Position(card.zoneIndex), 0, 0);
            card.Translate(dest);
            card.transform.SetSiblingIndex(card.zoneIndex);
        }
        yield return null;
    }
}
