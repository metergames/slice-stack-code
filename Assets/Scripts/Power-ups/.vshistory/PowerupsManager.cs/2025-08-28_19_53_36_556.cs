using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerupsManager : MonoBehaviour
{
    [System.Serializable]
    public class PowerUpUI
    {
        public Button selectButton;
        public Button infoButton;
        public Image background;       // for yellow/green color
        public Text countText;         // displays owned count
        public Image mainBadge;        // badge on main game screen
        public ShopItem shopItem;      // reference to shop item
        public bool isSelected = false;
    }

    public List<PowerUpUI> powerUps;
    public GameObject infoPopup;       // panel for info details
    public Text infoTitle;
    public Text infoDescription;

    private Color selectedColor = new Color32(0, 200, 0, 255); // green
    private Color unselectedColor = new Color32(255, 220, 0, 255); // yellow

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

        p.isSelected = !p.isSelected;
        RefreshUI(p);
    }

    private void RefreshUI(PowerUpUI p)
    {
        // update color
        p.background.color = p.isSelected ? selectedColor : unselectedColor;

        // update count
        p.countText.text = "x" + p.shopItem.OwnedCount.ToString();

        // show/hide badge
        p.mainBadge.gameObject.SetActive(p.isSelected);
    }

    private void ShowInfo(PowerUpUI p)
    {
        if (infoPopup == null) return;

        infoPopup.SetActive(true);
        infoTitle.text = p.shopItem.name;
        infoDescription.text = p.shopItem.ConfirmName; // or use a proper description field
    }

    public void CloseInfo()
    {
        if (infoPopup != null)
            infoPopup.SetActive(false);
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
}
