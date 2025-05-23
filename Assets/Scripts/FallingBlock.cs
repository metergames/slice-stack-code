using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    private Transform cameraTransform;
    private float yOffset = 25f;

    public void Init(Transform cam)
    {
        cameraTransform = cam;
    }

    void Update()
    {
        if (cameraTransform == null) return;

        if (transform.position.y < cameraTransform.position.y - yOffset)
            Destroy(gameObject);
    }
}
