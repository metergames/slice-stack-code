using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopManager : MonoBehaviour
{
    [Header("Shop Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject skinsPanel;
    [SerializeField] private GameObject backgroundsPanel;
    [SerializeField] private GameObject musicPanel;
    [SerializeField] private GameObject extrasPanel;

    [Header("Tabs")]
    [SerializeField] private Button skinsTab;
    [SerializeField] private Button backgroundsTab;
    [SerializeField] private Button musicTab;
    [SerializeField] private Button extrasTab;

    [Header("Item UI")]
    [SerializeField] private Transform itemContainer;
    [SerializeField] private GameObject shopItemUIPrefab;

    [Header("Purchase Popup")]
    [SerializeField] private GameObject purchasePopup;
    [SerializeField] private TMP_Text popupMessageText;
    [SerializeField] private Button confirmPurchaseButton;
    [SerializeField] private Button cancelPurchaseButton;

    [Header("Coin Display")]
    [SerializeField] private TMP_Text shopCoinText;
    [SerializeField] private TMP_Text mainCoinText;

    [Header("Tab Colors")]
    [SerializeField] private Color selectedColor = new Color(0.12f, 0.69f, 0.36f); // #1FB05B
    [SerializeField] private Color unselectedColor = new Color(0.67f, 0.67f, 0.67f); // #AAAAAA

    private Dictionary<ShopCategory, List<ShopItem>> categorizedItems;
    private Dictionary<ShopCategory, GameObject> categoryPanels;
    private Dictionary<ShopCategory, Button> tabButtons;
    private ShopCategory currentCategory;
    private ShopItem pendingPurchaseItem;

    private void Start()
    {
        categoryPanels = new Dictionary<ShopCategory, GameObject>
        {
            { ShopCategory.Skins, skinsPanel },
            { ShopCategory.Backgrounds, backgroundsPanel },
            { ShopCategory.Music, musicPanel },
            { ShopCategory.Extras, extrasPanel }
        };

        tabButtons = new Dictionary<ShopCategory, Button>
        {
            { ShopCategory.Skins, skinsTab },
            { ShopCategory.Backgrounds, backgroundsTab },
            { ShopCategory.Music, musicTab },
            { ShopCategory.Extras, extrasTab }
        };

        skinsTab.onClick.AddListener(() => ShowCategory(ShopCategory.Skins));
        backgroundsTab.onClick.AddListener(() => ShowCategory(ShopCategory.Backgrounds));
        musicTab.onClick.AddListener(() => ShowCategory(ShopCategory.Music));
        extrasTab.onClick.AddListener(() => ShowCategory(ShopCategory.Extras));

        confirmPurchaseButton.onClick.AddListener(ConfirmPurchase);
        cancelPurchaseButton.onClick.AddListener(CancelPurchase);

        LoadItems();
        ShowCategory(ShopCategory.Skins);
        UpdateCoinDisplay();
    }

    private void LoadItems()
    {
        categorizedItems = new Dictionary<ShopCategory, List<ShopItem>>();

        foreach (ShopCategory category in System.Enum.GetValues(typeof(ShopCategory)))
        {
            categorizedItems[category] = new List<ShopItem>();
        }

        ShopItem[] allItems = Resources.LoadAll<ShopItem>("ShopItems");
        foreach (ShopItem item in allItems)
        {
            categorizedItems[item.Category].Add(item);
        }
    }

    public void ShowCategory(ShopCategory category)
    {
        currentCategory = category;

        foreach (var panel in categoryPanels.Values)
        {
            panel.SetActive(false);
        }

        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ShopItem item in categorizedItems[category])
        {
            GameObject obj = Instantiate(shopItemUIPrefab, itemContainer);
            ShopItemUI ui = obj.GetComponent<ShopItemUI>();
            ui.Setup(item, this);
        }

        categoryPanels[category].SetActive(true);
        UpdateTabButtonColors();
    }

    private void UpdateTabButtonColors()
    {
        foreach (var kvp in tabButtons)
        {
            var colors = kvp.Value.colors;
            colors.normalColor = kvp.Key == currentCategory ? selectedColor : unselectedColor;
            kvp.Value.colors = colors;
        }
    }

    public void TryPurchaseItem(ShopItem item)
    {
        if (item.Owned)
        {
            Debug.Log("Item already owned.");
            return;
        }

        if (item.PurchaseWith == PurchaseType.Coins && GameData.Instance.Coins < item.Cost)
        {
            Debug.Log("Not enough coins.");
            return;
        }

        pendingPurchaseItem = item;
        string prompt = string.IsNullOrEmpty(item.ConfirmName)
            ? "Are you sure you want to purchase this item?"
            : $"Are you sure you want to purchase {item.ConfirmName}?";

        popupMessageText.text = prompt;
        purchasePopup.SetActive(true);
    }

    private void ConfirmPurchase()
    {
        if (pendingPurchaseItem == null) return;

        GameData.Instance.Coins -= (int)pendingPurchaseItem.Cost;
        GameData.Instance.Save();

        typeof(ShopItem)
            .GetField("owned", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .SetValue(pendingPurchaseItem, true);

        ShowCategory(currentCategory);
        UpdateCoinDisplay();

        purchasePopup.SetActive(false);
        pendingPurchaseItem = null;
    }

    private void CancelPurchase()
    {
        pendingPurchaseItem = null;
        purchasePopup.SetActive(false);
    }

    public void UpdateCoinDisplay()
    {
        int coins = GameData.Instance.Coins;
        shopCoinText.text = $"{coins}";
        mainCoinText.text = $"{coins}";
    }
}