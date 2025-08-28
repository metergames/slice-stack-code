using UnityEngine;

public enum ShopCategory
{
    Skins,
    Backgrounds,
    Music,
    Extras
}

public enum PurchaseType
{
    Coins,
    RealMoney
}

[CreateAssetMenu(fileName = "NewShopItem", menuName = "Shop/Shop Item")]
public class ShopItem : ScriptableObject
{
    // [Header("Identification")]
    [SerializeField] private string id;
    [SerializeField] private ShopCategory category;

    // [Header("Display")]
    [SerializeField] private Sprite icon;

    [SerializeField] private string title; // Only used if category == Music
    [SerializeField] private int ownedCount; // Only used if category == Extras

    [SerializeField] private Material skinMaterial; // Only used if category == Skins
    [SerializeField] private Material backgroundMaterial; // Only used if category == Backgrounds
    [SerializeField] private AudioClip audioClip; // Only used if category == Music
    [SerializeField] private ShopItem referenceItem;

    // [Header("Purchase Settings")]
    [SerializeField] private PurchaseType purchaseType;
    [SerializeField] private float cost;

    // [Header("State")]
    [SerializeField] private bool owned;
    [SerializeField] private bool selected;

    // [Header("Confirmation Prompt")]
    // [Tooltip("Used to complete: 'Are you sure you would like to purchase {this}?'")]
    [SerializeField] private string confirmName;

    // Properties (optional for cleaner access)
    public string ID => id;
    public ShopCategory Category => category;
    public Sprite Icon => icon;
    public string Title => title;
    public int OwnedCount => ownedCount;
    public Material SkinMaterial => skinMaterial;
    public Material BackgroundMaterial => backgroundMaterial;
    public AudioClip AudioClip => audioClip;
    public PurchaseType PurchaseWith => purchaseType;
    public float Cost => cost;
    public bool Owned => owned;
    public bool Selected => selected;
    public string ConfirmName => confirmName;

    public void SetSelected(bool value) => selected = value;
    public void SetOwned(bool value) => owned = value;
    public void SetOwnedCount(int value) => ownedCount = value;

    public void LoadState()
    {
        owned = PlayerPrefs.GetInt(ID + "_Owned", owned ? 1 : 0) == 1;
        selected = PlayerPrefs.GetInt(ID + "_Selected", selected ? 1 : 0) == 1;
        ownedCount = PlayerPrefs.GetInt(ID + "_OwnedCount", ownedCount);
    }

    public void SaveState()
    {
        PlayerPrefs.SetInt(ID + "_Owned", owned ? 1 : 0);
        PlayerPrefs.SetInt(ID + "_Selected", selected ? 1 : 0);
        PlayerPrefs.SetInt(ID + "_OwnedCount", ownedCount);
        PlayerPrefs.Save();
    }
}
