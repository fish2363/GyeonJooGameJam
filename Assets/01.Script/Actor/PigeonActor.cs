    using DG.Tweening;
    using System.Collections;
    using UnityEngine;

    public class PigeonActor : SequenceActorBase
    {
        [SerializeField] private float poopDuration = 0.4f;
        private float caretakerHitDelay = 2.5f;

        [Header("먹이 위치 / 원래 위치")]
        [SerializeField] private Transform eatPos;
        private Vector3 originPos;

        [Header("빙글빙글 비행 설정")]
        [SerializeField] private Transform flyCenter;         // 이 트랜스폼을 기준으로 돈다
        [SerializeField] private float flyRadius = 1.5f;      // 회전 반경
        private float flyLoopDuration = 2f;  // 한 바퀴 도는 시간

        private Tween flyTween;   // 빙글빙글 돌기용 트윈

        protected override void Awake()
        {
            base.Awake();
            originPos = transform.position;
        }

        public override IEnumerator Execute(SequenceContext ctx)
        {
            int idxPigeon = ctx.GetIndex(ESequenceCharacter.Pigeon);
            int idxCaretaker = ctx.GetIndex(ESequenceCharacter.Caretaker);

            // 1) 배설하기
            Debug.Log("비둘기: 배설하기");
            Animator.Play("Poop");
            yield return new WaitForSeconds(poopDuration);

            // 2) (예시 그대로 유지) 관리인이 나중에 실행됐으면 맞는 연출
            if (idxCaretaker != -1 && idxCaretaker > idxPigeon)
            {
                sequenceManager.ChangeAnim(ESequenceCharacter.Caretaker, "Gardner-Hit");
                Debug.Log("비둘기: 전에 있던 관리인이 똥을 맞음 (화냄)");
                yield return new WaitForSeconds(caretakerHitDelay);
            }
            else
            {
                yield break;
            }

            // 이후 행동(날아다니기 등)이 있다면 여기 이어서...
        }

    // 물 피해서 날아다니는 구간 시작
    protected override void OnTrigger()
    {
        base.OnTrigger();
        Debug.Log("비둘기: 물 피해서 날아댕김");

        // 이미 돌고 있으면 새로 만들지 않음 (툭 끊기는 거 방지)
        if (flyTween != null && flyTween.IsActive())
            return;

        if (flyCenter == null)
        {
            Debug.LogWarning("PigeonActor: flyCenter가 설정되어 있지 않습니다.");
            return;
        }

        // 현재 위치 기준으로 각도/반지름 계산
        Vector3 dir = (transform.position - flyCenter.position);
        float radius = dir.magnitude;
        if (radius < 0.001f)
        {
            // 거의 정확히 중심에 붙어있으면, 그냥 flyRadius 사용
            radius = flyRadius;
            dir = Vector3.right * radius;
        }
        else
        {
            // 반지름 너무 이상하면 설정값으로 덮어도 됨 (원하면 여기에 radius = flyRadius;)
        }

        float startAngle = Mathf.Atan2(dir.y, dir.x);
        float angle = startAngle;

        // startAngle에서부터 한 바퀴씩 계속 증가
        flyTween = DOTween.To(
            () => angle,
            x =>
            {
                angle = x;
                var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
                transform.position = flyCenter.position + offset;
            },
            startAngle + Mathf.PI * 2f,
            flyLoopDuration
        )
        .SetLoops(-1, LoopType.Incremental)  // 각도가 계속 누적되도록
        .SetEase(Ease.Linear);
    }

    // 먹이 위치로 내려가는 타이밍
    protected override void OnTrigger2()
    {
        base.OnTrigger2();

        // 1) 빙글빙글 트윈 정리 (지금 위치 그대로 유지)
        if (flyTween != null)
        {
            if (flyTween.IsActive())
                flyTween.Kill(false);   // false = Complete 안 하고 현재 값 유지
            flyTween = null;
        }

        // 2) 혹시 다른 position 트윈들 있으면 다 끊기
        transform.DOKill(); // 이 트랜스폼에 걸린 모든 트윈 정리

        // 3) "현재 위치"에서 eatPos까지 부드럽게 이동
        Animator.Play("Run");
        transform.DOMove(eatPos.position, 2f).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            Animator.Play("Eat");
        });
    }

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        // 리와인드 시에도 혹시 남아있을 flyTween 정리
        if (DOTween.IsTweening(transform))
            flyTween.Kill();

        transform.DOMove(originPos, 0.2f);
        return base.Rewind(ctx);
    }
}
