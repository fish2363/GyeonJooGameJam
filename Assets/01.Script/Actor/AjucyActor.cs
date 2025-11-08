using Ami.BroAudio;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class AjucyActor : SequenceActorBase
{
    [SerializeField] private Transform movePoint;
    public static bool IsIceCream;
    public const string FLAG_GoingRoad = "GoingRoad";
    public const string FLAG_AttackCat = "AttackCat";
    [SerializeField] private Transform givePos;
    [SerializeField] private GameObject iceCreamDialogue;

    private float eatTime=1f;
    private SequenceContext _ctx;
    [SerializeField] private Transform movePoint_CatAttack;
    private Vector3 origin;


    protected override void Awake()
    {
        origin = transform.position;
    }

    public override IEnumerator Execute(SequenceContext ctx)
    {
        if(SequenceContext.IsTropical)
        {
            string animName = IsIceCream ? "Icy_FrontWalk" : "FrontWalk";
            Animator.Play(animName);
            Debug.Log("아저씨 : 신호등 건넘");
            ctx.SetFlag(FLAG_GoingRoad);
            yield return transform.DOMove(movePoint.position,4f).SetEase(Ease.InSine).WaitForCompletion();
            var sr = GetComponent<SpriteRenderer>();
            sr.sortingLayerName = "Front";   // ← 레이어 이름으로 설정
            sr.sortingOrder = 100;       // ← 같은 레이어 안에서 맨 위쪽
            bool CatSee = ctx.HasFlag(CatActor.FLAG_CatSeeMusicion);
            if (!CatSee && IsIceCream)
            {
                Debug.Log("아저씨 : 아이스크림 먹음");
                Animator.Play("Icy_Eat");
                yield return new WaitForSeconds(eatTime);
                IsIceCream = false;
                Animator.Play("Idle");
            }
            else
                Animator.Play("Idle");
        }
        else
        {
            Debug.Log("아저씨 : 신호등 못 건넘");
            string animName = IsIceCream ? "Icy_Eat" : "Streching";
            Animator.Play(animName);
            yield return new WaitForSeconds(eatTime);
            IsIceCream = false;
            Animator.Play("Idle");
        }
        yield return null;
    }

    protected override void OnTrigger()
    {
        base.OnTrigger();
        iceCreamDialogue.SetActive(true);
        transform.DOMove(givePos.position, 0.7f)
            .OnComplete(() => {
                sequenceManager.ChangeAnim(ESequenceCharacter.AJumma, "Idle");
                Animator.Play("Icy_Idle");
                Debug.Log("아저씨 : 아이스크림 받음");
                IsIceCream = true;
            });
    }

    protected override void OnTrigger2()
    {
        base.OnTrigger2();

        transform.DOMove(movePoint_CatAttack.position, 1f).OnComplete(() =>
        {
            iceCreamDialogue.SetActive(false);
            Animator.Play("Icy_Cat");
            sequenceManager.ChangeAnim(ESequenceCharacter.Cat, "HitTale", 2);
        });
    }
    protected override void OnTrigger3()
    {
        base.OnTrigger3();
        iceCreamDialogue.SetActive(true);
    }
    public override IEnumerator Rewind(SequenceContext ctx)
    {
        iceCreamDialogue.SetActive(false);
        Animator.Play("Idle");
        yield return transform.DOMove(origin, 1f);
    }
}
