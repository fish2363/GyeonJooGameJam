using System;
using UnityEngine;
using DG.Tweening;

public class InGameManager : MonoBehaviour
{
    [SerializeField] private GameEventChannelSO inGameEvent;
    [SerializeField] private RectTransform _cardUI;
    [SerializeField] private RectTransform _cinemaUI;
    [SerializeField] private Ease cardUIEase;
    [SerializeField] private Ease cinemaEase;
    [SerializeField] private float cardUiSpeed=0.2f;
    [SerializeField] private float cinemaUISpeed=0.2f;

    void Awake()
    {
        inGameEvent.AddListener<ComeUpCardUIEvent>(HandleGameStart);
        inGameEvent.AddListener<ComeDownCardUIEvent>(HandleGameEnd);

        inGameEvent.RaiseEvent(new ComeUpCardUIEvent().Initialize());
    }
    private void OnDestroy()
    {
        inGameEvent.RemoveListener<ComeUpCardUIEvent>(HandleGameStart);
        inGameEvent.RemoveListener<ComeDownCardUIEvent>(HandleGameEnd);
    }

    private void HandleGameStart(ComeUpCardUIEvent obj)
    {
        _cinemaUI.DOScale(1.3f,cinemaUISpeed).SetEase(cinemaEase);
        _cardUI.DOAnchorPosY(198f, cardUiSpeed).SetEase(cardUIEase);
        _cardUI.GetComponent<CanvasGroupCompo>().SetGroup(true);
    }

    private void HandleGameEnd(ComeDownCardUIEvent obj)
    {
        _cinemaUI.DOScale(1f, cinemaUISpeed).SetEase(cinemaEase);
        _cardUI.DOAnchorPosY(-230f, cardUiSpeed).SetEase(cardUIEase);
        _cardUI.GetComponent<CanvasGroupCompo>().SetGroup(false);
    }
}
