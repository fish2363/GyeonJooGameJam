using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using Unity.Cinemachine;

public class MouseTextManager : MonoBehaviour
{
    public static MouseTextManager Instance { get; private set; }

    [Header("필수 설정")]
    [SerializeField] private Canvas targetCanvas;        
    [SerializeField] private MouseTextEffect textPrefab; 

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnTextAtMouse(string message)
    {
        if (targetCanvas == null || textPrefab == null)
        {
            return;
        }

        Vector2 screenPos = GetPointerScreenPosition();

        MouseTextEffect inst = Instantiate(textPrefab, targetCanvas.transform);
        RectTransform rt = inst.GetComponent<RectTransform>();

        if (targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            rt.position = screenPos;
        }
        else
        {
            Vector2 localPos;
            RectTransform canvasRect = targetCanvas.transform as RectTransform;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPos,
                targetCanvas.worldCamera,
                out localPos
            );

            rt.anchoredPosition = localPos;
        }

        inst.Play(message);
        Camera.main.GetComponent<CinemachineImpulseSource>().GenerateImpulse();
    }

    private Vector2 GetPointerScreenPosition()
    {
        if (Mouse.current != null)
        {
            return Mouse.current.position.ReadValue();
        }

        return Vector2.zero;
    }
}
