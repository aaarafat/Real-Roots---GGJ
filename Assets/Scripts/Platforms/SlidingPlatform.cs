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
    private void Awake()
    {
        _transform = transform;
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
            _playerController.AddForce(_force, Vector2.right);
    }
}
