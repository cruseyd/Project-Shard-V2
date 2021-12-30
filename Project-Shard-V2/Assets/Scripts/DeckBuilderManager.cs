using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckBuilderManager : MonoBehaviour
{
    private static DeckBuilderManager _instance;

    private GameObject _decklistItemUIPrefab;

    [SerializeField] private CardZoneUI _cardPool;
    [SerializeField] private FilterOptionUI _colorFilter;
    [SerializeField] private FilterOptionUI _keywordFilter;
    [SerializeField] private FilterOptionUI _levelFilter;
    [SerializeField] private Transform _decklistPanel;

    private List<CardUI> _cards;
    private List<DecklistItemUI> _decklistItems;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        if (_decklistItemUIPrefab == null)
        {
            _decklistItemUIPrefab = Resources.Load("Prefabs/DecklistItemUI") as GameObject;
        }
        

        _colorFilter.toggle.onValueChanged.AddListener(FilterCards);
        _colorFilter.dropdown.onValueChanged.AddListener(FilterCards);

        _keywordFilter.toggle.onValueChanged.AddListener(FilterCards);
        _keywordFilter.dropdown.onValueChanged.AddListener(FilterCards);

        _levelFilter.toggle.onValueChanged.AddListener(FilterCards);
        _levelFilter.dropdown.onValueChanged.AddListener(FilterCards);

        _cards = new List<CardUI>();
        _decklistItems = new List<DecklistItemUI>();
    }

    private void Start()
    {
        List<CardData> cards = GameManager.cardIndex.cards;
        GameManager.SetMainCanvas(GetComponent<Canvas>());
        foreach (CardData data in cards)
        {
            CardUI card = CardUI.Spawn(data, _cardPool);
            card.FaceUp(true);
            _cards.Add(card);
        }
        _cardPool.Organize();
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
    public void SortDecklist()
    {
        _decklistItems.Sort((a, b) => a.data.CompareTo(b.data));
        for (int ii = 0; ii < _decklistItems.Count; ii++)
        {
            _decklistItems[ii].transform.SetSiblingIndex(ii);
        }
    }
    private int NumCardInDeck(CardData a_data)
    {
        foreach (DecklistItemUI item in _decklistItems)
        {
            if (item.data.id == a_data.id)
            {
                return item.quantity;
            }
        }
        return 0;
    }
    private void AddDecklistItem(CardData a_data)
    {
        foreach (DecklistItemUI item in _decklistItems)
        {
            if (item.data.id == a_data.id)
            {
                item.Increment(1);
                return;
            }
        }
        GameObject itemUI_go = Instantiate(_decklistItemUIPrefab, _decklistPanel) as GameObject;
        DecklistItemUI itemUI = itemUI_go.GetComponent<DecklistItemUI>();
        _decklistItems.Add(itemUI);
        itemUI.Initialize(a_data);
        SortDecklist();
    }
    public static bool ProcessInput(CardGameInput a_input)
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
                    return true;
                case CardGameInput.Type.END_DRAG:
                    card.transform.SetParent(_instance._cardPool.transform);
                    card.transform.position = (Vector2)_instance._cardPool.transform.position + _instance._cardPool.Position(card.zoneIndex);
                    card.trackingMouse = false;
                    if (a_input.Hovering(DropZone.ID.DECKLIST))
                    {
                        _instance.AddDecklistItem(card.data);
                    }
                    return true;
            }
        }
        return false;
    }
}
