using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{

    [Header("DefaultMusic")]
    [SerializeField] AudioClip defaultMusic;
    [SerializeField,Range(0,1)] float pitch = 1;
    [SerializeField] bool defaultMusicLoop = true;
    [SerializeField] float timerUntilMusicVolumeNormalize;

    [Header("DefaultAmbientSound")]
    [SerializeField] AudioClip defaultAmbient;
    [SerializeField] bool ambientLoop = true;

    [Header("VolumeSettings")]
    [SerializeField,Range(0,1)] float musicVolumeWhileEffect = 0.3f;
    [SerializeField,Range(0,1)] float musicDefaultVolume = 1f;
    [SerializeField,Range(0,1)] float ambientDefaultVolume = 1f;
    [SerializeField,Range(0,1)] float effectDefaultVolume = 1f;
    [SerializeField] float musicVolumeSwitchSpeed = 5;
    float timer;
    float pretendedVolume;
    List<AudioSource> soundEffect = new List<AudioSource>();
    AudioSource currentMusic;
    AudioSource currentAmbient;

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
        ChangeAmbient(defaultAmbient);
        pretendedVolume = musicDefaultVolume;
    }

    private void Update()
    {
        if(timer>0)
        {
            timer-= Time.deltaTime;
        }
        else
        {
            pretendedVolume = musicDefaultVolume;
        }
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

        timer = timerUntilMusicVolumeNormalize;
        pretendedVolume = musicVolumeWhileEffect;
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
        public void ChangeAmbient(AudioClip musicToPlay)
    {
        if(!musicToPlay) 
        { return; }
        if(!currentAmbient)
            currentAmbient = gameObject.AddComponent<AudioSource>();
        currentAmbient.Stop();
        currentAmbient.clip = musicToPlay;
        currentAmbient.volume = ambientDefaultVolume;
        currentAmbient.loop = ambientLoop;
        currentAmbient.Play();
    }

}

