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

        float baseUnit = totalWidthMinusFixed / 65f;

        float spacing = 4f * baseUnit;
        float cellSize = 19f * baseUnit;

        // x is spacing, 4x is cellSize
        //float spacing = totalWidthMinusFixed / 14f;
        //float cellSize = 4f * spacing;

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }
}
