using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Data")]
    [SerializeField] private List<ShopItem> allShopItems;

    [Header("UI References")]
    [SerializeField] private Transform scrollViewContent;
    [SerializeField] private GameObject shopItemPrefab;

    [Header("Tab Buttons (Optional)")]
    [SerializeField] private Button skinsTab;
    [SerializeField] private Button backgroundsTab;
    [SerializeField] private Button musicTab;
    [SerializeField] private Button extrasTab;

    private ShopCategory currentCategory;

    private void Start()
    {
        // Optional: Hook up tab buttons (if not using UnityEvents)
        if (skinsTab) skinsTab.onClick.AddListener(() => ShowCategory(ShopCategory.Skins));
        if (backgroundsTab) backgroundsTab.onClick.AddListener(() => ShowCategory(ShopCategory.Backgrounds));
        if (musicTab) musicTab.onClick.AddListener(() => ShowCategory(ShopCategory.Music));
        if (extrasTab) extrasTab.onClick.AddListener(() => ShowCategory(ShopCategory.Extras));

        ShowCategory(ShopCategory.Skins); // Default tab
    }

    public void ShowCategory(ShopCategory category)
    {
        currentCategory = category;

        // Clear previous entries
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // Filter items by selected category
        foreach (ShopItem item in allShopItems)
        {
            if (item.Category != category) continue;

            GameObject go = Instantiate(shopItemPrefab, scrollViewContent);
            ShopItemUI itemUI = go.GetComponent<ShopItemUI>();
            itemUI.Setup(item);
        }
    }
}
