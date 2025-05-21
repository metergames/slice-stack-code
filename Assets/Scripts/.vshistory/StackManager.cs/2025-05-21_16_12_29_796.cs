using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    public GameObject blockPrefab;
    //public float blockHeight = 2f;
    public Transform startPosition;
    public Transform cameraFollowTarget;
    public float cameraFollowSpeed = 5f;
    public float cameraYOffset = 1f;
    public float spawnHeightOffset = 1f;
    public float perfectStackThreshold = 0.1f;

    private GameObject lastBlock;
    private BlockMover.Axis currentAxis = BlockMover.Axis.X;
    private List<GameObject> stackBlocks = new List<GameObject>();
    private bool gameOver = false;

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
        float blockY = lastBlock.transform.localScale.y;
        Vector3 spawnPos = lastBlock.transform.position + Vector3.up * (blockY + spawnHeightOffset);
        Vector3 spawnScale = lastBlock.transform.localScale;

        GameObject newBlock = Instantiate(blockPrefab, spawnPos, Quaternion.identity);
        newBlock.transform.localScale = spawnScale;

        BlockMover mover = newBlock.GetComponent<BlockMover>();
        mover.moveAxis = currentAxis;
        mover.StartMovement();

        lastBlock = newBlock;

        currentAxis = currentAxis == BlockMover.Axis.X ? BlockMover.Axis.Z : BlockMover.Axis.X; // Alternate the axis

        stackBlocks.Add(lastBlock);
    }

    private void PlaceBlock()
    {
        if (gameOver)
            return;

        BlockMover mover = lastBlock.GetComponent<BlockMover>();
        mover.StopMovement();

        Transform currentBlock = lastBlock.transform;
        Transform previousBlock = stackBlocks[stackBlocks.Count - 2].transform;

        BlockMover.Axis sliceAxis = currentAxis == BlockMover.Axis.X ? BlockMover.Axis.Z : BlockMover.Axis.X;

        float delta = sliceAxis == BlockMover.Axis.X ? currentBlock.position.x - previousBlock.position.x : currentBlock.position.z - previousBlock.position.z;
        float overlap = sliceAxis == BlockMover.Axis.X ? previousBlock.localScale.x - Mathf.Abs(delta) : previousBlock.localScale.z - Mathf.Abs(delta);

        // Didn't land anywhere, game over
        if (overlap <= 0f)
        {
            Rigidbody rbLoss = currentBlock.gameObject.AddComponent<Rigidbody>();
            rbLoss.mass = 0.5f;
            rbLoss.angularVelocity = Random.insideUnitSphere * 5f;

            gameOver = true;
            return;
        }

        // Perfect stack forgiveness
        if (Mathf.Abs(delta) <= perfectStackThreshold)
        {
            // Snap to perfect alignment
            Vector3 perfectPos = previousBlock.position;
            perfectPos.y = currentBlock.position.y;
            currentBlock.position = perfectPos;

            float currentHeight = currentBlock.localScale.y;
            float previousHeight = previousBlock.localScale.y;
            float targetY = previousBlock.position.y + (previousHeight / 2f) + (currentHeight / 2f);

            DropBlock(currentBlock, targetY, () =>
            {
                stackBlocks.Add(currentBlock.gameObject);
                SpawnNextBlock();
            });

            MoveCamera();
            return;
        }

        float currHeight = currentBlock.localScale.y;
        float prevHeight = previousBlock.localScale.y;

        float finalY = previousBlock.position.y + (prevHeight / 2f) + (currHeight / 2f);

        DropBlock(currentBlock, finalY, () =>
        {
            SliceBlock(currentBlock.gameObject, previousBlock.gameObject, currentAxis == BlockMover.Axis.X ? BlockMover.Axis.Z : BlockMover.Axis.X);

            //stackBlocks.Add(currentBlock.gameObject);
            SpawnNextBlock();
        });

        MoveCamera();
    }

    private void MoveCamera()
    {
        // Move Camera
        float newTargetY = lastBlock.transform.position.y + cameraYOffset;

        Vector3 currentFollowPos = cameraFollowTarget.position;
        Vector3 newFollowPos = new Vector3(currentFollowPos.x, newTargetY, currentFollowPos.z);

        // Smooth move
        DOTween.Kill(cameraFollowTarget); // In case one already exists
        cameraFollowTarget.DOMoveY(newTargetY, 0.3f).SetEase(Ease.OutSine);
    }

    private void DropBlock(Transform block, float targetY, System.Action onComplete)
    {
        //Vector3 targetPosition = block.position;
        //targetPosition.y = lastBlock.transform.position.y + (blockHeight / 2f);

        block.DOMoveY(targetY, 0.3f).SetEase(Ease.OutBounce).OnComplete(() => onComplete?.Invoke());
    }

    private void SliceBlock(GameObject current, GameObject previous, BlockMover.Axis axis)
    {
        Vector3 currPos = current.transform.position;
        Vector3 prevPos = previous.transform.position;

        Vector3 currScale = current.transform.localScale;
        Vector3 prevScale = previous.transform.localScale;

        float delta = axis == BlockMover.Axis.X
            ? currPos.x - prevPos.x
            : currPos.z - prevPos.z;

        float direction = Mathf.Sign(delta);
        float overlap = axis == BlockMover.Axis.X
            ? prevScale.x - Mathf.Abs(delta)
            : prevScale.z - Mathf.Abs(delta);

        // Resize current block
        if (axis == BlockMover.Axis.X)
        {
            current.transform.localScale = new Vector3(overlap, currScale.y, currScale.z);
            float newX = prevPos.x + (delta / 2f);
            current.transform.position = new Vector3(newX, currPos.y, currPos.z);
        }
        else
        {
            current.transform.localScale = new Vector3(currScale.x, currScale.y, overlap);
            float newZ = prevPos.z + (delta / 2f);
            current.transform.position = new Vector3(currPos.x, currPos.y, newZ);
        }

        // Create the falling piece
        float cutSize = axis == BlockMover.Axis.X
            ? currScale.x - overlap
            : currScale.z - overlap;

        Vector3 cutScale = axis == BlockMover.Axis.X
            ? new Vector3(cutSize, currScale.y, currScale.z)
            : new Vector3(currScale.x, currScale.y, cutSize);

        Vector3 cutPos = axis == BlockMover.Axis.X
            ? new Vector3(current.transform.position.x + direction * (overlap / 2f + cutSize / 2f), currPos.y, currPos.z)
            : new Vector3(currPos.x, currPos.y, current.transform.position.z + direction * (overlap / 2f + cutSize / 2f));

        GameObject fallingBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fallingBlock.transform.localScale = cutScale;
        fallingBlock.transform.position = cutPos;
        fallingBlock.GetComponent<Renderer>().material = current.GetComponent<Renderer>().material;

        Rigidbody rb = fallingBlock.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.angularVelocity = Random.insideUnitSphere * 5f;
        Destroy(fallingBlock, 2f); // Auto-cleanup
    }
}
