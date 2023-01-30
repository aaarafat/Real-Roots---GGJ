using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D _rigidbody;
    [SerializeField]
    float _jumpSpeed;
    Vector2 _jumpVelocity;
    [SerializeField]
    bool _isGrounded;
    bool _jump;
    bool _fall;
    PlayerManager _playerManager;
    [Header("Gravity")]
    [SerializeField]
    bool _inverted;
    [SerializeField]
    float _gravity;
    [SerializeField]
    LayerMask _groundLayerMask;
    float _direction;
    // Start is called before the first frame update
    void Start()
    {
        _direction = _inverted? -1 : 1;
        _rigidbody = GetComponent<Rigidbody2D>();
        _jumpVelocity = new Vector2(0, _jumpSpeed);
        _playerManager = GetComponent<PlayerManager>();
        PlayerManager.Crouched += HandleCrouching;
        PlayerManager.Jumped += () => _jump = true; ;
    }

    private void OnDestroy()
    {
        PlayerManager.Crouched -= HandleCrouching;
    }
    // Update is called once per frame
    void Update()
    {

    }

    void Jump()
    {
        if (!_isGrounded) return;
        _rigidbody.velocity = _jumpVelocity *_direction;
    }
    public void HandleCrouching(bool crouch)
    {
        if (_isGrounded)
        {
            if (crouch) StartCrouching();
            else StopCrouching();
        }
        else
        {
            _fall = true;
        }

    }
    void StartCrouching()
    {
    }
    void StopCrouching()
    {
    }

    private void FixedUpdate()
    {
        HandleGravity(Time.fixedDeltaTime);
      CheckGround();
      if(_jump)
        {
            _jump = false;
            Jump();
        }

    }

    private void HandleGravity(float delta)
    {
        _rigidbody.AddForce(Vector2.down * _direction * _gravity);
    }

    private void CheckGround()
    {

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one*0.9f, 0, Vector2.down * _direction, 0.2f, _groundLayerMask);
        _isGrounded = (hit) ? true : false;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
         //if ( _groundLayerMask.value >> collision.gameObject.layer == 1) { _isGrounded= true; }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //if (_groundLayerMask.value >> collision.gameObject.layer == 1) { _isGrounded = false; }
    }
}
