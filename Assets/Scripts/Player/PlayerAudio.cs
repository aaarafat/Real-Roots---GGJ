using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{

    [SerializeField] AudioClip _jumpAudio;
    [SerializeField] AudioClip _landAudio;
    [SerializeField] AudioClip _deathAudio;
    PlayerManager _playerManager;
    // Start is called before the first frame update
    void Start()
    {
        _playerManager = GetComponent<PlayerManager>();
        _playerManager.Landed += PlayLanding;
        _playerManager.Jumped += PlayJumping;
        GameManager.OnPlayerDeath += PlayExplosion;
    }
    private void OnDestroy()
    {
        GameManager.OnPlayerDeath -= PlayExplosion;
        _playerManager.Landed -= PlayLanding;
        _playerManager.Jumped -= PlayJumping;
    }

    private void PlayExplosion()
    {
        SoundManager.Instance.PlaySound(_deathAudio);
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
