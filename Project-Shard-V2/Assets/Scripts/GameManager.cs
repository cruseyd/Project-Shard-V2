using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;

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

    public void BeginGame()
    {
        SceneManager.LoadScene("CombatScene");
    }
}
