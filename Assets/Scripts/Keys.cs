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
    AudioClip _winAudio;

    [SerializeField]
    LayerMask _playerLayerMask;

    public bool _whiteTriggerd;
    public bool _blackTriggerd;

    float _wTriggerTimer = 0.0f;
    float _bTriggerTimer = 0.0f;

    bool _won = false;
    [SerializeField]
    float _triggerThreshold = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_won) return;
        UpdateTimers();
        if(_wTriggerTimer > _triggerThreshold && _bTriggerTimer > _triggerThreshold)
        {
            _white.enabled= false;
            _black.enabled= false;
            _won = true;
            SoundManager.Instance.PlaySound(_winAudio);
            GameManager.Instance.Win();
        }
    }

    private void UpdateTimers()
    {
        if (_whiteTriggerd)
        {
            _wTriggerTimer += Time.deltaTime;
        }
        else _wTriggerTimer = 0;

        if (_blackTriggerd)
        {
            _bTriggerTimer += Time.deltaTime;
        }
        else _bTriggerTimer = 0;
    }

    private void FixedUpdate()
    {
        _whiteTriggerd = _white.IsTouchingLayers(_playerLayerMask);
        _blackTriggerd = _black.IsTouchingLayers(_playerLayerMask);
    }
}
