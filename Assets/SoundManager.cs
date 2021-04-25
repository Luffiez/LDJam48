using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource sfxSource;
    public AudioSource drillSource;

    public List<AudioClip> clips = new List<AudioClip>();


    public void StartDrill()
    {
        drillSource.Play();
    }

    public void StopDrill()
    {
        drillSource.Stop();
    }

    public void PlaySfx(string sfxName, float volumeScale)
    {
        sfxSource.PlayOneShot(clips.Find(clip => clip.name == sfxName),volumeScale);
    }
}
