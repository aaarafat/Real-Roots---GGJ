using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance;
    public static event Action OnPlayerDeath;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

    }
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnValidate()
    {

    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void Die()
    {
        OnPlayerDeath?.Invoke();
        Loader.ReloadCurrentScene();
    }
}
