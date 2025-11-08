using System.Collections;
using UnityEngine;

public class IceCreamActor : SequenceActorBase
{
    private float maketingDuration = 1f;

    public override IEnumerator Execute(SequenceContext ctx)
    {
        Debug.Log("아줌마 : 홍보");

        Animator.Play("Maketer");
        yield return new WaitForSeconds(maketingDuration);

        bool AjucyGone = ctx.HasFlag(AjucyActor.FLAG_GoingRoad);
        if(!AjucyGone)
        {
            Debug.Log("아저씨 : 아이스크림 발견");
            sequenceManager.ChangeAnim(ESequenceCharacter.AJucy, "BackIdle");
            yield return new WaitForSeconds(0.3f);
            sequenceManager.ChangeAnim(ESequenceCharacter.AJucy, "backWalk",1);
            yield return new WaitForSeconds(0.4f);
            Animator.Play("Give");
            yield return new WaitForSeconds(0.3f);
            Animator.Play("Idle");
            Debug.Log("아저씨 : 아이스크림 받음");
        }
    }

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        yield return null;
        Animator.Play("Idle");
    }
}
