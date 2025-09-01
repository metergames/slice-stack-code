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

        countText.gameObject.SetActive(false); // default hidden
        if (item.Category == ShopCategory.Extras)
        {
            lockIcon.SetActive(false);
            darkFade.SetActive(false);

            if (item.PurchaseWith == PurchaseType.Coins)
            {
                // Show count for coin-purchased extras
                countText.SetActive(true);
                countText.GetComponentInChildren<TextMeshProUGUI>().text = item.OwnedCount.ToString();
            }
            else if (item.PurchaseWith == PurchaseType.RealMoney)
            {
                // Real money extras should never show count
                countText.SetActive(false);
            }
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
