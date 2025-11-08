using System.Collections;
using UnityEngine;

public class Trofical : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip[] clips;   // 0,1,2 순서대로 넣기
    [SerializeField] private float extraDelay = 0.1f; // 각 클립 끝나고 약간 텀

    private Coroutine _routine;
    private bool isCurrentState;

    public void ChangeLight(bool isGreen)
    {
        if (animator == null || clips == null || clips.Length == 0)
        {
            Debug.LogWarning("TrafficLight: Animator나 Clips가 비어있음");
            return;
        }
        if (isGreen == isCurrentState)
            return;
        else
            isCurrentState = isGreen;

        // 전에 돌던 시퀀스 있으면 중단
        if (_routine != null)
            StopCoroutine(_routine);
        
        _routine = StartCoroutine(ChangeLightRoutine(isGreen));
    }

    private IEnumerator ChangeLightRoutine(bool isGreen)
    {
        if (isGreen)
        {
            // 0 -> 1 -> 2
            for (int i = 0; i < clips.Length; i++)
            {
                PlayClip(clips[i]);
                yield return new WaitForSeconds(clips[i].length + extraDelay);
            }
        }
        else
        {
            // 2 -> 1 -> 0
            for (int i = clips.Length - 1; i >= 0; i--)
            {
                PlayClip(clips[i]);
                yield return new WaitForSeconds(clips[i].length + extraDelay);
            }
        }

        _routine = null;
    }

    private void PlayClip(AnimationClip clip)
    {
        if (clip == null) return;

        animator.Play(clip.name, 0, 0f);
    }
}
