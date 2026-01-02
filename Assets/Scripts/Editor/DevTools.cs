using UnityEngine;
using UnityEditor;

public class DevTools : MonoBehaviour
{
    // This adds an item to the top menu bar in Unity
    [MenuItem("Tools/Clear PlayerPrefs")]
    public static void ClearPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.LogWarning("PlayerPrefs Cleared Successfully!");
    }
}