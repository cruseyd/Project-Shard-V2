using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

    private CardIndex _cardIndex;
    private float _lastLeftClick = 0f;
    private Transform _draggedItemPrevTransform;
    [SerializeField] private List<Decklist> _decks;
    [SerializeField] private Canvas _mainCanvas;
    [SerializeField] private Canvas _overrideCanvas;
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
        if (Input.GetMouseButtonDown(0))
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
                        CombatManager.ProcessInput(input);
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

    public static void ProcessInput(CardGameInput a_input)
    {
        if (CombatManager.ProcessInput(a_input)) { return; }
        // other scene managers here
        if (a_input.target is CardUI)
        {
            CardUI card = a_input.target as CardUI;
            switch (a_input.type)
            {
                case CardGameInput.Type.BEGIN_HOVER:
                    //card.Zoom(true);
                    break;
                case CardGameInput.Type.END_HOVER:
                    //card.Zoom(false);
                    break;
                case CardGameInput.Type.BEGIN_DRAG:
                case CardGameInput.Type.CONTINUE_DRAG:
                    card.trackingMouse = true;
                    _instance._draggedItemPrevTransform = card.transform;
                    card.transform.SetParent(_instance._overrideCanvas.transform);
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