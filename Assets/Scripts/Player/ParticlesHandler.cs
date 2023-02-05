using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesHandler : MonoBehaviour
{
    [SerializeField]
    ParticleSystem _deathParticles;
    // Start is called before the first frame update
    void Start()
    {
        _deathParticles = GetComponentInChildren<ParticleSystem>();
        GameManager.OnPlayerDeath += HandleDeath;

    }

    private void HandleDeath()
    {
        Debug.Log("Play Particles");
        _deathParticles.Play();
    }

    private void OnDestroy()
    {
        GameManager.OnPlayerDeath -= HandleDeath;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
