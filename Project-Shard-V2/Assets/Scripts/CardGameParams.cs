﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardGameParams : MonoBehaviour
{
    private static CardGameParams _instance;


    [SerializeField] private float _cardAnimationRate;
    [SerializeField] private float _cardHeight;
    [SerializeField] private GenericDictionary<Card.StatName, Card.StatName> _cardStatPairs;
    [SerializeField] private GenericDictionary<Card.Color, Color> _cardColors;
    [SerializeField] private GenericDictionary<Card.Color, Sprite> _cardSprites;
    [SerializeField] private GenericDictionary<Card.Type, GameObject> _cardPrefabs;
    [SerializeField] private GenericDictionary<Card.Color, Color> _thresholdColors_0;
    [SerializeField] private GenericDictionary<Card.Color, Color> _thresholdColors_1;
    [SerializeField] private int _playerStartingInfluence;
    [SerializeField] private int _playerStartingHealth;
    [SerializeField] private int _playerStartingResource;
    [SerializeField] private int _playerHandSize;

    [SerializeField] private Gradient _cardSourceGradient;
    [SerializeField] private Gradient _cardTributeGradient;
    [SerializeField] private Color _cardPlayableMainColor;
    [SerializeField] private Color _cardPlayableSecondColor;
    [SerializeField] private Color _cardValidTargetMainColor;
    [SerializeField] private Color _cardValidTargetSecondColor;
    [SerializeField] private Color _cardSelectedTargetMainColor;
    [SerializeField] private Color _cardSelectedTargetSecondColor;

    public static int playerStartingInfluence { get { return _instance._playerStartingInfluence; } }
    public static int playerHandSize { get { return _instance._playerHandSize; } }
    public static int playerStartingHealth { get { return _instance._playerStartingHealth; } }
    public static int playerStartingResource { get { return _instance._playerStartingResource; } }
    public static float cardAnimationRate { get { return _instance._cardAnimationRate; } }
    public static float cardHeight { get { return _instance._cardHeight; } }
    public static GenericDictionary<Card.StatName, Card.StatName> cardStatPairs { get { return _instance._cardStatPairs; } }
    
    public static Gradient cardSourceGradient { get { return _instance._cardSourceGradient; } }
    public static Gradient cardTributeGradient { get { return _instance._cardTributeGradient; } }
    public static Color cardPlayableMainColor { get { return _instance._cardPlayableMainColor; } }
    public static Color cardPlayableSecondColor { get { return _instance._cardPlayableSecondColor; } }
    public static Color cardValidTargetMainColor { get { return _instance._cardValidTargetMainColor; } }
    public static Color cardValidTargetSecondColor { get { return _instance._cardValidTargetSecondColor; } }
    public static Color cardSelectedTargetMainColor { get { return _instance._cardSelectedTargetMainColor; } }
    public static Color cardSelectedTargetSecondColor { get { return _instance._cardSelectedTargetSecondColor; } }

    public static Color ThresholdColorBase(Card.Color a_color) { return _instance._thresholdColors_0[a_color]; }
    public static Color ThresholdColorParticle(Card.Color a_color) { return _instance._thresholdColors_1[a_color]; }
    public static Color GetColor(Card.Color a_color) { return _instance._cardColors[a_color]; }
    public static GameObject GetCardPrefab(Card.Type a_type) { return _instance._cardPrefabs[a_type]; }
    public static Sprite GetCardSprite(Card.Color a_color) { return _instance._cardSprites[a_color]; }
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        } else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}