using UnityEngine;

public class FallingBlock : MonoBehaviour
{
    public float killY = -15f;

    void Update()
    {
        if (transform.position.y < killY)
            Destroy(gameObject);
    }
}
