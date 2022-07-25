using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private CardIndex _cardIndex;
    private float _lastLeftClick = 0f;
    private Transform _draggedItemPrevTransform;
    [SerializeField] private List<Decklist> _decks;
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _overrideCanvas;

    public static Canvas overrideCanvas { get { return _instance._overrideCanvas; } }
    public static CardIndex cardIndex
    {
        get
        {
            return _instance._cardIndex;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _cardIndex = new CardIndex();
            Decklist.LoadDecks();
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this.gameObject);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CardGameInput input = new CardGameInput(CardGameInput.Type.CANCEL, null, null);
            CombatManager.ProcessInput(input);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            CardGameInput input = new CardGameInput(CardGameInput.Type.CONFIRM, null, null);
            CombatManager.ProcessInput(input);
        }
        if (Input.GetMouseButtonDown(1))
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = Input.mousePosition;
            List<RaycastResult> hits = new List<RaycastResult>();
            _mainCanvas.GetComponent<GraphicRaycaster>().Raycast(eventData, hits);
            foreach (RaycastResult hit in hits)
            {
                IRightClickable target = hit.gameObject.GetComponent<IRightClickable>();
                if (target != null)
                {
                    CardGameInput input = new CardGameInput(CardGameInput.Type.RIGHT_CLICK, target.gameObject.transform, eventData);
                    ProcessInput(input);
                }
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if ((Time.time - _lastLeftClick) < 0.5f)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.mousePosition;
                List<RaycastResult> hits = new List<RaycastResult>();
                _mainCanvas.GetComponent<GraphicRaycaster>().Raycast(eventData, hits);
                foreach (RaycastResult hit in hits)
                {
                    IDoubleClickable target = hit.gameObject.GetComponent<IDoubleClickable>();
                    if (target != null)
                    {
                        CardGameInput input = new CardGameInput(CardGameInput.Type.DOUBLE_CLICK, target.gameObject.transform, eventData);
                        ProcessInput(input);
                    }
                }
                _lastLeftClick = 0.0f;
            }
            else
            {
                _lastLeftClick = Time.time;
            }
        }
    }
    public void BeginGame()
    {
        SceneManager.LoadScene("CombatScene");
    }

    public static Decklist GetDecklist(int a_playerNum)
    {
        return _instance._decks[a_playerNum];
    }
    public static void SetDecklist(int a_playerNum, Decklist a_list)
    {
        _instance._decks[a_playerNum] = a_list;
    }
    public static void SetMainCanvas(Canvas a_canvas)
    {
        _instance._mainCanvas = a_canvas;
    }
    public static void ProcessInput(CardGameInput a_input)
    {
        if (CombatManager.ProcessInput(a_input)) { return; }
        if (DeckBuilderManager.ProcessInput(a_input)) { return;  }
        // other scene managers here
        if (a_input.target is CardUI)
        {
            CardUI card = a_input.target as CardUI;
            switch (a_input.type)
            {
                case CardGameInput.Type.BEGIN_HOVER:
                    card.Zoom(true);
                    break;
                case CardGameInput.Type.END_HOVER:
                    card.Zoom(false);
                    break;
                case CardGameInput.Type.BEGIN_DRAG:
                    _instance._draggedItemPrevTransform = card.transform.parent;
                    card.transform.SetParent(_instance._overrideCanvas.transform);
                    card.trackingMouse = true;
                    break;
                case CardGameInput.Type.CONTINUE_DRAG:
                    break;
                case CardGameInput.Type.END_DRAG:
                    card.transform.SetParent(_instance._draggedItemPrevTransform);
                    card.trackingMouse = false;
                    card.zone.Organize();
                    break;

                default: break;
            }
        }
        
    }
}
