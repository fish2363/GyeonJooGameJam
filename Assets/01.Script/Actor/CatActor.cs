using Ami.BroAudio;
using DG.Tweening;
using EPOOutline;
using System.Collections;
using UnityEngine;

public class CatActor : SequenceActorBase
{
    [SerializeField] private Transform chasePoint;
    [SerializeField] private Transform meetPigeonPoint;
    [SerializeField] private Transform exitPoint;
    [SerializeField] private GameObject dialogue;
    [SerializeField] private GameObject dialogueWow;
    [SerializeField] private Outlinable outline;
    public const string FLAG_CatSeeMusicion = "CatSeeMusicion";
    public const string FLAG_Can_CatSeeMusicion = "Can_CatSeeMusicion";
    public const string FLAG_CatSeePigeon = "CatSeePigeon";
    private SequenceContext _ctx;
    [SerializeField] private SoundID meow;

    private Vector3 origin;
    protected override void Awake()
    {
        origin = transform.position;
    }

    public override IEnumerator Execute(SequenceContext ctx)
    {
        _ctx = ctx;

        bool AflyPigeon = ctx.HasFlag(Pigeon2Actor.FLAG_FlyPigeon);
        if(!AflyPigeon)
        {
            Debug.Log("고양이 : 비둘기 보러가기");
            dialogue.SetActive(true);
            Animator.Play("MoveToPigeon");
            yield return transform.DOMove(meetPigeonPoint.position,1f).WaitForCompletion();
            ctx.SetFlag(FLAG_CatSeePigeon);
            BroAudio.Play(meow);
            Animator.Play("SeePigeon");
        }
        else
        {
            Debug.Log("고양이 실패:나가기");
            Animator.Play("ChasePigeon");
            BroAudio.Play(meow);
            transform.DOMove(exitPoint.position, 3.5f).OnComplete(() =>
            {
                outline.enabled = false;
                GetComponent<SpriteRenderer>().DOFade(0, 0.2f);
            });
        }

        yield return null;
    }

    protected override void OnTrigger()
    {
        base.OnTrigger();
        Debug.Log("고양이 : 비둘기 쫓아가기");
        dialogue.SetActive(false);
        BroAudio.Play(meow);
        transform.DOMove(chasePoint.position,3f)
            .OnComplete(() =>
            Animator.Play("SeePigeon")
        );
    }

    protected override void OnTrigger2()
    {
        base.OnTrigger2();
        dialogueWow.SetActive(true);
        BroAudio.Play(meow);
        _ctx.OnClear?.Invoke();
    }

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        dialogue.SetActive(false);
        dialogueWow.SetActive(false);
        yield return transform.DOMove(origin, 1f);
    }
}
