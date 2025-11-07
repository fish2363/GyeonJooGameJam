using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceManager : MonoBehaviour
{
    [Header("선택 관련")]
    [SerializeField] private int maxSelectCount = 5;

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
    }

    public void ChangeAnim(ESequenceCharacter character,string animName,int actionNum=0)
    {
        actorLookup[character].ChangeAnim(animName,actionNum);
    }

    private void Start()
    {
        inGameEvent?.RaiseEvent(new ComeUpCardUIEvent().Initialize());
    }

    // ========== 버튼 클릭 ==========

    public void OnButtonClicked(CharacterBtn btn)
    {
        if (selectedButtons.Contains(btn))
        {
            // 이미 선택 → 해제
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
            MouseTextManager.Instance?.SpawnTextAtMouse("NONO");
            return;
        }

        // 코루틴 안에서 안전하게 쓰려고 복사본 생성
        foreach(CharacterBtn outlinable in selectedButtons)
        {
            outlinable.outlinable.enabled = false;
        }

        var orderCopy = new List<ESequenceCharacter>(selectedOrder);
        inGameEvent?.RaiseEvent(new ComeDownCardUIEvent().Initialize());
        StartCoroutine(RunSequence(orderCopy));

        // 선택 상태 리셋
        ResetSelectionUI();
    }

    public void ResetSelection()
    {
        // 코루틴 안에서 안전하게 쓰려고 복사본 생성
        foreach (CharacterBtn outlinable in selectedButtons)
        {
            outlinable.outlinable.enabled = false;
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

        selectedButtons.Clear();
        selectedOrder.Clear();
    }

    // ========== 시퀀스 실행 + 리와인드 ==========

    private IEnumerator RunSequence(List<ESequenceCharacter> order)
    {
        var ctx = new SequenceContext(order, OnClear);
        var selectedButtons = new List<CharacterBtn>(this.selectedButtons);

        executedActors.Clear();

        // 정방향 실행
        for (int i = 0; i < order.Count; i++)
        {
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
                    selectedButtons[j].outlinable.enabled = true;
                else
                    selectedButtons[j].outlinable.enabled = false;
            }

            yield return StartCoroutine(actor.Execute(ctx));
        }
        if (IsClear) yield break;
        Debug.Log("정방향 시퀀스 끝!");

        // 리와인드가 필요 없으면 이 부분 빼도 됨
        yield return StartCoroutine(RewindAll(ctx));
        inGameEvent?.RaiseEvent(new ComeUpCardUIEvent().Initialize());
        Debug.Log("리와인드까지 끝!");
    }

    private IEnumerator RewindAll(SequenceContext ctx)
    {
        for (int i = executedActors.Count - 1; i >= 0; i--)
        {
            var actor = executedActors[i];
            if (actor == null) continue;

            yield return StartCoroutine(actor.Rewind(ctx));
        }

        executedActors.Clear();
        yield break;
    }

    private void OnClear()
    {
        Debug.Log("SequenceManager: 클리어 발생");
        IsClear = true;
        inGameEvent?.RaiseEvent(new ClearGameEvent().Initialize());
    }
}
