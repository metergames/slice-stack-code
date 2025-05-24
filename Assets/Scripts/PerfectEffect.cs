using UnityEngine;
using DG.Tweening;

public class PerfectEffect : MonoBehaviour
{
    public float largerThanBlock = 0.35f;
    public float startScale = 0.85f;
    public float scaleAnimationLength = 0.3f;

    public void Play(Vector3 blockScale)
    {
        Material mat = GetComponent<Renderer>().material;

        // Convert world scale (0.35 units larger) to local plane scale
        Vector3 finalScale = new Vector3(
            (blockScale.x + largerThanBlock) * 0.1f,
            1f,
            (blockScale.z + largerThanBlock) * 0.1f
        );

        // Start small
        transform.localScale = finalScale * startScale;

        Sequence seq = DOTween.Sequence();

        // Scale up smoothly
        seq.Append(transform.DOScale(finalScale, scaleAnimationLength).SetEase(Ease.OutBack));

        // Then scale back down slightly
        seq.Append(transform.DOScale(new Vector3(
            finalScale.x * 0.4f,
            1f,
            finalScale.z * 0.6f
        ), scaleAnimationLength).SetEase(Ease.InOutSine));


        // Destroy after the full animation
        seq.OnComplete(() => Destroy(gameObject));
    }
}
