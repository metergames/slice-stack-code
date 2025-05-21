using UnityEngine;
using DG.Tweening;

public class BlockMover : MonoBehaviour
{
    public float moveDistance = 3f;
    public float moveDuration = 1.5f;
    public Axis moveAxis = Axis.X;

    private Tween moveTween;

    public enum Axis { X, Z };

    private void Start()
    {
        StartMovement();
    }

    public void StartMovement()
    {
        Vector3 endOffset;

        if (moveAxis == Axis.X)
            endOffset = Vector3.right * (moveDistance / 2f);
        else endOffset = Vector3.forward * (moveDistance / 2f);

        Vector3 startPos = transform.position - endOffset;
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
