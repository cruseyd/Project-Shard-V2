using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActorUI : MonoBehaviour, ITargetUI, IDoubleClickable
{
    private static GameObject _statusEffectDisplayPrefab;

    [SerializeField] private Image _border;
    [SerializeField] private Image _background;
    [SerializeField] private ActorParticles _particles;
    [SerializeField] private GenericDictionary<Actor.StatName, ValueDisplay> _thresholds;
    [SerializeField] private GameObject _statusEffects;
    [SerializeField] private GenericDictionary<StatusEffect.Name, ValueDisplay> _statusEffectDisplays;

    private ITargetUI.State _state;
    public GenericDictionary<CardZone.Type, CardZoneUI> zones { get; private set; }
    public GenericDictionary<Actor.StatName, ValueDisplay> _statDisplays;
    public GenericDictionary<Actor.StatName, ValueDisplay> _maxStatDisplays;

    public ITarget data { get { return actor; } }
    public Actor actor { get; private set; }
    public CardZoneUI hand { get { return zones[CardZone.Type.HAND]; } }
    public CardZoneUI deck { get { return zones[CardZone.Type.DECK]; } }
    public CardZoneUI active { get { return zones[CardZone.Type.ACTIVE]; } }
    public CardZoneUI discard { get { return zones[CardZone.Type.DISCARD]; } }
    public ITargetUI.State state
    {
        get {
            return _state;
        }
        set
        {
            _state = value;
            switch (_state)
            {
                case ITargetUI.State.SOURCE:
                    MarkSource(); break;
                case ITargetUI.State.VALID_TARGET:
                    MarkValidTarget(); break;
                case ITargetUI.State.SELECTED_TARGET:
                    MarkChosenTarget(); break;
                default:
                    ClearMarks(); break;
            }
        }
    }
    private void Awake()
    {
        _particles = GetComponent<ActorParticles>();
    }
    public void SetStat(Actor.StatName a_stat, int a_value)
    {
        if (_statDisplays.ContainsKey(a_stat))
        {
            _statDisplays[a_stat].value = a_value;
        }
        else if (_maxStatDisplays.ContainsKey(a_stat))
        {
            _maxStatDisplays[a_stat].baseValue = a_value;
        }
        else if (_thresholds.ContainsKey(a_stat))
        {
            _thresholds[a_stat].value = a_value;
        }
    }
    public void Initialize(Actor a_actor)
    {
        actor = a_actor;

        SetStat(Actor.StatName.HEALTH, CardGameParams.playerStartingHealth);
        SetStat(Actor.StatName.MAX_HEALTH, CardGameParams.playerStartingHealth);
        SetStat(Actor.StatName.FOCUS, CardGameParams.playerStartingResource);
        SetStat(Actor.StatName.MAX_FOCUS, CardGameParams.playerStartingResource);
        SetStat(Actor.StatName.INFLUENCE, CardGameParams.playerStartingInfluence);
        SetStat(Actor.StatName.TOTAL_DEFIANCE, 0);

        
        foreach (ValueDisplay display in _thresholds.Values)
        {
            display.value = 0;
        }
        
    }

    public void SetZones(GenericDictionary<CardZone.Type, CardZoneUI> a_zones)
    {
        zones = a_zones;
        foreach (CardZone.Type zoneName in a_zones.Keys)
        {
            actor.GetZone(zoneName)?.SetUI(a_zones[zoneName]);
        }
    }

    public void Refresh()
    {
        state = ITargetUI.State.DEFAULT;
        RefreshStatDisplays();
        RefreshStatusEffects();
    }

    private void RefreshStatusEffects()
    {
        List<StatusEffect.Name> statusToRemove = new List<StatusEffect.Name>();
        foreach (StatusEffect.Name s in actor.statusEffects.Keys)
        {
            AddStatusEffect(s);
        }
        foreach (StatusEffect.Name s in _statusEffectDisplays.Keys)
        {
            int stacks = actor.GetStatusEffect(s);
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
        foreach (Actor.StatName sname in _statDisplays.Keys)
        {
            _statDisplays[sname].value = actor.GetStat(sname);
        }
        foreach (Actor.StatName sname in _maxStatDisplays.Keys)
        {
            _maxStatDisplays[sname].baseValue = actor.GetStat(sname);
        }
        foreach (Actor.StatName sname in _thresholds.Keys)
        {
            _thresholds[sname].value = actor.GetStat(sname);
        }
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
        
        _particles?.SetFaceColor(CardGameParams.cardSelectedTargetMainColor, 0);
        _particles?.SetFaceColor(CardGameParams.cardSelectedTargetSecondColor, 1);
        _particles?.PlayFace(true);
        _particles?.PlayEdge(false);
        
    }
    private void MarkValidTarget()
    {
        
        _particles?.SetFaceColor(CardGameParams.cardValidTargetMainColor, 0);
        _particles?.SetFaceColor(CardGameParams.cardValidTargetSecondColor, 1);
        _particles?.PlayFace(true);
        _particles?.PlayEdge(false);
    }
    public void DoubleClick(PointerEventData eventData) { }

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
