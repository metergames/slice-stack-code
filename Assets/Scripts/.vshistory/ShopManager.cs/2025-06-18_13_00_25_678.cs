using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI")]
    public Transform contentParent;
    public GameObject shopItemPrefab;
    public Button skinsTab, backgroundsTab, musicTab, extrasTab;
    public Color selectedColor = new Color(0.12f, 0.69f, 0.36f);  // #1FB05B
    public Color unselectedColor = new Color(0.67f, 0.67f, 0.67f); // #AAAAAA

    [Header("Items")]
    public List<ShopItem> allItems = new List<ShopItem>();

    private ShopCategory currentCategory;
    private List<ShopItemUI> spawnedItems = new List<ShopItemUI>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        skinsTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Skins));
        backgroundsTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Backgrounds));
        musicTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Music));
        extrasTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Extras));

        ChangeCategory(ShopCategory.Skins); // default tab
    }

    private void ChangeCategory(ShopCategory category)
    {
        currentCategory = category;
        UpdateTabColors();
        ClearItems();

        foreach (var item in allItems)
        {
            if (item.Category == category)
            {
                var go = Instantiate(shopItemPrefab, contentParent);
                var itemUI = go.GetComponent<ShopItemUI>();
                itemUI.Setup(item);
                spawnedItems.Add(itemUI);
            }
        }
    }

    private void UpdateTabColors()
    {
        skinsTab.GetComponent<Image>().color = currentCategory == ShopCategory.Skins ? selectedColor : unselectedColor;
        backgroundsTab.GetComponent<Image>().color = currentCategory == ShopCategory.Backgrounds ? selectedColor : unselectedColor;
        musicTab.GetComponent<Image>().color = currentCategory == ShopCategory.Music ? selectedColor : unselectedColor;
        extrasTab.GetComponent<Image>().color = currentCategory == ShopCategory.Extras ? selectedColor : unselectedColor;
    }

    private void ClearItems()
    {
        foreach (var item in spawnedItems)
        {
            Destroy(item.gameObject);
        }
        spawnedItems.Clear();
    }

    public void AttemptPurchase(ShopItem item)
    {
        if (item.Owned)
        {
            Debug.Log("Item already owned.");
            return;
        }

        if (item.PurchaseWith == PurchaseType.Coins)
        {
            if (CurrencyManager.Instance.SpendCoins((int)item.Cost))
            {
                item.GetType().GetField("owned", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(item, true);
                CurrencyManager.Instance.uiManager.UpdateCoins(CurrencyManager.Instance.GetCoinCount());
                ChangeCategory(currentCategory); // refresh display
                Debug.Log($"Purchased: {item.ID}");
            }
            else
            {
                Debug.Log("Not enough coins.");
            }
        }
        else
        {
            Debug.Log("Real money purchase is not implemented.");
        }
    }
}
