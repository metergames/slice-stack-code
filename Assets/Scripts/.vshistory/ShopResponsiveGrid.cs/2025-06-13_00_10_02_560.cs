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

        float s = totalWidthMinusFixed / 31f; // Calculate the base unit 's'
        float spacing = 2f * s; // Spacing is 2 times 's'
        float cellSize = 9f * s; // CellSize is 9 times 's'

        // x is spacing, 4x is cellSize
        //float spacing = totalWidthMinusFixed / 14f;
        //float cellSize = 4f * spacing;

        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
        gridLayoutGroup.spacing = new Vector2(spacing, spacing);
    }
}
