using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
public class MouseCarryVisual : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("호버 연출 설정")]
    [SerializeField] private float hoverScaleUp = 1.08f;  // 얼마나 커질지
    [SerializeField] private float wobbleAngle = 4f;     // 좌우로 몇 도 흔들릴지
    [SerializeField] private float wobbleDuration = 0.15f;  // 좌우 한 번 갔다 오는 시간

    private RectTransform _rect;
    private Vector3 _HoverScale;
    private Vector3 _defaultScale;
    private Quaternion _defaultRotation;

    private Transform _originalParent;
    private int _originalSiblingIndex;

    private Tween _hoverTween;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();

        _defaultScale = _rect.localScale;
        _HoverScale = new Vector3(4.3f, 4.3f, 4.3f);
        _defaultRotation = _rect.localRotation;

        _originalParent = _rect.parent;
        _originalSiblingIndex = _rect.GetSiblingIndex();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        EndHover();
    }

    private void StartHover()
    {
        // 기존 트윈 정리
        if (_hoverTween != null && _hoverTween.IsActive())
            _hoverTween.Kill();
        _rect.DOKill();

        if (_originalParent != null && _originalParent.parent != null)
        {
            // worldPositionStays = true → 화면 위치 안 튀게
            _rect.SetParent(_originalParent.parent, worldPositionStays: true);
            _rect.SetAsLastSibling(); // 맨 위로
        }

        // 기준 상태에서 시작
        _rect.localScale = _HoverScale;
        _rect.localRotation = _defaultRotation;

        var targetScale = _HoverScale * hoverScaleUp;

        var seq = DOTween.Sequence();

        // 살짝 커지기
        seq.Join(
            _rect.DOScale(targetScale, 0.08f)
                 .SetEase(Ease.OutQuad)
        );

        // 좌우 살랑살랑 무한 반복
        seq.Join(
            _rect
                .DOLocalRotate(new Vector3(0f, 0f, wobbleAngle), wobbleDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
        );

        _hoverTween = seq;
    }

    private void EndHover()
    {
        // 흔들림 멈추기
        if (_hoverTween != null && _hoverTween.IsActive())
            _hoverTween.Kill();
        _hoverTween = null;

        _rect.DOKill();

        // 원래 크기/각도로 복귀
        _rect.DOScale(_defaultScale, 0.1f).SetEase(Ease.OutQuad);
        _rect.DOLocalRotateQuaternion(_defaultRotation, 0.1f).SetEase(Ease.OutQuad);

        if (_originalParent != null)
        {
            _rect.SetParent(_originalParent, worldPositionStays: true);
            _rect.SetSiblingIndex(_originalSiblingIndex);
        }
    }
}
