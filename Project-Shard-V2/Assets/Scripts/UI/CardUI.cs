using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler, ITargetUI, IDoubleClickable
{

    private static GameObject _statusEffectDisplayPrefab;

    [SerializeField] private GameObject _cardFront;
    [SerializeField] private GameObject _cardBack;
    [SerializeField] private GameObject _statusEffects;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _abilityText;
    [SerializeField] private TextMeshProUGUI _keywordText;
    [SerializeField] private Image _textUnderlay;
    [SerializeField] private GenericDictionary<CardStats.Name, ValueDisplay> _statDisplays;
    [SerializeField] private GenericDictionary<StatusEffect.Name, ValueDisplay> _statusEffectDisplays;
    [SerializeField] private GenericDictionary<CardStats.Name, ValueDisplay> _maxStatDisplays;
    [SerializeField] private List<ThresholdDisplay> _thresholdDisplays;
    
    [SerializeField] private CardParticles _particles;

    private bool _translating = false;
    private bool _trackingMouse = false;
    [SerializeField] private bool _faceUp = false;
    private ITargetUI.State _state;
    private CardData _data;
    public CardData data { get { return _data; } }
    public bool trackingMouse
    {
        get { return _trackingMouse; }
        set
        {
            _trackingMouse = value;
            if (!_trackingMouse)
            {
                ResetPosition();
            }
        }
    }
    public bool faceUp { get { return _faceUp; } }
    public ITarget targetData { get { return card; } }
    public Card card { get; private set; }
    public ITargetUI.State state
    {
        get
        {
            return _state;
        }
        set
        {
            if (value == ITargetUI.State.DEFAULT) { ClearMarks(); }
            if (_state == value) { return; }
            //Debug.Log("CardUI::state: " + _state + " -> " + value);
            _state = value;
            switch (_state)
            {
                case ITargetUI.State.SOURCE:
                    MarkSource(); break;
                case ITargetUI.State.SELECTED_TARGET:
                    MarkChosenTarget(); break;
                case ITargetUI.State.VALID_TARGET:
                    MarkValidTarget(); break;
                case ITargetUI.State.PLAYABLE:
                    MarkPlayable(); break;
                case ITargetUI.State.TRIBUTE:
                    MarkTribute(); break;
                case ITargetUI.State.DEFAULT:
                default:
                    ClearMarks();
                    break;
            }
        }
    }
    public int zoneIndex;
    public CardZoneUI zone { get { return GetComponentInParent<CardZoneUI>(); } }
    public void FixedUpdate()
    {
        if (trackingMouse)
        {
            Track(Input.mousePosition, 0.1f);
        }
    }
    public static CardUI Spawn(CardData a_data, CardZoneUI a_zone)
    {
        int index = a_zone.transform.childCount;
        Vector2 spawnPos = (Vector2)a_zone.transform.position + a_zone.Position(index);
        GameObject cardGO = GameObject.Instantiate(CardGameParams.GetCardPrefab(a_data.type),
            spawnPos, Quaternion.identity);
        CardUI ui = cardGO.GetComponent<CardUI>();
        ui.transform.SetParent(a_zone.transform);
        ui.Initialize(a_data);
        return ui;
    }

    public void Awake()
    {
        //state = ITargetUI.State.DEFAULT;
        ClearMarks();
    }
    public void Initialize(Card a_card)
    {
        card = a_card;
        Initialize(card.data);
    }
    public void Initialize(CardData a_data)
    {
        _data = a_data;
        _nameText.text = a_data.name;
        _keywordText.text = "";
        if (a_data.k1 != null) { _keywordText.text += a_data.k1; }
        if (a_data.k2 != null) { _keywordText.text += (" " + a_data.k2); }
        if (a_data.k3 != null) { _keywordText.text += (" " + a_data.k3); }

        _abilityText.text = "<b><color=yellow>";
        if (a_data.ka1 != null) { _abilityText.text += a_data.ka1; }
        if (a_data.ka2 != null) { _abilityText.text += (", " + a_data.ka2); }
        if (a_data.ka3 != null) { _abilityText.text += (", " + a_data.ka3); }
        if (a_data.abilityKeywords.Count > 0)
        {
            _abilityText.text += "\n";
        }
        _abilityText.text += "</color></b>";
        _abilityText.text += a_data.text;
        _abilityText.text += "</b>";
        _particles = GetComponent<CardParticles>();
        InitStats(a_data);
        _cardFront.GetComponent<Image>().sprite = CardGameParams.GetCardSprite(a_data.color);
        state = ITargetUI.State.DEFAULT;
    }
    public virtual void Refresh()
    {
        if (card == null) { return; }
        ResetPosition();
        ResetScale();
        
        RefreshUIState();
        RefreshStatDisplays();
        RefreshStatusEffectDisplays();
    }
    private void RefreshStatusEffectDisplays()
    {
        List<StatusEffect.Name> statusToRemove = new List<StatusEffect.Name>();
        foreach (StatusEffect.Name s in card.statusEffects.Keys)
        {
            AddStatusEffect(s);
        }
        foreach (StatusEffect.Name s in _statusEffectDisplays.Keys)
        {
            int stacks = card.GetStatusEffect(s);
            if (stacks <= 0) { statusToRemove.Add(s); }
            else { _statusEffectDisplays[s].value = stacks; }
        }
        foreach (StatusEffect.Name s in statusToRemove)
        {
            GameObject display = _statusEffectDisplays[s].gameObject;
            _statusEffectDisplays.Remove(s);
            Destroy(display);
        }
    }
    private void RefreshStatDisplays()
    {
        foreach (CardStats.Name sname in _statDisplays.Keys)
        {
            _statDisplays[sname].value = card.stats.Get(sname);
        }
        foreach (CardStats.Name sname in _maxStatDisplays.Keys)
        {
            _maxStatDisplays[sname].baseValue = card.stats.Get(sname);
        }
    }
    private void RefreshUIState()
    {
        state = ITargetUI.State.DEFAULT;
        if (card.humanControlled)
        {
            if (card.playable || card.activatable) { state = ITargetUI.State.PLAYABLE; }
            else if (card is UnitCard && ((UnitCard)card).canAttack) { state = ITargetUI.State.PLAYABLE; }
            else if (card.zone.type == CardZone.Type.DISCARD || card.zone.type == CardZone.Type.DECK) { state = ITargetUI.State.DEFAULT; }
        }
    }
    private void InitStats(CardData a_data)
    {
        foreach (CardStats.Name sname in CardGameParams.cardStatPairs.Keys)
        {
            if (!_statDisplays.ContainsKey(sname)) { continue; }
            ValueDisplay display = _statDisplays[sname];
            if (display == null) { continue; }

            display.value = a_data.GetStat(sname);
            CardStats.Name bottomStat = CardGameParams.cardStatPairs[sname];
            if (bottomStat == CardStats.Name.DEFAULT)
            {
                display.baseValue = a_data.GetStat(sname);
            }
            else
            {
                display.baseValue = a_data.GetStat(bottomStat);
            }
        }

        for (int ii = 0; ii < _thresholdDisplays.Count; ii++)
        {
            _thresholdDisplays[ii].gameObject.SetActive(false);
            Card.Color thresholdColor = a_data.Threshold(ii);
            if (thresholdColor != Card.Color.DEFAULT)
            {
                _thresholdDisplays[ii].gameObject.SetActive(true);
                _thresholdDisplays[ii].SetColor(thresholdColor);
            }
        }
    } 
    public void FaceUp(bool a_flag, bool a_animate = false)
    {
        _faceUp = a_flag;
        if (a_animate)
        {
            StartCoroutine(DoFlip(a_flag));
        }
        else
        {
            _cardFront.SetActive(a_flag);
            _cardBack.SetActive(!a_flag);
        }
    }
    public IEnumerator DoFlip(bool faceUp)
    {
        float duration = CardGameParams.cardAnimationRate;
        float t = 0.0f;
        float halfDur = duration / 2.0f;
        Vector2 startScale = transform.localScale;
        Vector2 midScale = new Vector2(0, startScale.y);
        while (t < 1)
        {
            t += Time.deltaTime / halfDur;
            transform.localScale = Vector2.Lerp(startScale, midScale, t);
            yield return null;
        }
        FaceUp(faceUp, false);
        t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / halfDur;
            transform.localScale = Vector2.Lerp(midScale, startScale, t);
            yield return null;
        }
    }
    public void Track(Vector3 a_target, float a_minSpeed)
    {
        Vector3 r = a_target - transform.position;
        float distance = r.magnitude;
        if (distance < 1)
        {
            transform.position = a_target;
        } else
        {
            Vector3 dr = r.normalized * distance * a_minSpeed;
            transform.position += dr;
        }
    }
    public void Translate(Vector2 targetPos, float duration = 0)
    {
        if (duration == 0) { duration = CardGameParams.cardAnimationRate; }
        _trackingMouse = false;
        StartCoroutine(DoTranslate(targetPos, false, duration));
        StartCoroutine(DoZoom(false, 1.0f, duration));
    }
    public IEnumerator DoTranslate(Vector2 targetPos, bool blocking = true, float duration = 0)
    {
        if (duration == 0)
        {
            duration = CardGameParams.cardAnimationRate;
        }
        if (blocking) { _translating = true; }
        Vector2 startPos = transform.position;
        float t = 0.0f;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector2.Lerp(startPos, targetPos, t);
            yield return null;
        }
        if (blocking) { _translating = false; }
    }
    public void ResetScale()
    {
        if (zone == null) { return; }
        Vector3 targetScale = Vector3.one * zone.scale;
        transform.localScale = targetScale;
    }
    public void Zoom(bool flag, float factor = 2.0f)
    {
        if (_translating) { return; }

        Vector2 basePosition = Vector2.zero;
        Vector2 translation = Vector2.zero;
        OverrideSorting(flag);
        if (zone)
        {
            basePosition = zone.Position(zoneIndex) + (Vector2)transform.parent.position;
            //basePosition = new Vector2(zone.Position(zoneIndex) + transform.parent.position.x, transform.parent.position.y);
        }
        else
        {
            basePosition = new Vector2(transform.parent.position.x, transform.parent.position.y);
        }
        if (flag)
        {
            //_nameText.transform.localPosition = new Vector2(0, 0);
            if (zone.alignment == CardZoneUI.Alignment.STACK || !_faceUp) { return; }
            RectTransform rect = GetComponent<RectTransform>();
            float x = transform.position.x / Screen.width;
            float y = transform.position.y / Screen.height;
            float sgn_x = 1f; float sgn_y = 1f;
            if (x > 0.5f ^ !flag) { sgn_x = -1f; }
            if (y > 0.5f ^ !flag) { sgn_y = -1f; }
            float x_damping = Mathf.Abs(2.0f * (x - 0.5f));
            float y_damping = Mathf.Abs(2.0f * (y - 0.5f));
            translation = new Vector2(
                basePosition.x + (sgn_x * rect.rect.width / 2.0f * (factor - 1.0f) * x_damping * zone.scale),
                basePosition.y + (sgn_y * rect.rect.height / 2.0f * (factor - 1.0f) * y_damping * zone.scale));
        }
        else
        {
            //_nameText.transform.localPosition = new Vector2(0, -60);
            
            translation = basePosition;
        }
        //_abilityText.gameObject.SetActive(flag);
        //_textUnderlay?.gameObject.SetActive(flag);
        StartCoroutine(DoTranslate(translation, false, 0.1f));
        StartCoroutine(DoZoom(flag, factor, 0.1f));
    }
    public IEnumerator DoZoom(bool flag, float factor = 1.5f, float duration = 0.1f)
    {
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = Vector3.one;
        if (zone != null)
        {
            targetScale *= zone.scale;
        }
        if (flag)
        {
            transform.SetAsLastSibling();
            targetScale = targetScale * factor;
        }
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / duration;
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        if (!flag)
        {
            transform.SetSiblingIndex(zoneIndex);
        }
        transform.localScale = targetScale;
    }
    public void Move(CardZoneUI cardZone, bool a_faceUp, bool a_useQueue = true)
    {
        CardGameAnimation anim = new MoveCardAnim(this, cardZone, a_faceUp);
        if (a_useQueue)
        {
            AnimationQueue.Add(anim);
        } else
        {
            AnimationQueue.PlayNow(anim);
        }
        
    }
    public void ResetPosition()
    {
        if (zone == null) { return; }
        _trackingMouse = false;
        transform.SetSiblingIndex(zoneIndex);
        RectTransform rect = zone.GetComponent<RectTransform>();
        Vector2 dest = rect.TransformPoint(zone.Position(zoneIndex));
        Translate(dest);
    }
    public void OverrideSorting(bool a_flag)
    {
        if (a_flag)
        {
            transform.SetAsLastSibling();
            transform.parent.SetAsLastSibling();
        } else
        {
            transform.SetSiblingIndex(zoneIndex);
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        CardGameInput input = new CardGameInput(CardGameInput.Type.BEGIN_HOVER, this.transform, eventData);
        GameManager.ProcessInput(input);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        CardGameInput input = new CardGameInput(CardGameInput.Type.END_HOVER, this.transform, eventData);
        GameManager.ProcessInput(input);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        ResetScale();
        _translating = true;
        CardGameInput input = new CardGameInput(CardGameInput.Type.BEGIN_DRAG, this.transform, eventData);
        GameManager.ProcessInput(input);
    }

    public void OnDrag(PointerEventData eventData)
    {

        CardGameInput input = new CardGameInput(CardGameInput.Type.CONTINUE_DRAG, this.transform, eventData);
        GameManager.ProcessInput(input);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _translating = false;
        CardGameInput input = new CardGameInput(CardGameInput.Type.END_DRAG, this.transform, eventData);
        GameManager.ProcessInput(input);
    }

    public void DoubleClick(PointerEventData eventData)
    {
        //CardGameInput input = new CardGameInput(CardGameInput.Type.DOUBLE_CLICK, this.transform, null);
        //CardGameManager.ProcessInput(input);
    }

    private void ClearMarks()
    {
        _particles?.StopAll();
    }
    private void MarkSource()
    {
        _particles?.SetEdgeGradient(CardGameParams.cardSourceGradient);
        _particles?.PlayEdge(true);
        _particles?.PlayFace(false);
    }
    private void MarkChosenTarget()
    {
        ClearMarks();
        /*
        _particles?.SetFaceColor(CardGameParams.cardSelectedTargetMainColor, 0);
        _particles?.SetFaceColor(CardGameParams.cardSelectedTargetSecondColor, 1);
        _particles?.PlayFace(true);
        _particles?.PlayEdge(false);
        */
    }
    private void MarkValidTarget()
    {
        _particles?.SetFaceColor(CardGameParams.cardValidTargetMainColor, 0);
        _particles?.SetFaceColor(CardGameParams.cardValidTargetSecondColor, 1);
        _particles?.PlayFace(true);
        _particles?.PlayEdge(false);
    }
    private void MarkPlayable()
    {
        Color rayColor = Color.white;
        switch (_data.color)
        {
            case Card.Color.BLUE:
                rayColor = new Color(0, 200, 100, 0.05f); break;
            case Card.Color.GREEN:
                rayColor = new Color(50, 200, 0, 0.05f); break;
            case Card.Color.INDIGO:
                rayColor = new Color(100, 50, 200, 0.05f); break;
            case Card.Color.RED:
                rayColor = new Color(255, 160, 0, 0.05f); break;
            case Card.Color.GOLD:
                rayColor = new Color(255, 160, 0, 0.05f); break;
            case Card.Color.VIOLET:
                rayColor = new Color(255, 160, 0, 0.05f); break;
        }
        _particles?.SetFaceColor(rayColor, 0);
        _particles?.SetFaceColor(CardGameParams.cardPlayableSecondColor, 1);
        _particles?.PlayFace(true);
        _particles?.PlayEdge(false);
    }
    private void MarkTribute()
    {
        _particles?.SetEdgeGradient(CardGameParams.cardTributeGradient);
        _particles?.PlayEdge(true);
        _particles?.PlayFace(false);
    }

    public void AddStatusEffect(StatusEffect.Name a_status)
    {
        if (_statusEffectDisplayPrefab == null)
        {
            _statusEffectDisplayPrefab = Resources.Load("Prefabs/CircleDisplay") as GameObject;
        }
        if (_statusEffectDisplays.ContainsKey(a_status)) { return; }
        GameObject statusDisplay = Instantiate(_statusEffectDisplayPrefab, _statusEffects.transform) as GameObject;
        Tooltip tooltip = statusDisplay.GetComponent<Tooltip>();
        if (tooltip != null)
        {
            tooltip.header = a_status.ToString().ToLower();
            tooltip.content = StatusEffect.Tooltip(a_status);
        }
        _statusEffectDisplays[a_status] = statusDisplay.GetComponent<ValueDisplay>();
    }
}
