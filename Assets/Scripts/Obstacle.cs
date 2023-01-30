using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    private float _speed;
    bool _isActive = false;

    // Start is called before the first frame update
    private void Awake()
    {
        GameManager.OnSpeedChange += HandleSpeedChange;
        _rigidbody = GetComponent<Rigidbody2D>();
    }
    void Start()
    {

    }

    private void HandleSpeedChange(float speed)
    {
        _speed = speed;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (!_isActive) return;
        transform.position += Vector3.left * _speed * Time.fixedDeltaTime;
    }
}
