using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class DecklistItemUI : MonoBehaviour, IDoubleClickable, IRightClickable, ITargetUI
{
    private int _quantity;
    private CardData _data;
    [SerializeField] public TextMeshProUGUI _quantityText;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private Image _background; //will be a sprite

    public CardData data { get { return _data; } }
    public int quantity { get { return _quantity; } }

    public ITargetUI.State state
    {
        get
        {
            return ITargetUI.State.DEFAULT;
        }
        set
        {

        }
    }

    public ITarget targetData { get { return null; } }

    public void Initialize(CardData a_data)
    {
        _data = a_data;
        _quantity = 1;
        _quantityText.text = _quantity.ToString();
        _name.text = a_data.name;
        _level.text = a_data.level.ToString();
        _background.color = Color.white;
        _background.sprite = CardGameParams.GetDecklistItemSprite(a_data.color);
        //_background.color = CardGameParams.GetColor(a_data.color);
    }
    public void Increment(int a_delta)
    {
        _quantity += a_delta;
        _quantity = Mathf.Clamp(_quantity, 0, 9999);
        _quantityText.text = _quantity.ToString();
    }

    public void DoubleClick(PointerEventData eventData)
    {
    }

    public void Refresh()
    {
    }

    public void RightClick(PointerEventData eventData)
    {
    }
}
