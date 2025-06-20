using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] SoundLibrary soundLibrary;
    [SerializeField] AudioSource audioSource;
    
    public static SoundManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySound3D(AudioClip clip, Vector3 pos)
    {
        if (clip != null)
            AudioSource.PlayClipAtPoint(clip, pos);
    }

    public void PlaySound3D(string soundName, Vector3 pos)
    {
        PlaySound3D(soundLibrary.GetSound(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        audioSource.PlayOneShot(soundLibrary.GetSound(soundName));
    }
}
