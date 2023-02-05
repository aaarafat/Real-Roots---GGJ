using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHUDManager : MonoBehaviour
{
    [SerializeField]
    GameObject _passMenu;
    [SerializeField]
    GameObject _pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPlayerWin += HandleWin;
        GameManager.OnPause += HandlePause;
        GameManager.OnResume += HandleResume;
        
    }

    private void HandleResume()
    {
        Debug.Log("Menu Resume");

        _pauseMenu.SetActive(false);
    }

    private void HandlePause()
    {
        Debug.Log("Menu Pause");

        _pauseMenu.SetActive(true);
    }

    private void OnDestroy()
    {
        GameManager.Instance.Resume();
        GameManager.OnPlayerWin -= HandleWin;
        GameManager.OnPause -= HandlePause;
        GameManager.OnResume -= HandleResume;
    }
    private void HandleWin()
    {
        _passMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
