using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public RectTransform settingsPanel;
    public CanvasGroup settingsCanvasGroup;
    public float animationDuration = 0.4f;

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
    private bool isOpen = false;

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
        PulseButton(musicButton);
    }

    void ToggleSound()
    {
        soundOn = !soundOn;
        AudioManager.Instance.SetSFXEnabled(soundOn);
        AudioManager.Instance.SetUIEnabled(soundOn);
        PlayerPrefs.SetInt("SoundOn", soundOn ? 1 : 0);
        UpdateUI();
        PulseButton(soundButton);
    }

    void ToggleVibration()
    {
        vibrationOn = !vibrationOn;
        PlayerPrefs.SetInt("VibrationOn", vibrationOn ? 1 : 0);
        UpdateUI();
        PulseButton(vibrationButton);

        StackManager.VibrateClick(); // Only vibrate when turning vibrations on
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

    private void PulseButton(Button button)
    {
        Transform t = button.transform;

        // Kill any ongoing tweens on this transform
        t.DOKill();

        // Animate scale punch
        t.localScale = Vector3.one;
        t.DOPunchScale(Vector3.one * 0.15f, 0.2f, 6, 0.5f).SetEase(Ease.OutQuad);

        // Play UI click sound
        AudioManager.Instance.PlayUISound();
    }

    public void ToggleSettings()
    {
        AudioManager.Instance.PlayUISound();

        if (isOpen)
            CloseSettings();
        else
            OpenSettings();
    }

    private void OpenSettings()
    {
        isOpen = true;

        settingsPanel.localScale = Vector3.zero;
        settingsCanvasGroup.alpha = 0f;
        settingsCanvasGroup.interactable = false;
        settingsCanvasGroup.blocksRaycasts = false;

        settingsPanel.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
        settingsCanvasGroup.DOFade(1f, animationDuration).OnComplete(() =>
        {
            settingsCanvasGroup.interactable = true;
            settingsCanvasGroup.blocksRaycasts = true;
        });
    }

    private void CloseSettings()
    {
        isOpen = false;

        settingsCanvasGroup.interactable = false;
        settingsCanvasGroup.blocksRaycasts = false;

        settingsPanel.DOScale(0f, animationDuration).SetEase(Ease.InBack);
        settingsCanvasGroup.DOFade(0f, animationDuration);
    }

    public bool IsSettingsOpen() => isOpen;
}
