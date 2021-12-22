using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CardZoneUI : MonoBehaviour
{
    public enum Alignment
    {
        SPREAD,
        STACK,
        GRID
    }
    [SerializeField] private Alignment _alignment;

    private RectTransform _rect;
    private float _scale = 1.0f;
    private int _numCardsInRow = 1;
    private bool _initialized = false;
    public Alignment alignment { get { return _alignment; } }
    public bool initialized { get { return _initialized; } }

    public float paddingLeft = 0;
    public float paddingRight = 0;
    public float paddingTop = 0;
    public float paddingBottom = 0;

    public Vector2 spacing = Vector2.zero;
    public bool fillHorizontal = true;
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
        if (alignment == Alignment.GRID)
        {
            _rect.pivot = new Vector2(0, 1);
            _scale = 1;
            float zoneWidth = _rect.rect.width;
            float widthUsed = paddingLeft + paddingRight + CardGameParams.cardWidth;
            _numCardsInRow = 1;
            while (widthUsed < zoneWidth)
            {
                _numCardsInRow++;
                widthUsed += CardGameParams.cardWidth + spacing[0];
            }
            _numCardsInRow--;
            widthUsed -= (CardGameParams.cardWidth + spacing[0]);
            if (fillHorizontal)
            {
                float widthDelta = zoneWidth - widthUsed;
                spacing[0] += (widthDelta / (_numCardsInRow - 1));
            }
            Debug.Log("Rect width: " + zoneWidth);
            Debug.Log("Row width:" + _numCardsInRow);
        }
        _initialized = true;
    }

    public Vector2 Position(int a_zoneIndex)
    {
        if (alignment == Alignment.SPREAD)
        {
            int numCards = transform.GetComponentsInChildren<CardUI>().Length;
            //int numCards = transform.childCount;
            float width = _rect.rect.width;
            float spacing = width / (1.0f * numCards);
            float xpos = -width / 2.0f + spacing / 2.0f;
            return new Vector2(xpos + a_zoneIndex * spacing, 0);
        } 
        else if (alignment == Alignment.GRID)
        {
            int rowIndex = a_zoneIndex % _numCardsInRow;
            int colIndex = a_zoneIndex / _numCardsInRow;
            Vector2 position = new Vector2(paddingLeft, -paddingTop);
            position += new Vector2(rowIndex * (spacing[0] + CardGameParams.cardWidth) + CardGameParams.cardWidth/2,
                                   -colIndex * (spacing[1] + CardGameParams.cardHeight) - CardGameParams.cardHeight/2);
            return position;
        } else 
        {
            return Vector2.zero;
        }
    }
    public void Organize()
    {
        StartCoroutine(DoOrganize());
    }

    public IEnumerator DoOrganize()
    {
        CardUI[] cards = GetComponentsInChildren<CardUI>();
        if (alignment == Alignment.GRID)
        {
            int numRows = cards.Length / _numCardsInRow;
            if (cards.Length % _numCardsInRow != 0) { numRows++; }
            float height = paddingTop + paddingBottom + numRows * CardGameParams.cardHeight + (numRows - 1) * spacing[1];
            Debug.Log("setting rect.height: " + height);
            _rect.sizeDelta = new Vector2(_rect.rect.width, height);
            _rect.rect.Set(_rect.rect.x, _rect.rect.y, _rect.rect.width, height);
        }
        for (int ii = 0; ii < cards.Length; ii++)
        {
            cards[ii].zoneIndex = ii;
        }
        foreach (CardUI card in cards)
        {
            Vector2 dest = _rect.TransformPoint(0, 0, 0);
            dest = _rect.TransformPoint(Position(card.zoneIndex));
            card.Translate(dest);
            card.transform.SetSiblingIndex(card.zoneIndex);
        }
        yield return null;
    }
}
