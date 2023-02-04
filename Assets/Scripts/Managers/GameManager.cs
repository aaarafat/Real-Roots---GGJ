using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance;
    public static event Action OnPlayerDeath;
    public static event Action OnPlayerWin;
    public static event Action OnPause;
    public static event Action OnResume;
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

    public void Win()
    {
        OnPlayerWin?.Invoke();
    }

    public void Pause()
    {
        OnPause?.Invoke();
    }
    public void Resume()
    {
        OnResume?.Invoke();
    }
}
