using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderUI : MonoBehaviour
{
    [SerializeField] private CardZoneUI _cardPool;
    [SerializeField] private FilterOptionUI _colorFilter;
    [SerializeField] private FilterOptionUI _keywordFilter;
    [SerializeField] private FilterOptionUI _levelFilter;

    private List<CardUI> _cards;
    private void Awake()
    {
        GameManager.SetMainCanvas(GetComponent<Canvas>());

        _colorFilter.toggle.onValueChanged.AddListener(FilterCards);
        _colorFilter.dropdown.onValueChanged.AddListener(FilterCards);

        _keywordFilter.toggle.onValueChanged.AddListener(FilterCards);
        _keywordFilter.dropdown.onValueChanged.AddListener(FilterCards);

        _levelFilter.toggle.onValueChanged.AddListener(FilterCards);
        _levelFilter.dropdown.onValueChanged.AddListener(FilterCards);
    }

    private void Start()
    {
        List<CardData> cards = GameManager.cardIndex.cards;
        _cards = new List<CardUI>();
        foreach (CardData data in cards)
        {
            CardUI card = CardUI.Spawn(data, _cardPool);
            card.FaceUp(true);
            _cards.Add(card);
        }
    }
    private void Update()
    {
    }
    public void FilterCards(int dummy)
    {
        FilterCards(true);
    }
    public void FilterCards(bool dummy)
    {
        if (_cards == null || _cards.Count == 0) { return; }
        foreach (CardUI card in _cards)
        {
            if (_colorFilter.toggle.isOn)
            {
                if (card.data.color != (Card.Color)_colorFilter.dropdown.value)
                {
                    card.gameObject.SetActive(false);
                    continue;
                }
            }
            if (_keywordFilter.toggle.isOn)
            {
                if (card.data.keywords.Contains((Keyword)_keywordFilter.dropdown.value))
                {
                    card.gameObject.SetActive(false);
                    continue;
                }
            }
            if (_levelFilter.toggle.isOn)
            {
                if (card.data.level != (int)_levelFilter.dropdown.value)
                {
                    card.gameObject.SetActive(false);
                    continue;
                }
            }
            card.gameObject.SetActive(true);
        }
        SortCards();
    }
    public void SortCards()
    {
        _cards.Sort((c1, c2) => c1.data.CompareTo(c2.data));
        for (int ii = 0; ii < _cards.Count; ii++)
        {
            _cards[ii].transform.SetSiblingIndex(ii);
        }
        _cardPool.Organize();
    }

    bool ProcessInput(CardGameInput a_input)
    {
        if (a_input.target is CardUI)
        {
            CardUI card = a_input.target as CardUI;
            switch (a_input.type)
            {
                case CardGameInput.Type.BEGIN_HOVER:
                    //card.Zoom(true);
                    return true;
                case CardGameInput.Type.END_HOVER:
                    //card.Zoom(false);
                    return true;
                case CardGameInput.Type.BEGIN_DRAG:
                    card.transform.SetParent(GameManager.overrideCanvas.transform);
                    card.trackingMouse = true;
                    return true;
                case CardGameInput.Type.CONTINUE_DRAG:
                    break;
                case CardGameInput.Type.END_DRAG:
                    card.transform.SetParent(_cardPool.transform);
                    card.transform.position = _cardPool.Position(card.zoneIndex);
                    card.trackingMouse = false;
                    break;

                default: break;
            }
            return false;
    }
}
