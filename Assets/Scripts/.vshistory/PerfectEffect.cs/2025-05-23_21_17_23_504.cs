using UnityEngine;
using DG.Tweening;

public class PerfectEffect : MonoBehaviour
{
    private Material mat;
    private Vector3 originalScale;

    private void Awake()
    {
        mat = GetComponent<Renderer>().material;
        originalScale = transform.localScale;
    }

    public void Play()
    {
        transform.localScale = originalScale;
        Color c = mat.color;
        c.a = 1f;
        mat.color = c;

        Sequence s = DOTween.Sequence();
        s.Append(transform.DOScale(originalScale * 1.2f, 0.3f).SetEase(Ease.OutSine));
        s.Join(DOTween.To(() => mat.color.a, a => {
            Color newColor = mat.color;
            newColor.a = a;
            mat.color = newColor;
        }, 0f, 0.3f));
        s.OnComplete(() => Destroy(gameObject));
    }
}
