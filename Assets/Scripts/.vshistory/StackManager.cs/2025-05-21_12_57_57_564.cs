using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public float blockHeight = 2f;
    public Transform startPosition;

    private GameObject lastBlock;
    private BlockMover.Axis currentAxis = BlockMover.Axis.X;
    private List<GameObject> stackBlocks = new List<GameObject>();

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
        stackBlocks.Add(lastBlock);
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

        stackBlocks.Add(lastBlock);
    }

    private void PlaceBlock()
    {
        BlockMover mover = lastBlock.GetComponent<BlockMover>();
        mover.StopMovement();

        Transform currentBlock = lastBlock.transform;
        Transform previousBlock = stackBlocks[stackBlocks.Count - 2].transform;

        DropBlock(currentBlock, previousBlock.position.y + blockHeight / 2f, () =>
        {
            stackBlocks.Add(currentBlock.gameObject);
            SpawnNextBlock();
        });
    }

    private void DropBlock(Transform block, float targetY, System.Action onComplete)
    {
        //Vector3 targetPosition = block.position;
        //targetPosition.y = lastBlock.transform.position.y + (blockHeight / 2f);

        block.DOMoveY(targetY, 0.2f).SetEase(Ease.OutBounce).OnComplete(() => onComplete?.Invoke());
    }
}
