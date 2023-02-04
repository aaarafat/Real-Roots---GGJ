using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{

    [SerializeField] AudioClip _jumpAudio;
    [SerializeField] AudioClip _landAudio;
    PlayerManager _playerManager;
    // Start is called before the first frame update
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerManager.Landed += PlayLanding;
        _playerManager.Jumped += PlayJumping;
    }

    private void OnDestroy()
    {
        _playerManager.Landed -= PlayLanding;
        _playerManager.Jumped -= PlayJumping;
    }
    private void PlayJumping()
    {
        SoundManager.Instance.PlaySound(_jumpAudio);
    }

    private void PlayLanding()
    {
        SoundManager.Instance.PlaySound(_landAudio);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
