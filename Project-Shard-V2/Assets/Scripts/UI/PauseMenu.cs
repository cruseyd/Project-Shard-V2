using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    private static PauseMenu _instance;

    [SerializeField] private GameObject _menu;
    [SerializeField] private Scene _active;
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

    public void Open()
    {
        _open = true;
        _menu.SetActive(true);
    }
}
