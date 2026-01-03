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
    public GameObject countText;

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

        countText.gameObject.SetActive(false);
        if (item.Category == ShopCategory.Extras)
        {
            lockIcon.SetActive(false);
            darkFade.SetActive(false);

            if (item.PurchaseWith == PurchaseType.Coins)
            {
                countText.SetActive(true);
                countText.GetComponentInChildren<TextMeshProUGUI>().text = item.OwnedCount.ToString();
            }
            else if (item.PurchaseWith == PurchaseType.RealMoney)
            {
                countText.SetActive(false);
            }
        }

        itemButton.onClick.RemoveAllListeners();
        itemButton.onClick.AddListener(OnItemClicked);
    }

    private void OnItemClicked()
    {
        StackManager.VibrateClick();

        if (item.Owned)
        {
            AudioManager.Instance.PlayUISound();
            manager.OnItemSelected(item);
        }
        else
        {
            bool success = manager.AttemptPurchase(item);
            if (success)
            {
                AudioManager.Instance.PlayUIPurchaseSound();
            }
            // Error sound is played inside PlayInsufficientFundsAnimation() when purchase fails
        }
    }
}
