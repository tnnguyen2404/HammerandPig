using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] MusicLibrary musicLibrary;
    [SerializeField] AudioSource musicSource;
    
    public static MusicManager Instance {get; private set;}

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
        
        musicSource.volume = PlayerPrefs.GetFloat("Volume", 1f);
    }

    public void PlayMusic(string trackName, float fadeTime = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossFade(musicLibrary.GetTrack(trackName), fadeTime));
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeTime)
    {
        float percent = 0f;
        float targetVolume = PlayerPrefs.GetFloat("Volume", 1f);
        while (percent < 1)
        {
            percent += Time.deltaTime * 1/ fadeTime;
            musicSource.volume = Mathf.Lerp(targetVolume, 0f, percent);
            yield return null;
        }
        
        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1/ fadeTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, percent);
            yield return null;
        }
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void ResumeMusic()
    {
        musicSource.Play();
    }

    public void SetVolume(float value)
    {
        musicSource.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
    }

    public float GetVolume()
    {
        return musicSource.volume;
    }
}
