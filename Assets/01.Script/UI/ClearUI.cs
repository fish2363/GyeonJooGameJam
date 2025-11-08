using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Ami.BroAudio;

public class ClearUI : MonoBehaviour
{
    [SerializeField] private CanvasGroupCompo canvasGroup;
    [SerializeField] private GameEventChannelSO ClearChannel;
    [SerializeField] private RectTransform _clearUI;
    [SerializeField] private SoundID meow;

    public string nextSceneName;

    private void Awake()
    {
        ClearChannel.AddListener<ClearGameEvent>(HandleClearUI);
    }

    private void OnDestroy()
    {
        ClearChannel.RemoveListener<ClearGameEvent>(HandleClearUI);
    }

    private void HandleClearUI(ClearGameEvent obj)
    {
        BroAudio.Play(meow);
        canvasGroup.SetGroup(true,1);
        OpenUIAnimation();
    }

    private void OpenUIAnimation()
    {
        if (_clearUI == null)
        {
            Debug.LogWarning("ClearUI: _clearUI가 비어있음");
            return;
        }

        // 먼저 UI 활성화
        _clearUI.gameObject.SetActive(true);

        // 기존 트윈 있으면 정리
        _clearUI.DOKill();

        // 시작 상태: Y 스케일 0 (접혀있는 상태)
        Vector3 startScale = _clearUI.localScale;
        startScale.y = 0f;
        _clearUI.localScale = startScale;

        // 수욱~ 하고 늘어나기 (0 -> 1)
        _clearUI.DOScaleY(1f, 0.4f)
            .SetEase(Ease.OutBack); // 탄성 있게 "수욱" 느낌
    }

    public void NextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
