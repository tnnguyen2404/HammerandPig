using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] MusicLibrary musicLibrary;
    [SerializeField] AudioSource musicSource;
    
    public static MusicManager Instance;

    void Awake()
    {
        if (Instance == null)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void PlayMusic(string trackName, float fadeTime = 0.5f)
    {
        StartCoroutine(AnimateMusicCrossFade(musicLibrary.GetTrack(trackName), fadeTime));
    }

    IEnumerator AnimateMusicCrossFade(AudioClip nextTrack, float fadeTime)
    {
        float percent = 0f;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1/ fadeTime;
            musicSource.volume = Mathf.Lerp(1f, 0f, percent);
            yield return null;
        }
        
        musicSource.clip = nextTrack;
        musicSource.Play();

        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * 1/ fadeTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, percent);
            yield return null;
        }
    }
}
