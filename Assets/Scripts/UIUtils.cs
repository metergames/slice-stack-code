using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public static class UIUtils
{
    public static bool IsPointerOverUIButton()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);

#if UNITY_EDITOR || UNITY_STANDALONE
        pointerData.position = Input.mousePosition;
#elif UNITY_IOS || UNITY_ANDROID
        if (Input.touchCount == 0)
            return false;

        pointerData.position = Input.GetTouch(0).position;
#endif

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.GetComponent<Button>() != null)
                return true;
        }

        return false;
    }
}
