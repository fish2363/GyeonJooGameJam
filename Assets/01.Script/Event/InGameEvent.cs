using UnityEngine;

public static class InGameEvent
{
    public static ComeUpCardUIEvent GameStartEvent = new();
    public static ComeDownCardUIEvent GameEndEvent = new();
    public static ComeDownCinemaUIEvent ComeDownCinemaUIEvent = new();
    public static ComeUpCinemaUIEvent ComeUpCinemaUIEvent = new();
    public static ClearGameEvent ClearGameEvent = new();
}

public class ClearGameEvent : GameEvent
{
    public ClearGameEvent Initialize()
    {
        return this;
    }
}

public class ComeUpCardUIEvent : GameEvent
{
    public ComeUpCardUIEvent Initialize()
    {
        return this;
    }
}

public class ComeDownCardUIEvent : GameEvent
{
    public ComeDownCardUIEvent Initialize()
    {
        return this;
    }
}

public class ComeUpCinemaUIEvent : GameEvent
{
    public ComeUpCinemaUIEvent Initialize()
    {
        return this;
    }
}

public class ComeDownCinemaUIEvent : GameEvent
{
    public ComeDownCinemaUIEvent Initialize()
    {
        return this;
    }
}

