using UnityEngine;
using DG.Tweening;

public class BlockMover : MonoBehaviour
{
    public float moveDistance = 10f;
    public float moveDuration = 1.5f;
    public Axis moveAxis = Axis.X;

    private Tween moveTween;

    public enum Axis { X, Z };

    private void Start()
    {
        //StartMovement();
    }

    public void StartMovement()
    {
        Vector3 startOffset;
        Vector3 endOffset;

        if (moveAxis == Axis.X)
        {
            startOffset = Vector3.left * (moveDistance / 2f);
            endOffset = Vector3.right * (moveDistance / 2f);
        }
        else
        {
            startOffset = Vector3.forward * (moveDistance / 2f);
            endOffset = Vector3.back * (moveDistance / 2f);
        }

        Vector3 startPos = transform.position + startOffset;
        Vector3 endPos = transform.position + endOffset;

        transform.position = startPos;

        //if (moveAxis == Axis.X)
        //    endPos = startPos + Vector3.right * moveDistance;
        //else endPos = startPos + Vector3.forward * moveDistance;

        moveTween = transform.DOMove(endPos, moveDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }

    public void StopMovement()
    {
        if (moveTween != null && moveTween.IsPlaying())
            moveTween.Kill();
    }
}
