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

    public override IEnumerator Execute(SequenceContext ctx)
    {
        int idxBicycle = ctx.GetIndex(ESequenceCharacter.Bicycle);
        bool hasNextCard = idxBicycle < ctx.Order.Count - 1;

        Debug.Log("자전거: 타고 나가기");
        Animator.Play("Go");
        transform.DOMove(goPos.position, rideOutDuration).OnComplete(() =>
        {
            GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            outlinable.enabled = false;
        });

        // TODO: 타임라인 / 애니
        yield return new WaitForSeconds(rideOutDuration+2);


        if (hasNextCard)
        {
            Debug.Log("자전거: 다음 카드가 있으므로 돌아오기");
            Animator.Play("Comeback");
            GetComponent<SpriteRenderer>().DOFade(1, 0.2f).OnComplete(() =>
            {
                transform.DOMove(comePos.position, rideOutDuration);
                outlinable.enabled = true;
            });

            // TODO: 돌아오기 연출
            yield return new WaitForSeconds(rideBackDuration);
            Animator.Play("Idle");
        }



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

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        // 필요하면 되감기 연출
        GetComponent<SpriteRenderer>().DOFade(1, 0.2f).OnComplete(() =>
             transform.DOMove(comePos.position, rideOutDuration));
        Animator.Play("Idle");
        yield break;
    }
}
