using UnityEngine;
using DG.Tweening;

public class PerfectEffect : MonoBehaviour
{
    public void Play(Vector3 blockScale)
    {
        Material mat = GetComponent<Renderer>().material;

        // Convert world scale (0.35 units larger) to local plane scale
        Vector3 finalScale = new Vector3(
            (blockScale.x + 0.35f) * 0.1f,
            1f,
            (blockScale.z + 0.35f) * 0.1f
        );

        // Start small
        transform.localScale = finalScale * 0.7f;

        Sequence seq = DOTween.Sequence();

        // Scale up smoothly
        seq.Append(transform.DOScale(finalScale, 0.3f).SetEase(Ease.OutBack));

        // Then scale back down slightly
        seq.Append(transform.DOScale(finalScale * 0.5f, 0.3f).SetEase(Ease.InOutQuad));

        // Destroy after the full animation
        seq.OnComplete(() => Destroy(gameObject));
    }
}
