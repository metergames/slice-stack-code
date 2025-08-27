using UnityEngine;
using UnityEngine.UI;

public class ShopResponsiveGrid : MonoBehaviour
{
    void Start()
    {
        AdjustGrid();
    }

    private void AdjustGrid()
    {
        RectTransform rectTransform = this.GetComponent<RectTransform>();
        GridLayoutGroup gridLayoutGroup = this.GetComponent<GridLayoutGroup>();

        float width = rectTransform.rect.width;
        float totalWidthMinusFixed = width - 30f; // Remove 30 for equal and correct placement

        // 4x is spacing, 19x is cellSize (almost 1:5)
        float baseUnit = totalWidthMinusFixed / 65f;
        float spacing = 4f * baseUnit;
        float cellSize = 19f * baseUnit;

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }
}
