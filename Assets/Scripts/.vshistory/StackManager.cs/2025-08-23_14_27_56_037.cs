using DG.Tweening;
using System.Collections.Generic;
using Unity.Cinemachine;
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
    public CinemachineCamera cineCam;
    public GameUIManager uiManager;
    public Material gradientMaterial;
    public GameObject perfectEffectPrefab;
    public AudioManager audioManager;
    public SettingsManager settingsManager;

    private GameObject lastBlock;
    private BlockMover.Axis currentAxis = BlockMover.Axis.X;
    private List<GameObject> stackBlocks = new List<GameObject>();
    private bool gameOver = false;
    private int score = 0;
    private bool gameStarted = false;
    private float initialOrthoSize;
    private bool blockIsDropping = false;
    private int perfectStreak = 0;

    // Colors
    private float hue = 0f;
    private float saturation = 0.6f;
    private float lightness = 0.55f;

    private void Start()
    {
        int savedTopScore = PlayerPrefs.GetInt("TopScore", 0);
        uiManager.SetTopScore(savedTopScore);
        uiManager.ShowStartUI();

        initialOrthoSize = cineCam.Lens.OrthographicSize;

        hue = Random.Range(0f, 360f);

        SpawnFirstBlock();
        //SpawnNextBlock();

        if (SettingsManager.IsMusicEnabled())
            audioManager.PlayMusic();

        PlayerPrefs.SetInt("ReadyToPlay", 1);
    }

    private void Update()
    {
        if (UIUtils.IsPointerOverUIButton() || PlayerPrefs.GetInt("ReadyToPlay") == 0)
        {
            Debug.Log(PlayerPrefs.GetInt("ReadyToPlay"));
            return;

        }

        if (!gameStarted && !blockIsDropping && !settingsManager.IsSettingsOpen() && Input.GetMouseButtonDown(0))
        {
            Debug.Log(PlayerPrefs.GetInt("ReadyToPlay"));
            gameStarted = true;
            score = 0;
            uiManager.AnimateStartUIOut();
            uiManager.AnimateScoreIn();
            uiManager.AnimateCoinsIn();
            uiManager.UpdateScore(score);

            audioManager.PlayUISound();

            SpawnNextBlock(); // Kick off the game
            return;
        }

        if (!gameOver && gameStarted && !blockIsDropping && Input.GetMouseButtonDown(0))
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

        hue += 8f;
        hue = Mathf.Repeat(hue, 360f);

        Color blockColor = ColorUtils.ColorFromHSL(hue, saturation, lightness);
        SetBlockColor(newBlock, blockColor);

        // Adjust background to be vibrant but contrasting
        float skyHue = Mathf.Repeat(hue + 45f, 360f); // not full complement—just a nice offset
        float skySaturation = Mathf.Clamp01(saturation + 0.2f);
        float skyLightness = Mathf.Clamp01(lightness + 0.2f);

        Color skyTop = ColorUtils.ColorFromHSL(skyHue, skySaturation, skyLightness);
        Color skyBottom = ColorUtils.ColorFromHSL(skyHue, skySaturation * 0.8f, skyLightness * 0.8f);

        // Apply to material with tween
        gradientMaterial.DOColor(skyTop, "_TopColor", 0.6f);
        gradientMaterial.DOColor(skyBottom, "_BottomColor", 0.6f);

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

        blockIsDropping = true;

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

            bool newHigh = uiManager.SaveTopScoreIfNeeded(score);
            if (newHigh)
                uiManager.ShowNewHighScore();

            uiManager.ShowResetButton();
            uiManager.ShowGameOverText();

            Time.timeScale = 0.4f;

            TriggerGameOverEffects();
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

            perfectStreak++;
            CurrencyManager.Instance.AddCoins(perfectStreak >= 3 ? 3 : 1);

            score++;
            uiManager.UpdateScore(score);
            uiManager.AnimateScorePopup(true);

            DropBlock(currentBlock, targetY, () =>
            {
                audioManager.PlaySFX(audioManager.perfectClip);
                VibratePerfect();
                stackBlocks.Add(currentBlock.gameObject);
                SpawnNextBlock();
                blockIsDropping = false;
                SpawnPerfectEffect(currentBlock);
            });

            MoveCamera();
            return;
        }

        float currHeight = currentBlock.localScale.y;
        float prevHeight = previousBlock.localScale.y;

        float finalY = previousBlock.position.y + (prevHeight / 2f) + (currHeight / 2f);

        perfectStreak = 0;
        CurrencyManager.Instance.AddCoins(1);

        score++;
        uiManager.UpdateScore(score);
        uiManager.AnimateScorePopup();

        DropBlock(currentBlock, finalY, () =>
        {
            SliceBlock(currentBlock.gameObject, previousBlock.gameObject, currentAxis == BlockMover.Axis.X ? BlockMover.Axis.Z : BlockMover.Axis.X);

            //stackBlocks.Add(currentBlock.gameObject);
            SpawnNextBlock();
            blockIsDropping = false;
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
        cameraFollowTarget.DOMoveY(newTargetY, 0.4f).SetEase(Ease.OutSine);
    }

    private void DropBlock(Transform block, float targetY, System.Action onComplete)
    {
        //Vector3 targetPosition = block.position;
        //targetPosition.y = lastBlock.transform.position.y + (blockHeight / 2f);

        block.DOMoveY(targetY, 0.4f).SetEase(Ease.OutBounce).OnComplete(() => onComplete?.Invoke());
    }

    private void SliceBlock(GameObject current, GameObject previous, BlockMover.Axis axis)
    {
        audioManager.PlaySFX(audioManager.sliceClip);

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
        fallingBlock.name = "SlicedCube";
        fallingBlock.transform.localScale = cutScale;
        fallingBlock.transform.position = cutPos;
        fallingBlock.GetComponent<Renderer>().material = current.GetComponent<Renderer>().material;

        Rigidbody rb = fallingBlock.AddComponent<Rigidbody>();
        rb.mass = 0.5f;
        rb.angularVelocity = Random.insideUnitSphere * 3f;

        // Smart cleanup logic
        FallingBlock fb = fallingBlock.AddComponent<FallingBlock>();
        fb.Init(cameraFollowTarget);
        //Destroy(fallingBlock, 2f); // Auto-cleanup
    }

    private void TriggerGameOverEffects()
    {
        if (cameraFollowTarget == null || cineCam == null) return;

        audioManager.PlaySFX(audioManager.failClip);

        VibrateGameOver();

        DOTween.Kill(cameraFollowTarget);

        Vector3 camPos = cameraFollowTarget.position;
        Vector3 zoomOutTargetPos = camPos + new Vector3(0f, -3f, 0f); // just move down

        // Animate follow target position
        cameraFollowTarget.DOMove(zoomOutTargetPos, 0.4f).SetEase(Ease.OutSine);

        // Animate zoom out via FOV (orthographic camera)
        float targetSize = initialOrthoSize + 3f;

        DOTween.To(() => cineCam.Lens.OrthographicSize, x => cineCam.Lens.OrthographicSize = x, targetSize, 0.4f)
            .SetEase(Ease.OutSine);

        // Camera sway after zoom-out
        Vector3 swayTarget = zoomOutTargetPos + new Vector3(1f, 0f, 0f);
        cameraFollowTarget.DOMove(swayTarget, 3.2f)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    public void ResetGame()
    {
        uiManager.AnimateFontSoftness(0f, 1f, 0.5f); // Blur out text as screen fades to black

        uiManager.FadeToBlack(() =>
        {
            // Destroy all blocks except the base block
            for (int i = 1; i < stackBlocks.Count; i++)
            {
                Destroy(stackBlocks[i]);
            }

            // Keep only the base block
            GameObject baseBlock = stackBlocks[0];
            stackBlocks.Clear();
            stackBlocks.Add(baseBlock);

            // Clear "SlicedCube"s
            GameObject[] allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
            foreach (GameObject go in allObjects)
            {
                if (go.name == "SlicedCube")
                    Destroy(go);
            }

            // Reset references
            lastBlock = baseBlock;

            // Reset game state
            score = 0;
            gameOver = false;
            gameStarted = false;
            currentAxis = BlockMover.Axis.X;

            uiManager.SetTopScore(PlayerPrefs.GetInt("TopScore", 0));
            uiManager.ShowStartUI();
            //uiManager.UpdateScore(score);
            uiManager.HideNewHighScore();
            uiManager.HideResetButton();
            uiManager.HideGameOverText();

            // Immediately kill any ongoing tweens for the camera
            DOTween.Kill(cameraFollowTarget);

            // Reset camera position
            Vector3 camResetPos = lastBlock.transform.position + new Vector3(0f, 2f, 0f);
            cameraFollowTarget.position = camResetPos;
            cineCam.Lens.OrthographicSize = initialOrthoSize;

            // Reset score label position
            RectTransform rtScore = uiManager.scoreText.rectTransform;
            rtScore.anchoredPosition = new Vector2(0, -565); // Default position

            // Reset coins label position
            RectTransform rtCoins = uiManager.coinsText.rectTransform;
            rtCoins.anchoredPosition = new Vector2(345, -700); // Default position

            blockIsDropping = false;

            Time.timeScale = 1;

            uiManager.AnimateFontSoftness(1f, 0f, 0.5f); // Sharpen text as screen fades back in

            uiManager.FadeFromBlack();
        });
    }

    private void SetBlockColor(GameObject block, Color color)
    {
        Renderer renderer = block.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.material = new Material(renderer.material); // Clone material to avoid affecting others
            renderer.material.color = color;
        }
    }

    private void SpawnPerfectEffect(Transform block)
    {
        Vector3 spawnPos = block.position + Vector3.down * 0.5f; // 1.5f lower

        GameObject fx = Instantiate(perfectEffectPrefab, spawnPos, Quaternion.Euler(0f, 0f, 0f)); // Rotate so plane faces up
        fx.GetComponent<PerfectEffect>().Play(block.localScale);
    }

    public static void VibratePerfect()
    {
        if (SettingsManager.IsVibrationEnabled())
            VibrationManager.Vibrate(40, 70);
    }

    public static void VibrateGameOver()
    {
        if (SettingsManager.IsVibrationEnabled())
            VibrationManager.Vibrate(200, 125);
    }

    public static void VibrateClick()
    {
        if (SettingsManager.IsVibrationEnabled())
            VibrationManager.Vibrate(20, 50);
    }
}
