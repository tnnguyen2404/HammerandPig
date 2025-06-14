using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct MusicTrack
{
    public string trackName;
    public AudioClip audioClip;
}

public class MusicLibrary : MonoBehaviour
{
    public MusicTrack[] tracks;

    public AudioClip GetTrack(string trackName)
    {
        foreach (var track in tracks)
        {
            if (track.trackName == trackName)
                return track.audioClip;
        }
        return null;
    }
}
