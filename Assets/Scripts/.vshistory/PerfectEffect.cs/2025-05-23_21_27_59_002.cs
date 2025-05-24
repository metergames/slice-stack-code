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

        transform.localScale = finalScale * 0.85f; // Start small

        Color startColor = mat.color;
        startColor.a = 1f;
        mat.color = startColor;

        Sequence s = DOTween.Sequence();

        // Step 1: Scale up with bounce
        s.Append(transform.DOScale(finalScale, 0.35f).SetEase(Ease.OutBack));

        // Step 2: Scale slightly up + fade out together
        s.AppendCallback(() =>
        {
            // Optional tiny puff before fade
            transform.DOScale(finalScale * 1.05f, 0.3f).SetEase(Ease.InOutQuad);
        });

        s.Join(DOTween.To(() => mat.color.a, a =>
        {
            Color c = mat.color;
            c.a = a;
            mat.color = c;
        }, 0f, 3.4f).SetEase(Ease.OutSine));

        s.OnComplete(() => Destroy(gameObject));
    }
}
