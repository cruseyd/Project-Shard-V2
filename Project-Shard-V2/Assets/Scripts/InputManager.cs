using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private InputManager _instance;

    [SerializeField] private CardGameUI _gameUI;
    [SerializeField]

    private CardGame _game;

    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this.gameObject);
        }
    }
}
