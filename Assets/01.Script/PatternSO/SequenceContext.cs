using System;
using System.Collections.Generic;
public enum ESequenceCharacter
{
    Bicycle,
    Pigeon,
    Caretaker,
    Grandpa,
    // 스테이지2,3에서 늘려가면 됨
}
public class SequenceContext
{
    // 이번 턴에 선택된 순서
    public List<ESequenceCharacter> Order { get; private set; }

    // 캐릭터 → 몇 번째에 선택됐는지 (없으면 -1)
    public Dictionary<ESequenceCharacter, int> IndexByChar { get; private set; }

    // 비둘기 날아다님, 클리어 가능 등 상태 플래그
    public HashSet<string> Flags { get; private set; } = new HashSet<string>();

    // 클리어 시 호출되는 콜백
    public Action OnClear;

    public SequenceContext(List<ESequenceCharacter> order, Action onClear = null)
    {
        Order = order;
        OnClear = onClear;

        IndexByChar = new Dictionary<ESequenceCharacter, int>();
        for (int i = 0; i < order.Count; i++)
        {
            IndexByChar[order[i]] = i;
        }
    }

    public int GetIndex(ESequenceCharacter ch)
    {
        return IndexByChar.TryGetValue(ch, out int idx) ? idx : -1;
    }

    public bool HasFlag(string flag) => Flags.Contains(flag);
    public void SetFlag(string flag) => Flags.Add(flag);
    public void RemoveFlag(string flag) => Flags.Remove(flag);
}
