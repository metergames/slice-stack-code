using DG.Tweening;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer mainMixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup uiGroup;
    public AudioMixerGroup musicGroup;

    [Header("Clips")]
    public AudioClip sliceClip;
    public AudioClip perfectClip;
    public AudioClip failClip;
    public AudioClip uiClickClip;
    public AudioClip musicClip;

    private AudioSource sfxSource;
    private AudioSource uiSource;
    private AudioSource musicSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;

        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.outputAudioMixerGroup = uiGroup;

        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.clip = musicClip;
        musicSource.loop = true;
        musicSource.playOnAwake = false;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUISound()
    {
        uiSource.PlayOneShot(uiClickClip);
    }

    public void PlayMusic(float fadeInTime = 1f)
    {
        musicSource.Play();

    }

    // Toggle logic
    public void SetSFXEnabled(bool enabled)
    {
        mainMixer.SetFloat("SFXVolume", enabled ? 0f : -80f); // 0 dB = on, -80 = effectively off
    }

    public void SetUIEnabled(bool enabled)
    {
        mainMixer.SetFloat("UIVolume", enabled ? 0f : -80f);
    }

    private void FadeMixerGroup
}
