using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // 새 Input System
#endif

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Collider2D))]
public class CardClickTest : MonoBehaviour
{
    [Header("중앙 숫자 오브젝트")]
    [SerializeField] private GameObject numberRoot;

    [Header("선택 시 카드 색상 (#606060)")]
    [SerializeField] private Color32 selectedColor = new Color32(0x60, 0x60, 0x60, 0xFF);

    [SerializeField] private bool startSelected = false;
    [SerializeField] private bool ignoreUI = true; // UI 위 클릭 무시

    private SpriteRenderer sr;
    private Color originalColor;
    private bool isSelected;
    private Collider2D myCol;
    private Camera cam;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        myCol = GetComponent<Collider2D>();
        cam = Camera.main;
        originalColor = sr.color;
        Apply(startSelected);
    }

    void Update()
    {
        Vector2 screenPos;
        if (!WasPointerPressedThisFrame(out screenPos))
            return;

        // (선택) UI 위는 무시
        if (ignoreUI && EventSystem.current != null)
        {
#if ENABLE_INPUT_SYSTEM
            // 새 입력 시스템에서 마우스 포인터 ID는 보통 -1
            if (EventSystem.current.IsPointerOverGameObject(-1))
                return;
#else
            if (EventSystem.current.IsPointerOverGameObject())
                return;
#endif
        }

        var world = cam != null ? cam.ScreenToWorldPoint(screenPos) : (Vector3)screenPos;
        Vector2 p = new Vector2(world.x, world.y);

        if (myCol == Physics2D.OverlapPoint(p))
            Toggle();
    }

    bool WasPointerPressedThisFrame(out Vector2 pos)
    {
        pos = default;

#if ENABLE_INPUT_SYSTEM
        // 마우스
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }
        // 터치(모바일/에디터 시뮬레이션 가능)
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.wasPressedThisFrame)
            {
                pos = t.position.ReadValue();
                return true;
            }
        }
        return false;
#else
        if (Input.GetMouseButtonDown(0))
        {
            pos = Input.mousePosition;
            return true;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            pos = Input.GetTouch(0).position;
            return true;
        }
        return false;
#endif
    }

    public void Toggle() => Apply(!isSelected);

    public void Apply(bool selected)
    {
        isSelected = selected;
        sr.color = isSelected ? (Color)selectedColor : originalColor;
        if (numberRoot) numberRoot.SetActive(isSelected);
    }
}
