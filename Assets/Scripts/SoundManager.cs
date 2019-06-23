using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundClipGroup footsteps;
    [SerializeField] private SoundClip weaponShot;

    private List<AudioSource> audioSourcePool;

    private bool playFootsteps;
    private bool playingFootsteps;

    private void Start()
    {
        audioSourcePool = new List<AudioSource>();

        var events = GameManager.Instance.Events;

        events.AddListener<WeaponFired>((obj) => 
        {
            PlaySound(weaponShot);
        });

        events.AddListener<PlayerStartMoving>((obj) =>
        {
            playFootsteps = true;
            if(!playingFootsteps) StartCoroutine(PlayFootsteps());
        });

        events.AddListener<PlayerStopMoving>((obj) =>
        {
            playFootsteps = false;
        });
    }

    private IEnumerator PlayFootsteps()
    {
        playingFootsteps = true;
        var audioSource = PlaySound(footsteps.Clips[Random.Range(0, footsteps.Clips.Length)], footsteps.Group);
        while (audioSource.isPlaying) yield return null;
        playingFootsteps = false;
        if (playFootsteps) StartCoroutine(PlayFootsteps());
    }

    private AudioSource PlaySound(SoundClip clip)
    {
        return PlaySound(clip.Clip, clip.Group);
    }

    private AudioSource PlaySound(AudioClip clip, AudioMixerGroup group)
    {
        if(audioSourcePool.Count == 0)
        {
            audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
        }
        var last = audioSourcePool.Count - 1;
        var audioSource = audioSourcePool[last];
        audioSourcePool.RemoveAt(last);

        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = group;
        audioSource.Play();
        StartCoroutine(ReturnToPool(audioSource));
        return audioSource;
    }

    private IEnumerator ReturnToPool(AudioSource source)
    {
        while (source.isPlaying) yield return null;
        audioSourcePool.Add(source);
    }

}

[System.Serializable]
public class SoundClip
{
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioMixerGroup group;

    public AudioClip Clip => clip;
    public AudioMixerGroup Group => group;
}

[System.Serializable]
public class SoundClipGroup
{
    [SerializeField] private AudioClip[] clips;
    [SerializeField] private AudioMixerGroup group;

    public AudioClip[] Clips => clips;
    public AudioMixerGroup Group => group;

}
