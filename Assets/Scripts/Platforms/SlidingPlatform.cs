using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    Transform _transform;

    PlayerController _playerController;
    [SerializeField]
    float _force;
    [SerializeField]
    bool _invertY;

    float _direction = 1;
    private void Awake()
    {
        _transform = transform;
        if (_invertY)
        {
            _direction = -1;
            _transform.Rotate(0, 180, 0);

        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _playerController = _transform.GetComponentInChildren<PlayerController>();

    }
    private void FixedUpdate()
    {
        if (_playerController != null)
            _playerController.AddForce(_force, Vector2.right * _direction);
    }
}
