using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;


public class PauseMenu : MonoBehaviour
{
    private static PauseMenu _instance;

    [SerializeField] private GameObject _menu;
    [SerializeField] private Scene _active;

    [SerializeField] private TMP_Dropdown _playerDeck;
    [SerializeField] private TMP_Dropdown _enemyDeck;

    private bool _open;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_open) { Close(); }
            else { Open(); }
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Close();
            DontDestroyOnLoad(this);
            _active = SceneManager.GetActiveScene();
        } else
        {
            Destroy(this.gameObject);
        }
    }
    public void LoadScene(string a_sceneName)
    {
        if (_active.name == "CombatScene" || _active.name != a_sceneName)
        {
            SetDecks();
            SceneManager.LoadScene(a_sceneName);
        }
        Close();
    }
    public void MainMenuButton()
    {
        if (_active.name == "MainMenuScene")
        {
            Close();
        } else
        {
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    public void ReturnButton()
    {
        Close();
    }

    public void Close()
    {
        _open = false;
        _menu.SetActive(false);
    }

    public void SetDecks()
    {
        string playerDeckName = _playerDeck.options[_playerDeck.value].text;
        string enemyDeckName = _enemyDeck.options[_enemyDeck.value].text;
        GameManager.SetDecklist(0, Decklist.Get(playerDeckName));
        GameManager.SetDecklist(1, Decklist.Get(enemyDeckName));
    }
    public void Open()
    {
        _open = true;
        
        List<TMP_Dropdown.OptionData> deckOptions = new List<TMP_Dropdown.OptionData>();
        foreach (Decklist list in Decklist.decks)
        {
            deckOptions.Add(new TMP_Dropdown.OptionData(list.name));
        }
        _playerDeck.ClearOptions();
        _playerDeck.AddOptions(deckOptions);
        _enemyDeck.ClearOptions();
        _enemyDeck.AddOptions(deckOptions);
        
        _menu.SetActive(true);
    }
}
