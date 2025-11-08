using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private bool useLocalAxes = true; // 카메라 바라보는 방향 기준으로 이동할지

    [SerializeField] private float verticalSpeed = 5f; // 사실 moveSpeed랑 같게 써도 됨

    [Header("이동 제한 설정")]
    [SerializeField] private Vector3 minBounds = new Vector3(-10f, 1f, -10f);
    [SerializeField] private Vector3 maxBounds = new Vector3(10f, 5f, 10f);

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        Vector3 dir = Vector3.zero;

        // WASD 평면 이동
        if (Keyboard.current.wKey.isPressed) dir += Vector3.up;
        if (Keyboard.current.sKey.isPressed) dir += Vector3.down;
        if (Keyboard.current.aKey.isPressed) dir += Vector3.left;
        if (Keyboard.current.dKey.isPressed) dir += Vector3.right;


        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        float speed = moveSpeed;
        if (Keyboard.current.leftShiftKey.isPressed)
            speed *= sprintMultiplier;

        Vector3 delta = dir * speed * Time.deltaTime;

        if (useLocalAxes)
        {
            // 카메라 기준 방향으로 이동
            transform.Translate(delta, Space.Self);
        }
        else
        {
            // 월드 기준 축으로 이동
            transform.Translate(delta, Space.World);
        }

       
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minBounds.x, maxBounds.x);
        pos.y = Mathf.Clamp(pos.y, minBounds.y, maxBounds.y);
        pos.z = Mathf.Clamp(pos.z, minBounds.z, maxBounds.z);
        transform.position = pos;
    }

    // 에디터에서 영역 보이게 하고 싶으면 (선택)
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 center = (minBounds + maxBounds) * 0.5f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}
