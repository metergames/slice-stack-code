using DG.Tweening;
using System.Collections;
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

    private void Start()
    {
        string savedMusicID = PlayerPrefs.GetString("SelectedMusicID", "");
        if (!string.IsNullOrEmpty(savedMusicID))
        {
            // Find the ShopItem with that ID
            ShopItem savedItem = ShopManager.Instance.allItems.Find(i => i.ID == savedMusicID);
            if (savedItem != null && savedItem.AudioClip != null)
            {
                musicClip = savedItem.AudioClip;
                musicSource.clip = musicClip;
            }
        }
    }

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
        FadeMixerGroup("MusicVolume", 0f, fadeInTime);
    }

    public void StopMusic(float fadeOutTime = 1f)
    {
        FadeMixerGroup("MusicVolume", -80f, fadeOutTime, () => musicSource.Stop());
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

    public void SetMusicEnabled(bool enabled)
    {
        mainMixer.SetFloat("MusicVolume", enabled ? 0f : -80f);

        if (!musicSource.isPlaying && enabled)
            PlayMusic();
    }

    private void FadeMixerGroup(string exposedParam, float targetVolumeDb, float duration, System.Action onComplete = null)
    {
        float currentValue;
        mainMixer.GetFloat(exposedParam, out currentValue);

        DOTween.To(() => currentValue, x =>
        {
            currentValue = x;
            mainMixer.SetFloat(exposedParam, currentValue);
        }, targetVolumeDb, duration)
        .SetEase(Ease.InOutSine)
        .OnComplete(() => onComplete?.Invoke());
    }

    // Change Music Track In-Game
    public void SetMusic(AudioClip newClip, float fadeTime = 0.6f)
    {
        StartCoroutine(SwapMusicRoutine(newClip, fadeTime));
    }

    private IEnumerator SwapMusicRoutine(AudioClip newClip, float fadeTime)
    {
        // If something is already playing, fade it out
        if (musicSource.isPlaying)
        {
            float startVol = musicSource.volume;
            float t = 0f;
            while (t < fadeTime)
            {
                t += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVol, 0f, t / fadeTime);
                yield return null;
            }
            musicSource.Stop();
        }

        // Assign and play the new clip
        musicSource.clip = newClip;
        musicClip = newClip;
        musicSource.volume = 0f;
        musicSource.Play();

        // Fade in
        float fadeInT = 0f;
        while (fadeInT < fadeTime)
        {
            fadeInT += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0f, 1f, fadeInT / fadeTime);
            yield return null;
        }
        musicSource.volume = 1f;
    }
}
