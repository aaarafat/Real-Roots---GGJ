using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelHUDManager : MonoBehaviour
{
    [SerializeField]
    GameObject _passMenu;
    // Start is called before the first frame update
    void Start()
    {
        GameManager.OnPlayerWin += HandleWin;
        
    }
    private void OnDestroy()
    {
        GameManager.OnPlayerWin -= HandleWin;
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
