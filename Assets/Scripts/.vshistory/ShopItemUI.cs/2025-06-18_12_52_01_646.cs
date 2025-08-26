using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject darkFade;
    [SerializeField] private GameObject selectedBorder;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private TMP_Text titleText; // Only for music

    public void Setup(ShopItem item)
    {
        iconImage.sprite = item.Icon;

        // Title only for Music
        if (titleText != null)
        {
            titleText.gameObject.SetActive(item.Category == ShopCategory.Music);
            if (item.Category == ShopCategory.Music)
                titleText.text = item.Title;
        }

        // Owned logic
        bool isOwned = item.Owned;
        lockIcon.SetActive(!isOwned);
        darkFade.SetActive(!isOwned);
        costText.gameObject.SetActive(!isOwned);
        costText.text = item.Cost.ToString("0.00");

        // Selected logic
        selectedBorder.SetActive(item.Selected);
    }
}
