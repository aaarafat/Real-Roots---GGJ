using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    Rigidbody2D _rigidbody;
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
    float _torque;
    private Vector2 _velocity;

    float _lerpRotation;

    // Start is called before the first frame update
    void Start()
    {
        _direction = _inverted? -1 : 1;
        _rigidbody = GetComponent<Rigidbody2D>();
        _jumpVelocity = new Vector2(0, _jumpSpeed);
        _playerManager = GetComponent<PlayerManager>();
        _playerManager.Crouched += HandleCrouching;
        _playerManager.Jumped += HandleJumping ;
    }

    private void HandleJumping()
    {
        Debug.Log($"Test from {_direction}");
        Jump();
    }

    private void OnDestroy()
    {
        _playerManager.Crouched -= HandleCrouching;
        _playerManager.Jumped += HandleJumping;
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
       

    }
    void StartCrouching()
    {
    }
    void StopCrouching()
    {
    }

    public void Move(float amount)
    {
        _velocity = _rigidbody.velocity;
        _velocity.x = amount * _direction * _speed;
        _torque = amount * _rotationSpeed * -1 ;

    }

    private void FixedUpdate()
    {
        CheckGround();
        if(_jump)
        {
            _jump = false;
            Jump();
        }
        _rigidbody.velocity = _velocity;
        //_rigidbody.AddTorque(_torque);
        //_rigidbody.MoveRotation(_rigidbody.rotation + _torque*Time.fixedDeltaTime);
        transform.Rotate(new Vector3(0, 0, _torque * Time.fixedDeltaTime));
        HandleGravity(Time.fixedDeltaTime);

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

    private float GetLerpAngle(float angle)
    {
        float positive = Mathf.Sign(angle);
        float absAngle = Mathf.Abs(angle);
        float goTo = 0;
        if (absAngle > 0) goTo = 90;
        else if (absAngle > 90) goTo = 90;
        return 0;


    }
}
