using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CaretakerActor : SequenceActorBase
{
    private float hitDuration = 0.6f;
    [SerializeField] private float waterDuration = 1f;
    private float angryDuration = 0.5f;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private Transform goPos;
    [SerializeField] private Transform attackPos;
    private Vector3 originPos;
    public const string FLAG_CLEAR_ENABLED = "ClearEnabledByPigeon"; // 이후에 할아버지가 있는지
    protected override void Awake()
    {
        base.Awake();
        originPos = transform.position;
    }
    public override IEnumerator Execute(SequenceContext ctx)
    {
        int idxPigeon = ctx.GetIndex(ESequenceCharacter.Pigeon);
        int idxCaretaker = ctx.GetIndex(ESequenceCharacter.Caretaker);

        if (idxPigeon == -1)
        {
            // 비둘기가 아예 안 나온 경우 기본 행동
            Debug.Log("관리인: 비둘기가 없으니 기본 행동 (물 뿌리기)");
            // TODO: 물 뿌리기 애니
            Animator.Play("Gardner-Idle");
            yield return new WaitForSeconds(waterDuration);
            yield break;
        }

        if (idxCaretaker > idxPigeon)
        {
            // 비둘기보다 늦게 실행 → 화단에 물 뿌리기
            Debug.Log("관리인: 비둘기보다 늦게 → 똥 맞아서 화냄");
            Animator.Play("Gardner-Angry");
            yield return new WaitForSeconds(hitDuration);
            transform.DOMove(attackPos.position, 0.2f);

            yield return new WaitForSeconds(angryDuration/2);
            sequenceManager.ChangeAnim(ESequenceCharacter.Pigeon, "Hit");
            yield return new WaitForSeconds(angryDuration/2);
            Animator.Play("Gardner-Idle");

            Debug.Log("비둘기: 물 맞음");
            sequenceManager.ChangeAnim(ESequenceCharacter.Pigeon,"FlyingPark",1);
            yield return new WaitForSeconds(3f);
            ctx.SetFlag(FLAG_CLEAR_ENABLED);
        }
        else if (idxCaretaker < idxPigeon)
        {
            // 비둘기보다 먼저 실행 → 똥 피하기
            Animator.Play("Gardner-Move");
            Debug.Log("관리인: 비둘기보다 먼저 → Move");
            transform.DOMove(goPos.position,moveDuration/2).SetEase(Ease.InQuad);
            yield return new WaitForSeconds(moveDuration);
            Animator.Play("Gardner-Idle");
        }
        bool hasNextCard = ctx.HasFlag(BicycleActor.FLAG_GONE);
        if (hasNextCard)
        {
            Debug.Log("자전거: 돌아오기");
            sequenceManager.ChangeAnim(ESequenceCharacter.Bicycle, "Comeback", 1);
        }
    }
   
    public override IEnumerator Rewind(SequenceContext ctx)
    {
        transform.DOKill(false);
        transform.DOMove(originPos, 1);
        Animator.Play("Gardner-Idle");
        yield break;
    }
}
