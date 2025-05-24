using UnityEngine;
using DG.Tweening;

public class PerfectEffect : MonoBehaviour
{
    private Material mat;

    public void Play(Vector3 blockScale)
    {
        mat = GetComponent<Renderer>().material;

        // Convert 0.35 world units into local scale (because plane is 10x)
        float addedScale = 0.035f;
        Vector3 finalScale = new Vector3(
            (blockScale.x + 0.35f) * 0.1f,
            1f,
            (blockScale.z + 0.35f) * 0.1f
        );

        transform.localScale = finalScale * 0.7f; // Start small

        Color startColor = mat.color;
        startColor.a = 1f;
        mat.color = startColor;

        Sequence s = DOTween.Sequence();

        // Scale up smoothly
        s.Append(transform.DOScale(finalScale, 0.4f).SetEase(Ease.OutBack));

        // Fade out AFTER scale starts
        s.Join(DOTween.To(() => mat.color.a, a => {
            Color c = mat.color;
            c.a = a;
            mat.color = c;
        }, 0f, 0.5f).SetEase(Ease.InQuad).SetDelay(0.1f));

        s.OnComplete(() => Destroy(gameObject));
    }
}
