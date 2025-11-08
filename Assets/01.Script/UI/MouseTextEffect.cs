using UnityEngine;
using TMPro;
using DG.Tweening;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(TextMeshProUGUI))]
public class MouseTextEffect : MonoBehaviour
{
    [Header("애니메이션 설정")]
    [SerializeField] private float popDuration = 0.15f;      // 뽀용 등장 시간
    [SerializeField] private float settleDuration = 0.1f;    // 1.1 -> 1.0 정리 시간
    [SerializeField] private float moveUpDuration = 0.35f;   // 위로 올라가는 시간
    [SerializeField] private float fallDuration = 0.4f;      // 내려오면서 회전/페이드 시간

    [SerializeField] private float upDistance = 80f;         // 위로 올라가는 거리
    [SerializeField] private float fallDownDistance = 40f;   // 다시 내려오는 거리
    [SerializeField] private float fallRightDistance = 30f;  // 오른쪽으로 이동 거리
    [SerializeField] private float endRotationZ = 15f;       // 오른쪽으로 기울어지는 각도

    private RectTransform rectTransform;
    private TextMeshProUGUI tmp;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tmp = GetComponent<TextMeshProUGUI>();
    }

    public void Play(string message)
    {
        tmp.text = message;

        // 초기 상태 세팅
        rectTransform.localScale = Vector3.zero;
        rectTransform.rotation = Quaternion.identity;

        var c = tmp.color;
        c.a = 1f;
        tmp.color = c;

        Vector2 startPos = rectTransform.anchoredPosition;
        Vector2 upPos = startPos + new Vector2(0f, upDistance);
        Vector2 fallPos = upPos + new Vector2(fallRightDistance, -fallDownDistance);

        // DOTween 시퀀스
        Sequence seq = DOTween.Sequence();

        // 1. 뽀용! (스케일 0 -> 1.1)
        seq.Append(rectTransform
            .DOScale(1.1f, popDuration)
            .SetEase(Ease.OutBack));

        // 2. 1.1 -> 1.0로 살짝 정리
        seq.Append(rectTransform
            .DOScale(1.0f, settleDuration)
            .SetEase(Ease.InQuad));

        // 3. 위로 쓱 올라가기
        seq.Append(rectTransform
            .DOAnchorPos(upPos, moveUpDuration)
            .SetEase(Ease.OutQuad));
        // 위치 이동과 동시에 회전 + 페이드
        seq.Join(rectTransform
            .DORotate(new Vector3(0f, 0f, -endRotationZ), fallDuration) // 오른쪽 기울어지게 -Z
            .SetEase(Ease.OutQuad));

        // 4. 오른쪽으로 살짝 회전하면서 내려오고 페이드 아웃
        seq.Append(rectTransform
            .DOAnchorPos(fallPos, fallDuration)
            .SetEase(Ease.InQuad));


        seq.Join(tmp
            .DOFade(0f, fallDuration)
            .SetEase(Ease.OutQuad));

        // 끝나면 삭제
        seq.OnComplete(() => Destroy(gameObject));
    }
}
