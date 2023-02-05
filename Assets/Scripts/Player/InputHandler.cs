using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerInputActions _inputActions;
    public bool _isCrouching {  get; private set; }
    public bool _jumpPressed { get; private set; }

    PlayerManager _playerManger;
    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerInputActions();
        }

        _inputActions.PlayerMovement.Movement.performed +=HandleMovement;
        _inputActions.PlayerMovement.Jump.performed += HandleJumpInput;


        _inputActions.Enable();

    }

    private void HandleMovement(InputAction.CallbackContext action)
    {

        _playerManger.setMoveAmount(action.ReadValue<float>());
        
    }

    private void HandleJumpInput(InputAction.CallbackContext action)
    {
        _jumpPressed = true;
        _playerManger.Jump();
    }

    private void OnDisable()
    {
        _inputActions.Disable();
        _inputActions.PlayerMovement.Movement.performed -= HandleMovement;
        _inputActions.PlayerMovement.Jump.performed -= HandleJumpInput;
    }
    void Start()
    {
        _playerManger = GetComponent<PlayerManager>();
        
        _isCrouching = false;
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
