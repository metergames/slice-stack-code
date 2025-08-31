using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class PowerupsManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpUI
    {
        public Button selectButton;
        public Image background;       // for yellow/green color
        public TextMeshProUGUI countText;         // displays owned count
        public GameObject mainBadge;        // badge on main game screen
        public ShopItem shopItem;      // reference to shop item
        public bool isSelected = false;

        [Header("Info")]
        public Button infoButton;
        public string infoTitle;
        [TextArea]
        public string infoText;
    }

    public List<PowerUpUI> powerUps;
    public GameObject infoPopup;       // panel for info details
    public TextMeshProUGUI infoTitlePop;
    public TextMeshProUGUI infoDescriptionPop;

    [Header("References")]
    [SerializeField] private RectTransform powerupsPanel;
    [SerializeField] private CanvasGroup powerupsCanvasGroup;
    [SerializeField] private Button shopButton;
    [SerializeField] private Button settingsButton;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.3f;

    public Color selectedColor = new Color32(112, 213, 83, 255); // green
    public Color unselectedColor = new Color32(213, 200, 83, 255); // yellow
    private bool isOpen = false;

    private void Start()
    {
        foreach (var p in powerUps)
        {
            p.selectButton.onClick.AddListener(() => TogglePowerUp(p));
            p.infoButton.onClick.AddListener(() => ShowInfo(p));
            RefreshUI(p);
        }

        if (infoPopup != null)
            infoPopup.SetActive(false);
    }

    private void TogglePowerUp(PowerUpUI p)
    {
        if (p.shopItem.OwnedCount <= 0) return; // can’t select if not owned

        AudioManager.Instance.PlayUISound();
        p.isSelected = !p.isSelected;
        RefreshUI(p);
    }

    private void RefreshUI(PowerUpUI p)
    {
        // update color
        p.background.color = p.isSelected ? selectedColor : unselectedColor;

        // update count
        p.countText.text = p.shopItem.OwnedCount.ToString();

        // show/hide badge
        p.mainBadge.gameObject.SetActive(p.isSelected);
    }

    private void ShowInfo(PowerUpUI p)
    {
        if (infoPopup == null) return;

        infoPopup.SetActive(true);
        AudioManager.Instance.PlayUISound();
        infoTitlePop.text = p.infoTitle;
        infoDescriptionPop.text = p.infoText;
    }

    public void CloseInfo()
    {
        if (infoPopup != null)
            infoPopup.SetActive(false);
        AudioManager.Instance.PlayUISound();
    }

    public List<ShopItem> GetSelectedPowerUps()
    {
        List<ShopItem> selected = new List<ShopItem>();
        foreach (var p in powerUps)
        {
            if (p.isSelected)
                selected.Add(p.shopItem);
        }
        return selected;
    }

    public void ResetSelectedPowerUps()
    {
        foreach (var p in powerUps)
        {
            if (p.isSelected)
                p.isSelected = false;
        }
    }

    public void TogglePowerups()
    {
        AudioManager.Instance.PlayUISound();

        shopButton.interactable = !shopButton.interactable;
        settingsButton.interactable = !settingsButton.interactable;

        if (isOpen)
            ClosePowerups();
        else
            OpenPowerups();
    }

    private void OpenPowerups()
    {
        isOpen = true;

        powerupsPanel.localScale = Vector3.zero;
        powerupsCanvasGroup.alpha = 0f;
        powerupsCanvasGroup.interactable = false;
        powerupsCanvasGroup.blocksRaycasts = false;

        powerupsPanel.DOScale(1f, animationDuration).SetEase(Ease.OutBack);
        powerupsCanvasGroup.DOFade(1f, animationDuration).OnComplete(() =>
        {
            powerupsCanvasGroup.interactable = true;
            powerupsCanvasGroup.blocksRaycasts = true;
        });
    }

    private void ClosePowerups()
    {
        isOpen = false;

        powerupsCanvasGroup.interactable = false;
        powerupsCanvasGroup.blocksRaycasts = false;

        powerupsPanel.DOScale(0f, animationDuration).SetEase(Ease.InBack);
        powerupsCanvasGroup.DOFade(0f, animationDuration);
    }

    public bool IsPowerupsOpen() => isOpen;
}
