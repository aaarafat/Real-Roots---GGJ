using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] AudioSource _effectSource;
    [SerializeField] AudioSource _MusicSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

    }
    public void PlaySound(AudioClip audio , float scale = 1)
    {
        _effectSource.PlayOneShot(audio, scale);
    }
}
