using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    InputHandler _inputHandler;
    PlayerController _playerController;
    public event Action<bool> Crouched;
    public event Action Jumped;
    public event Action<PlayerController> LandedOnPlatform;
    public event Action Landed;
    public bool IsDead = false;
    public bool IsGrounded;

    float _moveAmount;
    // Start is called before the first frame update
    void Start()
    {
        IsDead= false;
        _inputHandler= GetComponent<InputHandler>();
        _playerController= GetComponent<PlayerController>();
        GameManager.OnPlayerDeath += HandleDeath;
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerDeath -= HandleDeath;
    }

    private void HandleDeath()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        _moveAmount= 0;
        IsDead = true;
        _playerController.Die();
    }

    // Update is called once per frame
    void Update()
    {
        _playerController.Move(_moveAmount);
    }
    public void Crouch(bool crouch)
    {
        Crouched?.Invoke(crouch);
    }
    public void Jump()
    {
        if(IsDead) return;
        if(!IsGrounded) return;
        Jumped?.Invoke();
    }

    public void Land()
    {
        Landed?.Invoke();
    }
    public void setMoveAmount(float moveAmount)
    {
        if (IsDead) return;
        _moveAmount = moveAmount;
    }
}
