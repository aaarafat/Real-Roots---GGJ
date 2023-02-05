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
    private bool _isDead;
    private float _deathTimer;

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
        if (_isDead) _deathTimer += Time.deltaTime;
        if(_deathTimer > 0.5f)
        {
            _deathTimer= 0;
            _isDead= false;
            Loader.ReloadCurrentScene();
        }
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
        _isDead = true;
    }

    public void Win()
    {
        OnPlayerWin?.Invoke();
    }

    public void Pause()
    {
        Time.timeScale= 0;
        IsPaused= true;
        OnPause?.Invoke();
    }
    public void Resume()
    {
        Time.timeScale = 1;
        IsPaused = false;
        OnResume?.Invoke();
    }
}
