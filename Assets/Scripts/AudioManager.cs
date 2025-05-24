using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Mixer")]
    public AudioMixer mainMixer;
    public AudioMixerGroup sfxGroup;
    public AudioMixerGroup uiGroup;

    [Header("Clips")]
    public AudioClip sliceClip;
    public AudioClip perfectClip;
    public AudioClip failClip;
    public AudioClip uiClickClip;

    private AudioSource sfxSource;
    private AudioSource uiSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.outputAudioMixerGroup = sfxGroup;

        uiSource = gameObject.AddComponent<AudioSource>();
        uiSource.outputAudioMixerGroup = uiGroup;
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    public void PlayUISound()
    {
        uiSource.PlayOneShot(uiClickClip);
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
}
