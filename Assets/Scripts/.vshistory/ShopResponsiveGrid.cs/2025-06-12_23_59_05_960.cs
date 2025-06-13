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

        float spacing = totalWidthMinusFixed / 14f;
        float cell

        float cellSize = (width - 30 - 50) / 3f;
        gridLayoutGroup.cellSize = new Vector2(cellSize, cellSize);
    }
}
