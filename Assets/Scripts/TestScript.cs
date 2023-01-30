using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    Rigidbody2D _rigidbody;
    private float _speed;

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
        Debug.Log("Speeeeed");
        _speed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position += Vector3.left* _speed * Time.fixedDeltaTime;
    }
}
