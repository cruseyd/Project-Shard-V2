using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;
using System.Reflection;

public class CombatManager : MonoBehaviour
{
    private static CombatManager _instance;
    
    [SerializeField] private CardGameUI _gameUI;
    

    private CardGame _game;
    private GamePhase _phaseValue;
    private float _lastButtonPress;
    //private float _lastLeftClick;
    private bool _buttonsEnabled;

    public static GamePhase.Name phase
    {
        get
        {
            return _instance._phase.name;
        }
    }
    private GamePhase _phase
    {
        get
        {
            return _phaseValue;
        }
        set
        {
            Debug.Log("Phase Transition: " + _phaseValue + " -> " + value);
            _phaseValue?.Exit(_game);
            if (_phaseValue != null) { _game.events.ExitPhase(_phaseValue); }
            _phaseValue = value;
            GamePhase newPhase = _phaseValue.Enter(_game);
            if (newPhase != null)
            {
                _game.events.EnterPhase(newPhase);
                _phase = newPhase;
            }
        }
    }
    public float buttonCooldown;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _game = new CardGame();
            _buttonsEnabled = true;
            GameManager.SetMainCanvas(GetComponent<Canvas>());
           // _lastLeftClick = 0.0f;
        } else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        _game.SetUI(_gameUI);
        _game.Player(0).SetDeck(GameManager.GetDecklist(0));
        _game.Player(1).SetDeck(GameManager.GetDecklist(1));
        _phase = GamePhase.preGame;
    }
    void Update()
    {
        if (!_buttonsEnabled && (Time.time - _lastButtonPress) > buttonCooldown) { _buttonsEnabled = true; }
    }

    public void ConfirmButton()
    {
        if (!_buttonsEnabled) { return; }
        _buttonsEnabled = false;
        _lastButtonPress = Time.time;
        GamePhase newPhase = _phase.Confirm(_game);
        if (newPhase != null)
        {
            _phase = newPhase;
        }
    }
    public void PrintButton()
    {
        _game.Print();
    }
    public static bool ProcessInput(CardGameInput a_input)
    {
        if (_instance == null) { return false; }
        GamePhase newPhase = _instance._phase.ProcessInput(_instance._game, a_input);
        if (newPhase != null)
        {
            _instance._phase = newPhase;
        } else
        {
            if (a_input.type == CardGameInput.Type.CANCEL)
            {
                _instance._phase = GamePhase.idle;
            }
        }
        return true;
    }
    public static void ConfirmAction(GameAction a_action)
    {
        if (_instance == null) { return; }
        if (!a_action.IsValid(_instance._game)) { return; }
        if (a_action.actor == _instance._game.humanPlayer || (!a_action.previewForAI))
        {
            _instance._game.TakeAction(a_action);

            _instance._game.Refresh();
            a_action.Show(_instance._game);
        }
        else
        {
            _instance._phase = new PreviewPhase(a_action);
        }
    }
}
