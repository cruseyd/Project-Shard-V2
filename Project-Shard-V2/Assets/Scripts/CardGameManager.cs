using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using System;
using System.Reflection;

public class CardGameManager : MonoBehaviour
{
    private static CardGameManager _instance;
    
    [SerializeField] private CardGameUI _gameUI;
    [SerializeField] private List<Decklist> _decks;

    private CardGame _game;
    private CardIndex _cardIndex;
    private GamePhase _phaseValue;
    private float _lastButtonPress;
    private float _lastLeftClick;
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
    public static CardIndex cardIndex { get { return _instance._cardIndex; } }
    public float buttonCooldown;
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _cardIndex = new CardIndex();
            _game = new CardGame();
            _buttonsEnabled = true;
            _lastLeftClick = 0.0f;
        } else
        {
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        _game.SetUI(_gameUI);
        // This is temporary
        for (int ii = 0; ii < _decks.Count; ii++)
        {
            _game.Player(ii).SetDeck(_decks[ii]);
        }
        _phase = GamePhase.preGame;
    }
    void Update()
    {
        if (!_buttonsEnabled && (Time.time - _lastButtonPress) > buttonCooldown) { _buttonsEnabled = true; }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GamePhase newPhase = _phase.ProcessInput(_game, new CardGameInput(CardGameInput.Type.CANCEL, null, null));
            if (newPhase != null) { _phase = newPhase; }
            else { _phase = GamePhase.idle; }
        } else if (Input.GetKeyDown(KeyCode.Space))
        {
            GamePhase newPhase = _phase.ProcessInput(_game, new CardGameInput(CardGameInput.Type.CONFIRM, null, null));
            if (newPhase != null) { _phase = newPhase; }
        }
        if (Input.GetMouseButtonDown(0))
        {
            if ( (Time.time - _lastLeftClick) < 0.5f)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                GetComponent<GraphicRaycaster>().Raycast(eventData, hits);
                foreach (RaycastResult hit in hits)
                {
                    IDoubleClickable target = hit.gameObject.GetComponent<IDoubleClickable>();
                    if (target != null)
                    {
                        target.DoubleClick(eventData);
                        GamePhase newPhase = _phase.ProcessInput(_game,
                            new CardGameInput(CardGameInput.Type.DOUBLE_CLICK, target.gameObject.transform, eventData));
                        if (newPhase != null) { _phase = newPhase; }
                    }
                }
                _lastLeftClick = 0.0f;
            } else
            {
                _lastLeftClick = Time.time;
            }
        }
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
    public static void ProcessInput(CardGameInput a_input)
    {
        GamePhase newPhase = _instance._phase.ProcessInput(_instance._game, a_input);
        if (newPhase != null)
        {
            _instance._phase = newPhase;
        }
    }
    public static void ConfirmAction(GameAction a_action)
    {
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
