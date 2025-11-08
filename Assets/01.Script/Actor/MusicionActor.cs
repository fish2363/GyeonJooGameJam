using Ami.BroAudio;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MusicionActor : SequenceActorBase
{
    [SerializeField] private SoundID meow;

    public override IEnumerator Execute(SequenceContext ctx)
    {
        Debug.Log("À½¾Ç°¡ : ³ë·¡ ºÎ¸£±â");
        BroAudio.Play(meow);
        Animator.Play("Song");

        bool ASeeCat = ctx.HasFlag(CatActor.FLAG_Can_CatSeeMusicion);
        if (ASeeCat)
        {
            sequenceManager.ChangeAnim(ESequenceCharacter.Cat, "SeePigeon");
            Debug.Log("°í¾çÀÌ : À½¾Ç°¡ ¹ß°ß");

            bool CatSee = ctx.HasFlag(AjucyActor.FLAG_GoingRoad);
            if (CatSee)
            {
                if(AjucyActor.IsIceCream)
                {
                    Debug.Log("¾ÆÀú¾¾ : ²¿¸® ¹âÀ½");
                    sequenceManager.ChangeAnim(ESequenceCharacter.AJucy, "Icy_FrontWalk", 2);
                }
                else
                {
                    sequenceManager.ChangeAnim(ESequenceCharacter.AJucy, "BackIdle", 3);
                }
            }
        }
        yield return null;
    }

    public override IEnumerator Rewind(SequenceContext ctx)
    {
        yield return null;
        Animator.Play("Idle");
    }
}
