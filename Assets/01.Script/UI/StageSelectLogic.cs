using UnityEngine;
using UnityEngine.InputSystem;

public class StageSelectLogic : MonoBehaviour
{
    [Header("UI 카드들")]
    [SerializeField] private RectTransform leftCard;   // 왼쪽 카드
    [SerializeField] private RectTransform rightCard;  // 오른쪽 카드

    [Header("설정")]
    [SerializeField] private float margin = 100f;          // 화면 중앙 기준 여백 (px)
    private Vector3 normalScale;         // 기본 크기
    private Vector3 focusedScale; // 커졌을 때 크기
    [SerializeField] private float scaleLerpSpeed = 10f;   // 부드럽게 따라가는 속도

    private void OnEnable()
    {
        normalScale = leftCard.localScale;
        focusedScale = normalScale * 1.1f;
    }

    private void Update()
    {
        // 마우스가 아예 없을 수도 있으니까 방어 코드
        if (Mouse.current == null)
            return;

        float mouseX = Mouse.current.position.ReadValue().x;

        float centerX = Screen.width * 0.5f;

        Vector3 leftTargetScale = normalScale;
        Vector3 rightTargetScale = normalScale;

        if (mouseX < centerX - margin)
        {
            leftTargetScale = focusedScale;
        }
        else if (mouseX > centerX + margin)
        {
            rightTargetScale = focusedScale;
        }

        leftCard.localScale = Vector3.Lerp(leftCard.localScale, leftTargetScale, Time.deltaTime * scaleLerpSpeed);
        rightCard.localScale = Vector3.Lerp(rightCard.localScale, rightTargetScale, Time.deltaTime * scaleLerpSpeed);
    }
}
