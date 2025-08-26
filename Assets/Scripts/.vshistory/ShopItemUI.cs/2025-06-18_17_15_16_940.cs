using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI costText;
    public GameObject lockIcon;
    public GameObject pricePanel;
    public GameObject border;
    public Button purchaseButton;

    private ShopItem item;

    public void Setup(ShopItem shopItem)
    {
        item = shopItem;
        iconImage.sprite = item.Icon;

        // Optional title field only for music
        if (item.Category == ShopCategory.Music)
            titleText.text = item.Title;
        else
            titleText.text = "";

        costText.text = item.PurchaseWith == PurchaseType.Coins
            ? item.Cost.ToString("F0") + " Coins"
            : "$" + item.Cost.ToString("F2");

        bool isOwned = item.Owned;
        bool isSelected = item.Selected;

        lockIcon.SetActive(!isOwned);
        pricePanel.SetActive(!isOwned);
        border.SetActive(isSelected);

        purchaseButton.onClick.RemoveAllListeners();
        purchaseButton.onClick.AddListener(() => ShopManager.Instance.AttemptPurchase(item));
    }
}
