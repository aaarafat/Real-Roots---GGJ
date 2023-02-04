using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance;
    public static event Action<float> OnSpeedChange;
    public float _speed;
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
        OnSpeedChange?.Invoke(_speed);
    }

    private void OnValidate()
    {
        OnSpeedChange?.Invoke(_speed);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
