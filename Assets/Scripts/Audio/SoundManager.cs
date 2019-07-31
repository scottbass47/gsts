using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private SoundClipGroup[] groups;
    [SerializeField] private SoundClip[] singles;

    private Dictionary<Sounds, SoundAsset> soundDict;

    private List<AudioSource> audioSourcePool;
    private Dictionary<int, AudioSource> loopingSounds;
    private int nextID = 0;

    private bool playFootsteps;
    private bool playingFootsteps;

    private static SoundManager instance;

    private void Awake()
    {
        instance = this;
        soundDict = new Dictionary<Sounds, SoundAsset>();
        loopingSounds = new Dictionary<int, AudioSource>();

        foreach(var sound in groups)
        {
            soundDict.Add(sound.Sound, sound);
        }

        foreach(var sound in singles)
        {
            soundDict.Add(sound.Sound, sound);
        }
    }

    private void Start()
    {
        audioSourcePool = new List<AudioSource>();

        var events = GameManager.Instance.Events;

        //events.AddListener<WeaponFired>(this.gameObject, (obj) => 
        //{
        //    PlaySound(weaponShot);
        //});

        //events.AddListener<PlayerStartMoving>(this.gameObject, (obj) =>
        //{
        //    playFootsteps = true;
        //    if(!playingFootsteps) StartCoroutine(PlayFootsteps());
        //});

        //events.AddListener<PlayerStopMoving>(this.gameObject, (obj) =>
        //{
        //    playFootsteps = false;
        //});
    }

    public static void PlaySound(Sounds soundType)
    {
        instance.PlaySoundInternal(soundType);
    }

    private void PlaySoundInternal(Sounds soundType)
    {
        var sound = soundDict[soundType];
        PlaySound(sound.GetClip(), sound.Group);
    }

    public static int PlaySoundLooping(Sounds soundType)
    {
        return instance.PlaySoundLoopingInternal(soundType);
    }

    public static void StopSoundLooping(int soundID)
    {
        instance.StopSoundLoopingInternal(soundID);
    }


    private int PlaySoundLoopingInternal(Sounds soundType)
    {
        var sounds = soundDict[soundType];
        var audioSource = ObtainAudioSource();
        var id = nextID++;
        loopingSounds.Add(id, audioSource);
        StartCoroutine(PlayLoopRoutine(sounds, id));
        return id;
    }

    private IEnumerator PlayLoopRoutine(SoundAsset sound, int soundID)
    {
        var audioSource = loopingSounds[soundID];
        audioSource.clip = sound.GetClip();
        audioSource.outputAudioMixerGroup = sound.Group;
        audioSource.Play();

        while (audioSource.isPlaying) yield return null;

        // If the loop has been stopped, we can retire the audio source and break
        if (!loopingSounds.ContainsKey(soundID))
        {
            ReturnAudioSourceToPool(audioSource);
            yield break;
        }
        StartCoroutine(PlayLoopRoutine(sound, soundID));
    }

    private void StopSoundLoopingInternal(int soundID)
    {   
        loopingSounds.Remove(soundID);
    }

    //private IEnumerator PlayFootsteps()
    //{
    //    playingFootsteps = true;
    //    var audioSource = PlaySound(footsteps.Clips[Random.Range(0, footsteps.Clips.Length)], footsteps.Group);
    //    while (audioSource.isPlaying) yield return null;
    //    playingFootsteps = false;
    //    if (playFootsteps) StartCoroutine(PlayFootsteps());
    //}

    private AudioSource ObtainAudioSource()
    {
        if(audioSourcePool.Count == 0)
        {
            audioSourcePool.Add(gameObject.AddComponent<AudioSource>());
        }
        var last = audioSourcePool.Count - 1;
        var audioSource = audioSourcePool[last];
        audioSourcePool.RemoveAt(last);

        return audioSource;
    }

    private AudioSource PlaySound(SoundClip clip)
    {
        return PlaySound(clip.Clip, clip.Group);
    }

    private AudioSource PlaySound(AudioClip clip, AudioMixerGroup group)
    {
        var audioSource = ObtainAudioSource();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = group;
        audioSource.Play();
        StartCoroutine(ReturnToPool(audioSource));
        return audioSource;
    }

    private void ReturnAudioSourceToPool(AudioSource source)
    {
        audioSourcePool.Add(source);
    }

    private IEnumerator ReturnToPool(AudioSource source)
    {
        while (source.isPlaying) yield return null;
        ReturnAudioSourceToPool(source);
    }

}

[System.Serializable]
public abstract class SoundAsset
{
    public Sounds Sound;
    public AudioMixerGroup Group;

    public abstract AudioClip GetClip();
}

[System.Serializable]
public class SoundClip : SoundAsset
{
    public AudioClip Clip;

    public override AudioClip GetClip()
    {
        return Clip;
    }
}

[System.Serializable]
public class SoundClipGroup : SoundAsset
{
    public AudioClip[] Clips;

    public override AudioClip GetClip()
    {
        return Clips[Random.Range(0, Clips.Length)];
    }
}

public enum Sounds
{
    PlayerFootsteps,
    PlayerGunshot,
    EnemyFleshHit,
    EnemyFleshHitFatal,
}
