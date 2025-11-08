using DG.Tweening;
using UnityEngine;

public class TaskUI : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO inGameEvent;
    [SerializeField] private RectTransform _rect;
    [SerializeField] private Ease cinemaEase;

    private void Awake()
    {
        inGameEvent.AddListener<ComeDownCinemaUIEvent>(HandleComeDownCinemaUI);
        inGameEvent.AddListener<ComeUpCinemaUIEvent>(HandleComeUpCinemaUI);
    }

    private void HandleComeUpCinemaUI(ComeUpCinemaUIEvent obj)
    {
        _rect.DOAnchorPosY(524.1692f, 0.2f).SetEase(cinemaEase);
        _rect.DOScaleY(2.459167f, 0.2f).SetEase(cinemaEase).SetDelay(0.5f);
    }

    private void HandleComeDownCinemaUI(ComeDownCinemaUIEvent obj)
    {
        _rect.DOAnchorPosY(392f, 0.2f).SetEase(cinemaEase).SetDelay(0.3f);
    }

    private void OnDestroy()
    {
        inGameEvent.RemoveListener<ComeDownCinemaUIEvent>(HandleComeDownCinemaUI);
        inGameEvent.RemoveListener<ComeUpCinemaUIEvent>(HandleComeUpCinemaUI);
    }
}
