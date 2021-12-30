using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DecklistItemUI : MonoBehaviour
{
    private int _quantity;
    private CardData _data;
    [SerializeField] public TextMeshProUGUI _quantityText;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _level;
    [SerializeField] private Image _background; //will be a sprite

    public CardData data { get { return _data; } }
    public int quantity { get { return _quantity; } }
    public void Initialize(CardData a_data)
    {
        _data = a_data;
        _quantity = 1;
        _quantityText.text = _quantity.ToString();
        _name.text = a_data.name;
        _level.text = a_data.level.ToString();

        _background.color = CardGameParams.GetColor(a_data.color);
    }
    public void Increment(int a_delta)
    {
        _quantity += a_delta;
        _quantity = Mathf.Clamp(_quantity, 0, 9999);
        _quantityText.text = _quantity.ToString();
    }

}
