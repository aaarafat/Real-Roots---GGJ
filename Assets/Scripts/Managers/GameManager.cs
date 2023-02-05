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

    public static bool IsPaused = false;
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

    public void HandlePauseAndResume()
    {
        if (IsPaused)
            Resume();
        else Pause();
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
        Debug.Log("Manager Pause");

        Time.timeScale= 0;
        IsPaused= true;
        OnPause?.Invoke();
    }
    public void Resume()
    {
        Debug.Log("Manager Resume");
        Time.timeScale = 1;
        IsPaused = false;
        OnResume?.Invoke();
    }
}
