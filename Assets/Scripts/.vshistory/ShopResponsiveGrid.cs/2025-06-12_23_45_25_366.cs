using UnityEngine;
using UnityEngine.UI;

public class ShopResponsiveGrid : MonoBehaviour
{
    void Start()
    {
        float width = scrollViewRectTransform.rect.width;
        float cellSize = (width - totalSpacing) / 3f;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);

    }
}
