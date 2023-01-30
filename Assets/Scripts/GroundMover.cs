using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMover : MonoBehaviour
{

    MeshRenderer _meshRenderer;
    float _speed;



    private void Awake()
    {
        GameManager.OnSpeedChange += HandleSpeedChange;
        _meshRenderer= GetComponent<MeshRenderer>();
    }

    private void HandleSpeedChange(float speed)
    {
        _speed = speed;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
         
        _meshRenderer.material.mainTextureOffset += Vector2.right * _speed * Time.fixedDeltaTime / _meshRenderer.transform.localScale.x * _meshRenderer.material.mainTextureScale.x;
    }

}

