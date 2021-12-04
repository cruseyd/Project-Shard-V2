using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardGameUI : MonoBehaviour
{

    [SerializeField] private GameObject _tooltipWindow;
    [SerializeField] private GenericDictionary<CardZone.Type, CardZoneUI> _neutralZones;
    [SerializeField] private DropZone _dropZone;
    [SerializeField] private DropZone _tributeZone;
    [SerializeField] private Button _confirmButton;
    [SerializeField] private TargeterUI _targeter;
    [SerializeField] private List<Transform> playerPositions;
    [SerializeField] private List<GenericDictionary<CardZone.Type, CardZoneUI>> _zones;

    public List<ActorUI> players { get; private set; }
    public CardZoneUI preview { get { return _neutralZones[CardZone.Type.PREVIEW]; } }

    private void Awake()
    {
        EnableDropZone(false);
        Tooltip._window = _tooltipWindow;
        players = new List<ActorUI>();
    }

    public void SetPlayerUI(Actor a_actor, int a_playerNum)
    {
        Debug.Assert(a_playerNum < playerPositions.Count);
        players.Add(a_actor.Spawn(playerPositions[a_playerNum]));
        players[a_playerNum].SetZones(_zones[a_playerNum]);
    }
    public void SetConfirmButtonText(string a_text)
    {
        _confirmButton.GetComponentInChildren<TextMeshProUGUI>().text = a_text;
    }
    public void SetConfirmButtonActive(bool a_flag)
    {
        _confirmButton.gameObject.SetActive(a_flag);
    }
    public void EnableDropZone(bool a_flag)
    {
        _dropZone.gameObject.SetActive(true);
        _tributeZone.gameObject.SetActive(true);
    }
    public void EnableTargetEffects(Transform a_source)
    {
        _targeter.SetSource(a_source);
    }
    public void FreezeTargetEffects(Transform a_target)
    {
        _targeter.Freeze(a_target);
    }
    public void DisableTargetEffects()
    {
        _targeter.Disable();
    }
    public void Update()
    {
        
    }
}
