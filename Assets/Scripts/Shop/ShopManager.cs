using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance;

    [Header("UI")]
    public Transform contentParent;
    public GameObject shopItemPrefab;
    public Button skinsTab, backgroundsTab, musicTab, extrasTab;
    public TextMeshProUGUI shopCoinsLabel;

    [Header("Tab Colors")]
    public Color selectedColor = new Color(0.12f, 0.69f, 0.36f);  // #1FB05B
    public Color unselectedColor = new Color(0.67f, 0.67f, 0.67f); // #AAAAAA
    public Color selectedTextColor = Color.white;                 // #FFFFFF
    public Color unselectedTextColor = new Color(0.196f, 0.196f, 0.196f); // #323232

    [Header("Coin Label Animation")]
    public Color coinLabelErrorColor = new Color32(239, 68, 68, 255); // Red color for error

    [Header("Scene References")]
    public GameObject stackBlockPrefab;
    public Transform backgroundObject;
    public Transform backgroundPrefab;
    public AudioManager audioManager;
    public AudioManager audioManagerPrefab;

    [Header("Items")]
    public List<ShopItem> allItems;

    public static event System.Action OnBackgroundChanged;

    private ShopCategory currentCategory;
    private List<ShopItemUI> spawnedItems = new List<ShopItemUI>();
    private Color coinLabelOriginalColor;

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

        // Cache original coin label color
        if (shopCoinsLabel != null)
            coinLabelOriginalColor = shopCoinsLabel.color;

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

        if (currentCategory == ShopCategory.Backgrounds)
        {
            var groups = new[]
            {
                BackgroundGroup.None,
                BackgroundGroup.ClassicMinimal,
                BackgroundGroup.SoftPastels,
                BackgroundGroup.DarkAesthetics,
                BackgroundGroup.Neon
            };

            foreach (var group in groups)
            {
                foreach (var item in allItems)
                {
                    if (item.Category == ShopCategory.Backgrounds && item.BackgroundGroup == group)
                    {
                        var go = Instantiate(shopItemPrefab, contentParent);
                        var ui = go.GetComponent<ShopItemUI>();
                        ui.Setup(item, this);
                        spawnedItems.Add(ui);
                    }
                }
            }
        }
        else
        {
            foreach (var item in allItems)
            {
                if (item.Category == currentCategory)
                {
                    var go = Instantiate(shopItemPrefab, contentParent);
                    var ui = go.GetComponent<ShopItemUI>();
                    ui.Setup(item, this);
                    spawnedItems.Add(ui);
                }
            }
        }
    }

    public void UpdateShopCoinsLabel()
    {
        if (shopCoinsLabel != null)
        {
            int coins = CurrencyManager.Instance.GetCoinCount();
            shopCoinsLabel.text = $"<sprite=0>{coins}";
        }
    }

    public void PlayInsufficientFundsAnimation()
    {
        if (shopCoinsLabel == null) return;

        RectTransform rt = shopCoinsLabel.rectTransform;

        // Kill any existing tweens on label
        DOTween.Kill(shopCoinsLabel);
        DOTween.Kill(rt);

        // Reset to original color
        shopCoinsLabel.color = coinLabelOriginalColor;

        // Flash red and shake
        Sequence seq = DOTween.Sequence();

        // Color flash to red then back
        seq.Append(shopCoinsLabel.DOColor(coinLabelErrorColor, 0.1f));
        seq.Join(rt.DOShakeAnchorPos(0.4f, new Vector2(15f, 0f), 20, 90, false, true));
        seq.Append(shopCoinsLabel.DOColor(coinLabelOriginalColor, 0.3f));

        // Play error sound
        AudioManager.Instance?.PlayUIErrorSound();
    }

    public void OnItemSelected(ShopItem item)
    {
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
                break;

            case ShopCategory.Backgrounds:
                backgroundObject.GetComponent<Image>().material = item.BackgroundMaterial;
                backgroundPrefab.GetComponent<Image>().material = item.BackgroundMaterial;
                OnBackgroundChanged?.Invoke();
                break;

            case ShopCategory.Music:
                audioManager.SetMusic(item.AudioClip);
                audioManagerPrefab.musicClip = item.AudioClip;
                PlayerPrefs.SetString("SelectedMusicID", item.ID);
                PlayerPrefs.Save();
                break;
        }

        RefreshItems();
    }

    public bool AttemptPurchase(ShopItem item)
    {
        if (item.Category == ShopCategory.Extras)
        {
            if (item.PurchaseWith == PurchaseType.Coins)
            {
                if (CurrencyManager.Instance.SpendCoins((int)item.Cost))
                {
                    item.SetOwnedCount(item.OwnedCount + 1);
                    item.SaveState();
                    CurrencyManager.Instance.uiManager.UpdateCoins(CurrencyManager.Instance.GetCoinCount());
                    UpdateShopCoinsLabel();
                    ChangeCategory(currentCategory);
                    Debug.Log($"Purchased extra: {item.ID}. New count: {item.OwnedCount}");
                    return true;
                }
                else
                {
                    Debug.Log("Not enough coins for extra.");
                    PlayInsufficientFundsAnimation();
                    return false;
                }
            }
            else
            {
                switch (item.ID)
                {
                    case "EXTRA_MULTIPLELIVES_01":
                        Debug.Log("Initiating IAP for 10 extra lives...");
                        item.ReferenceItem.SetOwnedCount(item.ReferenceItem.OwnedCount + 10);
                        item.ReferenceItem.SaveState();
                        ChangeCategory(currentCategory);
                        Debug.Log("Player received 10 extra lives!");
                        return true;

                    case "EXTRA_ADD1000COINS_06":
                        CurrencyManager.Instance.AddCoins(1000);
                        UpdateShopCoinsLabel();
                        Debug.Log("Player purchased 1000 coins!");
                        return true;

                    default:
                        Debug.LogWarning($"Unhandled real money item: {item.ID}");
                        return false;
                }
            }
        }
        else
        {
            if (item.Owned)
            {
                Debug.Log("Item already owned.");
                return false;
            }

            if (item.PurchaseWith == PurchaseType.Coins)
            {
                if (CurrencyManager.Instance.SpendCoins((int)item.Cost))
                {
                    item.SetOwned(true);
                    item.SaveState();
                    CurrencyManager.Instance.uiManager.UpdateCoins(CurrencyManager.Instance.GetCoinCount());
                    UpdateShopCoinsLabel();
                    ChangeCategory(currentCategory);
                    Debug.Log($"Purchased: {item.ID}");
                    return true;
                }
                else
                {
                    Debug.Log("Not enough coins.");
                    PlayInsufficientFundsAnimation();
                    return false;
                }
            }
            else
            {
                Debug.Log("Real money purchase is not implemented.");
                return false;
            }
        }
    }

    public void OpenShopMenu()
    {
        PlayerPrefs.SetInt("ReadyToPlay", 0);
        UpdateShopCoinsLabel();
        RefreshItems();
    }

    public void CloseShopMenu()
    {
        PlayerPrefs.SetInt("ReadyToPlay", 1);
    }
}
