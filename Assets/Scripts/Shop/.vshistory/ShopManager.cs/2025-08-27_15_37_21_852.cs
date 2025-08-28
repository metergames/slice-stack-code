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

    [Header("Tab Colors")]
    public Color selectedColor = new Color(0.12f, 0.69f, 0.36f);  // #1FB05B
    public Color unselectedColor = new Color(0.67f, 0.67f, 0.67f); // #AAAAAA
    public Color selectedTextColor = Color.white;                 // #FFFFFF
    public Color unselectedTextColor = new Color(0.196f, 0.196f, 0.196f); // #323232

    [Header("Scene References")]
    public GameObject stackBlockPrefab;  // assigned prefab
    public Transform backgroundObject;   // background GameObject with material
    public Transform backgroundPrefab;   // background GameObject prefab with material
    public AudioManager audioManager;     // for switching music
    public AudioManager audioManagerPrefab;     // prefab for switching music

    [Header("Items")]
    public List<ShopItem> allItems;

    private ShopCategory currentCategory;
    private List<ShopItemUI> spawnedItems = new List<ShopItemUI>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var item in allItems)
            item.LoadState();

        skinsTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Skins));
        backgroundsTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Backgrounds));
        musicTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Music));
        extrasTab.onClick.AddListener(() => ChangeCategory(ShopCategory.Extras));

        ChangeCategory(ShopCategory.Skins);
    }

    private void ChangeCategory(ShopCategory category)
    {
        currentCategory = category;
        UpdateTabVisuals();
        RefreshItems();
    }

    private void UpdateTabVisuals()
    {
        var tabs = new[]
        {
            new { btn = skinsTab, cat = ShopCategory.Skins },
            new { btn = backgroundsTab, cat = ShopCategory.Backgrounds },
            new { btn = musicTab, cat = ShopCategory.Music },
            new { btn = extrasTab, cat = ShopCategory.Extras }
        };

        foreach (var t in tabs)
        {
            Color bgColor = (currentCategory == t.cat) ? selectedColor : unselectedColor;
            t.btn.GetComponent<Image>().color = bgColor;

            var txt = t.btn.GetComponentInChildren<Text>();
            if (txt != null)
                txt.color = (currentCategory == t.cat) ? selectedTextColor : unselectedTextColor;
        }
    }

    private void RefreshItems()
    {
        foreach (var child in spawnedItems)
            Destroy(child.gameObject);
        spawnedItems.Clear();

        foreach (var item in allItems)
            if (item.Category == currentCategory)
            {
                var go = Instantiate(shopItemPrefab, contentParent);
                var ui = go.GetComponent<ShopItemUI>();
                ui.Setup(item, this);
                spawnedItems.Add(ui);
            }
    }

    public void OnItemSelected(ShopItem item)
    {
        // Mark selected flag appropriately
        foreach (var it in allItems)
            if (it.Category == item.Category)
            {
                it.SetSelected(it == item);
                it.SaveState();
            }

        switch (item.Category)
        {
            case ShopCategory.Skins:
                stackBlockPrefab.GetComponent<Renderer>().material = item.SkinMaterial;
                //var block = Instantiate(stackBlockPrefab);
                //block.GetComponent<Renderer>().material = item.SkinMaterial; // does it for this newly instantiated block, but not for all the next ones. maybe make it a property.
                break;

            case ShopCategory.Backgrounds:
                backgroundObject.GetComponent<Image>().material = item.BackgroundMaterial;
                backgroundPrefab.GetComponent<Image>().material = item.BackgroundMaterial;
                break;

            case ShopCategory.Music:
                audioManager.SetMusic(item.AudioClip);
                audioManagerPrefab.musicClip = item.AudioClip;
                break;
        }

        RefreshItems();
    }

    public void AttemptPurchase(ShopItem item)
    {
        if (item.Category == ShopCategory.Extras) // Extras
        {
            if (item.PurchaseWith == PurchaseType.Coins)
            {
                if (CurrencyManager.Instance.SpendCoins((int)item.Cost))
                {
                    item.SetOwnedCount(item.OwnedCount + 1);
                    item.SaveState();
                    CurrencyManager.Instance.uiManager.UpdateCoins(CurrencyManager.Instance.GetCoinCount());
                    ChangeCategory(currentCategory); // refresh display
                    Debug.Log($"Purchased extra: {item.ID}. New count: {item.OwnedCount}");
                }
                else
                {
                    Debug.Log("Not enough coins for extra.");
                }
            }
            else
            {
                switch (item.ID)
                {
                    case "extra_lives_pack_10":
                        // Here you’d normally trigger Unity IAP purchase
                        Debug.Log("Initiating IAP for 10 extra lives...");

                        // For testing, just grant instantly
                        item.SetOwnedCount(item.OwnedCount + 10);
                        item.SaveState();
                        ChangeCategory(currentCategory);
                        Debug.Log("Player received 10 extra lives!");
                        break;

                    case "coins_pack_1000":
                        // Example: grant coins
                        CurrencyManager.Instance.AddCoins(1000);
                        Debug.Log("Player purchased 1000 coins!");
                        break;

                    default:
                        Debug.LogWarning($"Unhandled real money item: {item.ID}");
                        break;
                }
            }
        }
        else // Skins, Backgrounds, Music
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
                    item.SetOwned(true);
                    item.SaveState();
                    //item.GetType().GetField("owned", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(item, true);
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
}
