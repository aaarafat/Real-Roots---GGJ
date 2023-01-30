using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler _inputHandler;
    PlayerController _playerController;
    public static event Action<bool> Crouched;
    public static event Action Jumped;
    // Start is called before the first frame update
    void Start()
    {
        _inputHandler= GetComponent<InputHandler>();
        _playerController= GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Crouch(bool crouch)
    {
        Crouched?.Invoke(crouch);
    }
    public void Jump()
    {
        Jumped?.Invoke();
    }
}
