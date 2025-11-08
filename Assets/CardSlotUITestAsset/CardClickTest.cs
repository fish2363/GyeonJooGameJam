using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))] // BoxCollider2D 등 아무거나 OK
public class CardClickTest : MonoBehaviour
{
    [Header("중앙 숫자 오브젝트(켜고 끌 대상)")]
    [SerializeField] private GameObject numberRoot; // 중앙 숫자(텍스트/스프라이트) GameObject

    [Header("선택 시 카드 색상")]
    [SerializeField] private Color32 selectedColor = new Color32(0x60, 0x60, 0x60, 0xFF);

    [SerializeField] private bool startSelected = false;

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isSelected;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        Apply(startSelected, instant: true);
    }

    // 2D 오브젝트 클릭용(카메라가 오브젝트를 레이캐스트할 수 있게 Collider2D 필요)
    private void OnMouseDown()
    {
        Apply(!isSelected);
    }

    public void Apply(bool selected, bool instant = false)
    {
        isSelected = selected;

        // 색 전환
        sr.color = isSelected ? (Color)selectedColor : originalColor;

        // 중앙 숫자 on/off
        if (numberRoot) numberRoot.SetActive(isSelected);
    }

    // 외부에서 호출하고 싶을 때(선택 토글)
    public void Toggle() => Apply(!isSelected);
}
