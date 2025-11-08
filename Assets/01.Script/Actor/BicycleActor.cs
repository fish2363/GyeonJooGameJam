using Ami.BroAudio;
using DG.Tweening;
using EPOOutline;
using System.Collections;
using UnityEngine;

public class BicycleActor : SequenceActorBase
{
    [SerializeField] private float rideOutDuration = 1f;
    [SerializeField] private float rideBackDuration = 1f;
    [SerializeField] private Transform goPos;
    [SerializeField] private Transform comePos;
    [SerializeField] Outlinable outlinable;
    [SerializeField] private SoundID bell;

    public const string FLAG_GONE = "RiderGone";

    SequenceContext _ctx;

    public override IEnumerator Execute(SequenceContext ctx)
    {
        _ctx = ctx;
        int idxBicycle = ctx.GetIndex(ESequenceCharacter.Bicycle);
        bool hasNextCard = idxBicycle < ctx.Order.Count - 1;

        Debug.Log("자전거: 타고 나가기");
        Animator.Play("Go");
        BroAudio.Play(bell);
        transform.DOMove(goPos.position, rideOutDuration).OnComplete(() =>
        {
            GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            outlinable.enabled = false;
            ctx.SetFlag(FLAG_GONE);
        });

        // TODO: 타임라인 / 애니
        yield return new WaitForSeconds(1f);


        // 4) 할아버지가 과자 주기까지 성공했는지 확인
        bool grandpaSnackDone = ctx.HasFlag(GrandpaActor.FLAG_GRANDPA_SNACK_DONE);

        if (!grandpaSnackDone)
        {
            // 순서는 맞았는데, 할아버지가 비둘기와 상호작용 실패 → 클리어 아님
            yield break;
        }

        Debug.Log("클리어! (관리인 → 비둘기 → 할아버지 → 자전거)");

        ctx.OnClear?.Invoke();
    }
    protected override void OnTrigger()
    {
        base.OnTrigger();
        StartCoroutine(Comeback());
    }

    private IEnumerator Comeback()
    {
        _ctx.RemoveFlag(FLAG_GONE);
        GetComponent<SpriteRenderer>().DOFade(1, 0.2f).OnComplete(() =>
        {
            transform.DOMove(comePos.position, rideOutDuration);
        });

        // TODO: 돌아오기 연출
        yield return new WaitForSeconds(rideBackDuration);
        Animator.Play("Idle");
    }
    public override IEnumerator Rewind(SequenceContext ctx)
    {
        // 필요하면 되감기 연출
        GetComponent<SpriteRenderer>().DOFade(1, 0.2f).OnComplete(() =>
             transform.DOMove(comePos.position, rideOutDuration));
        Animator.Play("Idle");
        yield break;
    }
}
