using UnityEngine;

public class StackManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public float blockHeight = 1f;
    public Transform startPosition;

    private GameObject lastBlock;
    private BlockMover.Axis currentAxis = BlockMover.Axis.X;

    private void Start()
    {
        SpawnFirstBlock();
        SpawnNextBlock();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Click on screen
            PlaceBlock();
    }

    private void SpawnFirstBlock()
    {
        lastBlock = Instantiate(blockPrefab, startPosition.position, Quaternion.identity);
        lastBlock.name = "BaseBlock";
        lastBlock.GetComponent<BlockMover>().enabled = false;
    }

    private void SpawnNextBlock()
    {
        Vector3 spawnPos = lastBlock.transform.position + Vector3.up * blockHeight;

        GameObject newBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        BlockMover mover = newBlock.GetComponent<BlockMover>();

        mover.moveAxis = currentAxis;
        mover.StartMovement();

        lastBlock = newBlock;

        currentAxis = currentAxis == BlockMover.Axis.X ? BlockMover.Axis.Z : BlockMover.Axis.X; // Alternate the axis
    }

    private void PlaceBlock()
    {
        BlockMover mover = lastBlock.GetComponent<BlockMover>();
        mover.StopMovement();

        // Slice logic

        SpawnNextBlock();
    }
}
