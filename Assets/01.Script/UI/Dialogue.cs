using DG.Tweening;
using UnityEngine;

public class Dialogue : MonoBehaviour
{
    private float duration = 0.7f;    // 애니메이션 시간
    [SerializeField] private Ease ease = Ease.OutBack; // 뽀용 느낌 이징
    public float YScale;
    private Vector3 _originPos;

    private void Awake()
    {
        _originPos = transform.localPosition;
    }

    private void OnEnable()
    {
        // 혹시 이전 트윈 남아있으면 정리
        transform.DOKill();

        float height = YScale * 1.2f;
        // 원래 위치까지 뽀용~
        transform.DOScaleY(height, duration).SetEase(ease).OnComplete(() =>
        {
            transform.DOScaleY(YScale, 0.3f).SetEase(ease);
        });
    }
}
