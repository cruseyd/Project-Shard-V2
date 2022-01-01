using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    private static TooltipManager _instance;

    [SerializeField] private TooltipWindow _window;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        } else
        {
            Destroy(this.gameObject);
        }
    }

    void Start()
    {
        Hide();
    }

    public static void Show(Tooltip a_tooltip, Vector2 a_position)
    {
        _instance._window.SetText(a_tooltip.content, a_tooltip.header);
        _instance._window.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        _instance._window.gameObject.SetActive(false);
    }
}
