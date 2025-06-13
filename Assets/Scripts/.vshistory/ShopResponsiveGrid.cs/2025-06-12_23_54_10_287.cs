using UnityEngine;
using UnityEngine.UI;

public class ShopResponsiveGrid : MonoBehaviour
{
    void Start()
    {
        float width = this.GetComponent<RectTransform>().rect.width;
        var gridLayoutGroup = this.GetComponent<GridLayoutGroup>();
        float cellSize = (width - 30 - 50) / 3f;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }
}
