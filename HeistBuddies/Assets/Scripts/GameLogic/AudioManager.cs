using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    [Header("DefaultMusic")]
    [SerializeField] AudioClip defaultMusic;
    [SerializeField,Range(0,1)] float pitch = 1;
    [SerializeField] bool defaultMusicLoop = true;

    [Header("VolumeSettings")]
    [SerializeField,Range(0,1)] float musicVolumeWhileEffect = 0.3f;
    [SerializeField,Range(0,1)] float musicDefaultVolume = 1f;
    [SerializeField,Range(0,1)] float effectDefaultVolume = 1f;
    [SerializeField] float musicVolumeSwitchSpeed = 5;

    List<AudioSource> soundEffect = new List<AudioSource>();
    AudioSource currentMusic;

    private void OnValidate() 
    {
        if(currentMusic)
        {
            currentMusic.volume = musicDefaultVolume;
            currentMusic.loop = defaultMusicLoop;
            currentMusic.pitch = pitch;
        }
    }
    
    private void Start()
    {

        ChangeMusic(defaultMusic);

    }

    private void Update()
    {
        float pretendedVolume = soundEffect.Count>0?musicVolumeWhileEffect:musicDefaultVolume;
        if(currentMusic&&pretendedVolume!=currentMusic.volume)
            currentMusic.volume = Mathf.Lerp(currentMusic.volume,pretendedVolume,musicVolumeSwitchSpeed*Time.deltaTime);
    }

    void CheckEndOfEffects()
    {
        for(int i = soundEffect.Count-1;i>=0;i--)
        {
            if(soundEffect[i].isPlaying == false)
            {
                soundEffect.Remove(soundEffect[i]);
            }
        }
    }
    // Start is called before the first frame update
    public void PlaySoundEffect(AudioClip effectToPlay)
    {
        if(!effectToPlay)
        { return; }

        AudioSource newEffect = gameObject.AddComponent<AudioSource>();
        newEffect.clip = effectToPlay;
        newEffect.volume = effectDefaultVolume;
        newEffect.Play();
        soundEffect.Add(newEffect);


    }
    
    public void ChangeMusic(AudioClip musicToPlay)
    {
        if(!musicToPlay) 
        { return; }
        if(!currentMusic)
            currentMusic = gameObject.AddComponent<AudioSource>();
        currentMusic.Stop();
        currentMusic.clip = musicToPlay;
        currentMusic.volume = musicDefaultVolume;
        currentMusic.loop = defaultMusicLoop;
        currentMusic.pitch = pitch;
        currentMusic.Play();
    }

    public void HalfChangeMusic(AudioClip musicToPlay)
    {
        if (!musicToPlay)
        { return; }

    }
}

