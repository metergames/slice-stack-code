using UnityEngine;
using UnityEngine.UI;

public class ShopResponsiveGrid : MonoBehaviour
{
    void Start()
    {
        float width = this.GetComponent<RectTransform>().rect.width;
        Debug.Log(width);
        var gridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        float cellSize = (width - 50 - 30) / 3f;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }
}
