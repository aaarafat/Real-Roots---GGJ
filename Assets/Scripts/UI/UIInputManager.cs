using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UIInputManager : MonoBehaviour
{
    PlayerInputActions _inputActions;

    private void OnEnable()
    {
        if (_inputActions == null)
        {
            _inputActions = new PlayerInputActions();
        }

        _inputActions.Menu.Pause.performed += HandleGamePause;

        _inputActions.Enable();

    }
    private void HandleGamePause(InputAction.CallbackContext action)
    {
        GameManager.Instance.HandlePauseAndResume();
    }


    private void OnDisable()
    {
        _inputActions.Menu.Pause.performed -= HandleGamePause;
        _inputActions.Disable();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
