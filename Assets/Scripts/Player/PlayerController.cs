using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D _rigidbody;
    CircleCollider2D _collider;


    bool _isGrounded;
    [SerializeField]
    float _jumpSpeed;
    [SerializeField]
    float _speed;
    [SerializeField]
    float _rotationSpeed;
    Vector2 _jumpVelocity;

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
    float _angularVelocity;
    private Vector2 _velocity;
    float _inAirTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        _direction = _inverted? -1 : 1;
        _rigidbody = GetComponent<Rigidbody2D>();
        _collider= GetComponent<CircleCollider2D>();
        _jumpVelocity = new Vector2(0, _jumpSpeed);
        _playerManager = GetComponent<PlayerManager>();
        _playerManager.Jumped += HandleJumping ;
    }

    private void HandleJumping()
    {
        Jump();
    }

    private void OnDestroy()
    {
        _playerManager.Jumped -= HandleJumping;
    }
    // Update is called once per frame
    void Update()
    {
        if (!_isGrounded)
        {
            _inAirTimer += Time.deltaTime;
        }
    }
    void Jump()
    {
        if (!_isGrounded) return;
        _rigidbody.velocity = _jumpVelocity *_direction;
    }
    public void Move(float amount)
    {
        _velocity = _rigidbody.velocity;
        _velocity.x = amount * _direction * _speed;
        _angularVelocity = _velocity.x /( _collider.radius *_direction) * _rotationSpeed * Mathf.PI  ;

    }

    private void FixedUpdate()
    {
        CheckGround();
        if(_jump)
        {
            _jump = false;
            Jump();
        }
        if(Mathf.Abs(_rigidbody.velocity.x) > 1)
            transform.Rotate(new Vector3(0, 0,- _angularVelocity * Time.fixedDeltaTime));
        _rigidbody.velocity = _velocity;
        HandleGravity(Time.fixedDeltaTime);

    }

    private void HandleGravity(float delta)
    {
        _rigidbody.AddForce(Vector2.down * _direction * _gravity);
    }

    private void CheckGround()
    {

        RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * 0.9f, 0, Vector2.down * _direction, 0.2f, _groundLayerMask);
        transform.SetParent(hit.transform);

            if(hit && !_isGrounded)
            {
                if(hit.collider.gameObject.tag == "Platform")
                {
                    // Expand if needed
                }
                if(_inAirTimer >= 0.25f)
                {
                    _inAirTimer= 0;
                    _playerManager.Land();
                }
            }
       
        _isGrounded = (hit) ? true : false;

    }
    public void AddForce(float force,Vector2 direction)
    {
        _rigidbody.AddForce(direction * _direction * force);
    }
}
