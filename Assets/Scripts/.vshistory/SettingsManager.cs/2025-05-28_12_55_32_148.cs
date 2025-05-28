using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Buttons")]
    public Button musicButton;
    public Button soundButton;
    public Button vibrationButton;

    [Header("Colors")]
    private readonly Color onColor = new Color32(0, 200, 83, 255);     // #00C853 (green)
    private readonly Color offColor = new Color32(213, 0, 0, 255);     // #D50000 (red)

    private bool musicOn = true;
    private bool soundOn = true;
    private bool vibrationOn = true;

    void Start()
    {
        LoadSettings();
        UpdateUI();

        musicButton.onClick.AddListener(ToggleMusic);
        soundButton.onClick.AddListener(ToggleSound);
        vibrationButton.onClick.AddListener(ToggleVibration);
    }

    void ToggleMusic()
    {
        musicOn = !musicOn;
        AudioManager.Instance.SetMusicEnabled(musicOn);
        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);
        UpdateUI();
    }

    void ToggleSound()
    {
        soundOn = !soundOn;
        AudioManager.Instance.SetSFXEnabled(soundOn);
        AudioManager.Instance.SetUIEnabled(soundOn);
        PlayerPrefs.SetInt("SoundOn", soundOn ? 1 : 0);
        UpdateUI();
    }

    void ToggleVibration()
    {
        vibrationOn = !vibrationOn;
        PlayerPrefs.SetInt("VibrationOn", vibrationOn ? 1 : 0);
        UpdateUI();
    }

    void LoadSettings()
    {
        musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        soundOn = PlayerPrefs.GetInt("SoundOn", 1) == 1;
        vibrationOn = PlayerPrefs.GetInt("VibrationOn", 1) == 1;

        AudioManager.Instance.SetMusicEnabled(musicOn);
        AudioManager.Instance.SetSFXEnabled(soundOn);
        AudioManager.Instance.SetUIEnabled(soundOn);
    }

    void UpdateUI()
    {
        musicButton.image.color = musicOn ? onColor : offColor;
        soundButton.image.color = soundOn ? onColor : offColor;
        vibrationButton.image.color = vibrationOn ? onColor : offColor;
    }

    public static bool IsVibrationEnabled()
    {
        return PlayerPrefs.GetInt("VibrationOn", 1) == 1;
    }
}
