using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    [SerializeField]
    Collider2D _white;
    [SerializeField]
    Collider2D _black;

    [SerializeField]
    LayerMask _playerLayerMask;

    public bool _whiteTriggerd;
    public bool _blackTriggerd;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _whiteTriggerd = _white.IsTouchingLayers(_playerLayerMask);
        _blackTriggerd = _black.IsTouchingLayers(_playerLayerMask);
    }
}
