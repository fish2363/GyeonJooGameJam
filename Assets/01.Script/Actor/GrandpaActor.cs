using System.Collections;
using UnityEngine;

public class GrandpaActor : SequenceActorBase
{
    private float snackDuration = 2f;
    private float pigeonSnackDur = 3f;
    [SerializeField] private float newspaperDuration = 1f;

    // 할아버지가 과자 주기까지 성공했다는 플래그
    public const string FLAG_GRANDPA_SNACK_DONE = "GrandpaSnackDone";

    public override IEnumerator Execute(SequenceContext ctx)
    {
        bool clearEnabled = ctx.HasFlag(CaretakerActor.FLAG_CLEAR_ENABLED);

        if (clearEnabled)
        {
            Debug.Log("할아버지: 날아다니는 비둘기에게 과자 주기");
            // TODO: 과자 주기 애니/타임라인
            Animator.Play("GrandFather-Feed");
            yield return new WaitForSeconds(snackDuration);

            Debug.Log("비둘기: 과자 먹으러 내려오기");
            // TODO: 내려오는 애니/타임라인
            sequenceManager.ChangeAnim(ESequenceCharacter.Pigeon, "FlyingPark", 2);

            ctx.SetFlag(FLAG_GRANDPA_SNACK_DONE);
        }
        else
        {
            Debug.Log("할아버지: 조건 안 맞음 → 신문 보기");
            // TODO: 신문 보기 애니
            Animator.Play("GrandFather-OpenPaper");
            yield return new WaitForSeconds(newspaperDuration);
        }
    }

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        // 필요시 되감기
        Animator.Play("GrandFather-Idle");
        yield break;
    }
}
