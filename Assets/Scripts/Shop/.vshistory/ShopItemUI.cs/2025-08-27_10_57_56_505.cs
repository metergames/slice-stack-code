using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;
    public GameObject lockIcon;
    public GameObject darkFade;
    public GameObject selectedBorder;
    public Button itemButton;
    public GameObject countText; // only for Extras

    private ShopItem item;
    private ShopManager manager;

    public void Setup(ShopItem shopItem, ShopManager shopManager)
    {
        item = shopItem;
        manager = shopManager;

        iconImage.sprite = item.Icon;

        if (item.Category == ShopCategory.Music)
            titleText.text = item.Title;
        else
            titleText.text = "";

        costText.text = item.PurchaseWith == PurchaseType.Coins
            ? $"{item.Cost:F0} Coins" : $"${item.Cost:F2}";

        bool isOwned = item.Owned;
        bool isSelected = item.Selected;

        lockIcon.SetActive(!isOwned);
        darkFade.SetActive(!isOwned);
        costText.gameObject.SetActive(!isOwned);
        selectedBorder.SetActive(isSelected);

        countText.gameObject.SetActive(item.Category == ShopCategory.Extras);
        if (item.Category == ShopCategory.Extras)
        {
            lockIcon.SetActive(false);
            darkFade.SetActive(false);
            countText.GetComponentInChildren<TextMeshProUGUI>().text = item.OwnedCount.ToString();
        }

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlayUISound();
            StackManager.VibrateClick();
            if (item.Owned)
                manager.OnItemSelected(item);
            else
                manager.AttemptPurchase(item);
        });
    }
}
