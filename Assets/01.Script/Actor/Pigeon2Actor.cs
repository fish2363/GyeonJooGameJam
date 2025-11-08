using DG.Tweening;
using System.Collections;
using UnityEngine;

public class Pigeon2Actor : SequenceActorBase
{
    [SerializeField] private Transform runPoint;
    public const string FLAG_FlyPigeon = "flyPigeon";
    private Vector3 origin;
    protected override void Awake()
    {
        origin = transform.position;
    }
    public override IEnumerator Execute(SequenceContext ctx)
    {
        ctx.SetFlag(FLAG_FlyPigeon);
        Animator.Play("RunToCat");
        Debug.Log("비둘기:날아오르기");
        transform.DOMove(runPoint.position,3f);
        bool ASeePigeon = ctx.HasFlag(CatActor.FLAG_CatSeePigeon);

        if (ASeePigeon)
        {
            yield return new WaitForSeconds(1f);
            Debug.Log("고양이:비둘기에게");
            sequenceManager.ChangeAnim(ESequenceCharacter.Cat, "GaJiMa", 1);
            ctx.SetFlag(CatActor.FLAG_Can_CatSeeMusicion);
            yield return new WaitForSeconds(2.5f);
        }
    }

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        Animator.Play("Idle");
        yield return transform.DOMove(origin, 1f);
    }
}
