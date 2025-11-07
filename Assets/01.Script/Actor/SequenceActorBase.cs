using System;
using System.Collections;
using UnityEngine;

public abstract class SequenceActorBase : MonoBehaviour
{
    [SerializeField] protected SequenceManager sequenceManager;
    public Animator Animator { get; set; }

    public ESequenceCharacter characterId;

    protected virtual void Awake()
    {
        Animator = GetComponentInChildren<Animator>();
    }
    /// <summary>정방향 실행 (선택 순서대로 호출)</summary>
    public abstract IEnumerator Execute(SequenceContext ctx);

    /// <summary>리와인드용 (역순으로 호출). 필요 없으면 건들지 말고 비워두면 됨</summary>
    public virtual IEnumerator Rewind(SequenceContext ctx)
    {
        yield break;
    }

    public void ChangeAnim(string anim, int actionNum)
    {
        Animator.Play(anim);
        if(actionNum == 1)
            OnTrigger();
        else if (actionNum == 2)
            OnTrigger2();
        else if (actionNum == 3)
            OnTrigger3();
    }

    protected virtual void OnTrigger()
    {
        
    }

    protected virtual void OnTrigger2()
    {

    }
    protected virtual void OnTrigger3()
    {

    }
}