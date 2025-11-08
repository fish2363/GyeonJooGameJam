using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using Ami.BroAudio;

public class SequenceManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.Rendering.Volume volume;

    private FilmGrain filmGrain;
    private ColorAdjustments colorAdjustments;
    private LensDistortion lensDistortion;

    [SerializeField] private CinemachineCamera playCam;
    [SerializeField] private CinemachineCamera readyCam;

    [Header("선택 관련")]
    [SerializeField] private int maxSelectCount = 5;
    [SerializeField] private PlayableAsset cleardirector;
    [SerializeField] private PlayableDirector director;

    [Header("이번 스테이지에 등장하는 액터들")]
    [SerializeField] private List<SequenceActorBase> actors = new();

    [SerializeField] private GameEventChannelSO inGameEvent; // 있으면 Clear 이벤트 등 쓸 때
    public static bool IsClear=false;
    // UI용 선택 버튼들
    private readonly List<CharacterBtn> selectedButtons = new();

    // 로직용 선택 순서
    private readonly List<ESequenceCharacter> selectedOrder = new();

    // 캐릭터 타입 -> 액터
    private Dictionary<ESequenceCharacter, SequenceActorBase> actorLookup;

    // 이번 턴에 실제 실행된 액터들 (리와인드용)
    private readonly List<SequenceActorBase> executedActors = new();
    public List<Trofical> troficals = new();

    [SerializeField] private SoundID click;

    private const int activeTruePriority = 2;
    private const int activeFalsePriority = 1;
    private Tween _ppTween;

    private void Awake()
    {
        actorLookup = new Dictionary<ESequenceCharacter, SequenceActorBase>();
        foreach (var actor in actors)
        {
            if (actor == null) continue;

            if (actorLookup.ContainsKey(actor.characterId))
            {
                Debug.LogWarning($"중복된 characterId: {actor.characterId}");
                continue;
            }

            actorLookup[actor.characterId] = actor;
        }

        if (volume == null || volume.profile == null)
        {
            Debug.LogWarning("PostFXController: Volume이나 Volume Profile이 비어있음");
            return;
        }

        volume.profile.TryGet(out filmGrain);
        volume.profile.TryGet(out colorAdjustments);
        volume.profile.TryGet(out lensDistortion);
    }

    public void ChangeAnim(ESequenceCharacter character,string animName,int actionNum=0)
    {
        actorLookup[character].ChangeAnim(animName,actionNum);
    }

    public void StartGame()
    {
        inGameEvent?.RaiseEvent(new ComeUpCardUIEvent().Initialize());
        inGameEvent?.RaiseEvent(new ComeUpCinemaUIEvent().Initialize());
    }

    // ========== 버튼 클릭 ==========

    public void OnButtonClicked(CharacterBtn btn)
    {
        Debug.Log("DP?");
        if (selectedButtons.Contains(btn))
        {
        Debug.Log("DP?");
            // 이미 선택 → 해제
            btn.Flip(true);
            selectedButtons.Remove(btn);
            selectedOrder.Remove(btn.characterId);
            btn.OnDeselected();
        }
        else
        {
            if (selectedButtons.Count >= maxSelectCount)
            {
                Debug.LogWarning("최대 선택 수 초과");
                MouseTextManager.Instance?.SpawnTextAtMouse("NONO");
                return;
            }
        Debug.Log("DP?");
            btn.Flip(false);
            selectedButtons.Add(btn);
            selectedOrder.Add(btn.characterId);
            btn.OnSelected();
        }

        // 번호 갱신
        for (int i = 0; i < selectedButtons.Count; i++)
            selectedButtons[i].SetOrder(i + 1);

        Debug.Log("선택 순서: " + string.Join(" -> ", selectedOrder));
    }

    // ========== 확인 / 리셋 ==========

    public void Confirm()
    {
        if (selectedOrder.Count == 0)
            return;

        if (selectedOrder.Count != maxSelectCount)
        {
            Debug.LogWarning("선택 개수가 부족함");
            MouseTextManager.Instance?.SpawnTextAtMouse("모든 버튼을 선택해주세요!");
            return;
        }
        BroAudio.Play(click);
        inGameEvent?.RaiseEvent(new ComeDownCinemaUIEvent().Initialize());
        var orderCopy = new List<ESequenceCharacter>(selectedOrder);
        StartCoroutine(RunSequence(orderCopy));

    }

    public void ResetSelection()
    {
        BroAudio.Play(click);

        inGameEvent?.RaiseEvent(new ComeUpCinemaUIEvent().Initialize());

        // 코루틴 안에서 안전하게 쓰려고 복사본 생성
        foreach (CharacterBtn outlinable in selectedButtons)
        {
            outlinable.outlinable.enabled = false;
            outlinable.Flip(true);
        }
        ResetSelectionUI();
        Debug.Log("선택 리셋");
    }

    private void ResetSelectionUI()
    {
        foreach (var btn in selectedButtons)
        {
            if (btn != null)
                btn.OnDeselected();
        }
        foreach (CharacterBtn outlinable in selectedButtons)
        {
            outlinable.outlinable.enabled = false;
        }
        selectedButtons.Clear();
        selectedOrder.Clear();
    }

    // ========== 시퀀스 실행 + 리와인드 ==========

    private IEnumerator RunSequence(List<ESequenceCharacter> order)
    {
        playCam.Priority = activeTruePriority;
        readyCam.Priority = activeFalsePriority;
        var ctx = new SequenceContext(order, OnClear);
        var selectedButtons = new List<CharacterBtn>(this.selectedButtons);

        executedActors.Clear();

        // 정방향 실행
        for (int i = 0; i < order.Count; i++)
        {
            SequenceContext.IsTropical = (i == 2 || i == 3);
            foreach(Trofical trofical in troficals)
            {
                trofical.ChangeLight(SequenceContext.IsTropical);
            }
            Debug.Log($"신호등 : {SequenceContext.IsTropical}");

            var who = order[i];

            if (!actorLookup.TryGetValue(who, out var actor) || actor == null)
            {
                Debug.LogWarning($"Actor 없음: {who}");
                continue;
            }

            executedActors.Add(actor);

            for(int j=0;j<selectedButtons.Count;j++)
            {
                if(j==i)
                {
                    selectedButtons[j].outlinable.enabled = true;
                    selectedButtons[i].Flip(true);
                    //selectedButtons[i].SpinAndDisappear(4);
                }
                else
                    selectedButtons[j].outlinable.enabled = false;
            }
            yield return StartCoroutine(actor.Execute(ctx));
            yield return new WaitForSeconds(1.5f);
            
        }
        if (IsClear)
        {
            director.playableAsset = cleardirector;
            director.Play();
            director.stopped += Clear;
            yield break;
        }
        Debug.Log("정방향 시퀀스 끝!");
        // 리와인드가 필요 없으면 이 부분 빼도 됨
        yield return new WaitForSeconds(2f);
        inGameEvent?.RaiseEvent(new ComeDownCardUIEvent().Initialize());
        yield return StartCoroutine(RewindAll(ctx));
        inGameEvent?.RaiseEvent(new ComeUpCardUIEvent().Initialize());
        inGameEvent?.RaiseEvent(new ComeUpCinemaUIEvent().Initialize());
        Debug.Log("리와인드까지 끝!");

        // 선택 상태 리셋
        ResetSelectionUI();
    }

    public void Clear(PlayableDirector director)
    {
        director.stopped -= Clear;
        inGameEvent?.RaiseEvent(new ClearGameEvent().Initialize());
    }

    private IEnumerator RewindAll(SequenceContext ctx)
    {
        filmGrain.active = true;
        colorAdjustments.active = true;
        for (int i = executedActors.Count - 1; i >= 0; i--)
        {
            var actor = executedActors[i];
            if (actor == null) continue;

            yield return StartCoroutine(actor.Rewind(ctx));
            yield return new WaitForSeconds(1f);
        }
        filmGrain.active = false;
        colorAdjustments.active = false;
        executedActors.Clear();
        playCam.Priority = activeFalsePriority;
        readyCam.Priority = activeTruePriority;
        yield break;
    }

    public void OnCinemaUI()
    {
        inGameEvent?.RaiseEvent(new ComeDownCinemaUIEvent().Initialize());
    }
    public void SetLens()
    {
        _ppTween?.Kill();
        _ppTween = DOTween.To(
                () => lensDistortion.intensity.value,
                v => lensDistortion.intensity.Override(v),
                -1f,
                3f
            )
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                lensDistortion.active = false;
            });
    }
    private void OnClear()
    {
        inGameEvent?.RaiseEvent(new ComeDownCardUIEvent().Initialize());
        inGameEvent?.RaiseEvent(new ComeUpCinemaUIEvent().Initialize());
        Debug.Log("SequenceManager: 클리어 발생");
        IsClear = true;
    }
}
