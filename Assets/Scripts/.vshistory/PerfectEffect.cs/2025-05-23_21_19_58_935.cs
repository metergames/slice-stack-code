using UnityEngine;
using DG.Tweening;

public class PerfectEffect : MonoBehaviour
{
    private Material mat;
    private Vector3 startScale;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        startScale = transform.localScale;
    }

    public void Play(Vector3 blockScale)
    {
        // Convert world size increase (0.35) to local plane scale
        float addedScale = 0.035f; // Since 1 = 10 world units for Plane

        Vector3 targetScale = new Vector3(
            (blockScale.x + 0.35f) * 0.1f, // Convert world to local
            1f,
            (blockScale.z + 0.35f) * 0.1f
        );

        transform.localScale = targetScale;

        Color c = mat.color;
        c.a = 1f;
        mat.color = c;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(targetScale * 1.1f, 0.3f).SetEase(Ease.OutQuad));
        s.Join(DOTween.To(() => mat.color.a, a => {
            Color newColor = mat.color;
            newColor.a = a;
            mat.color = newColor;
        }, 0f, 0.3f));
        s.OnComplete(() => Destroy(gameObject));
    }
}
