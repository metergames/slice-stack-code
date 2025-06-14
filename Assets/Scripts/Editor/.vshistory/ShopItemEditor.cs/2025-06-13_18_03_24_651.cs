using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ShopItem))] // This tells Unity which script this editor is for
public class ShopItemEditor : Editor
{
    // SerializedProperties to easily work with the fields
    private SerializedProperty idProp;
    private SerializedProperty categoryProp;
    private SerializedProperty iconProp;
    private SerializedProperty titleProp;
    private SerializedProperty ownedCountProp;
    private SerializedProperty purchaseTypeProp;
    private SerializedProperty costProp;
    private SerializedProperty ownedProp;
    private SerializedProperty selectedProp;
    private SerializedProperty confirmNameProp;

    private void OnEnable()
    {
        // Get references to all the serialized properties
        idProp = serializedObject.FindProperty("id");
        categoryProp = serializedObject.FindProperty("category");
        iconProp = serializedObject.FindProperty("icon");
        titleProp = serializedObject.FindProperty("title");
        ownedCountProp = serializedObject.FindProperty("ownedCount");
        purchaseTypeProp = serializedObject.FindProperty("purchaseType");
        costProp = serializedObject.FindProperty("cost");
        ownedProp = serializedObject.FindProperty("owned");
        selectedProp = serializedObject.FindProperty("selected");
        confirmNameProp = serializedObject.FindProperty("confirmName");
    }

    public override void OnInspectorGUI()
    {
        // Always start with this to ensure changes are registered
        serializedObject.Update();

        // --- Identification Header & Fields ---
        EditorGUILayout.LabelField("Identification", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(idProp);
        EditorGUILayout.PropertyField(categoryProp);

        EditorGUILayout.Space(); // Add some space for readability

        // --- Display Header & Fields ---
        EditorGUILayout.LabelField("Display", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(iconProp);

        // Conditional display for 'title' based on 'category'
        if ((ShopCategory)categoryProp.enumValueIndex == ShopCategory.Music)
        {
            EditorGUILayout.PropertyField(titleProp, new GUIContent("Title (Music Item)"));
        }

        // Conditional display for 'ownedCount' based on 'category'
        if ((ShopCategory)categoryProp.enumValueIndex == ShopCategory.Extras)
        {
            EditorGUILayout.PropertyField(ownedCountProp, new GUIContent("Owned Count (Extras Item)"));
        }

        EditorGUILayout.Space();

        // --- Purchase Settings Header & Fields ---
        EditorGUILayout.LabelField("Purchase Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(purchaseTypeProp);
        EditorGUILayout.PropertyField(costProp);

        EditorGUILayout.Space();

        // --- State Header & Fields ---
        EditorGUILayout.LabelField("State", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(ownedProp);
        EditorGUILayout.PropertyField(selectedProp);

        EditorGUILayout.Space();

        // --- Confirmation Prompt Header & Field ---
        EditorGUILayout.LabelField("Confirmation Prompt", EditorStyles.boldLabel);
        // You can add the Tooltip back here if you want it to be static,
        // or dynamically generate help text as discussed in the previous answer.
        EditorGUILayout.PropertyField(confirmNameProp, new GUIContent(confirmNameProp.displayName, "Used to complete: 'Are you sure you would like to purchase {this}?'"));


        // Apply changes to the serialized object
        serializedObject.ApplyModifiedProperties();
    }
}