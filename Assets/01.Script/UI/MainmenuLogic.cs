using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using DG.Tweening;
using UnityEngine.UI;
using Ami.BroAudio;

public class MainmenuLogic : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private Image whitePanel;

    [Header("PP 볼륨 (Global Volume)")]
    [SerializeField] private UnityEngine.Rendering.Volume ppVolume;

    [Header("Bloom 페이드 설정")]
    [SerializeField] private float fadeTime = 1.2f;
    private float targetBloomIntensity = 800f; // 얼마나 강하게 갈지

    private Bloom _bloom;
    private bool _isStarting;
    private Tween _ppTween;

    [SerializeField] private Ami.BroAudio.SoundID stage1Bgm;

        
    private void Awake()
    {
        if (ppVolume == null)
        {
            Debug.LogWarning("MainmenuLogic: ppVolume이 비어있음");
            return;
        }

        var profile = ppVolume.profile;
        if (profile == null)
        {
            Debug.LogWarning("MainmenuLogic: Volume Profile이 비어있음");
            return;
        }

        if (!profile.TryGet(out _bloom))
        {
            Debug.LogWarning("MainmenuLogic: Bloom override가 프로필에 없습니다.");
            return;
        }
        BroAudio.Stop(BroAudioType.Music);
        BroAudio.Play(stage1Bgm);
    }

    public void GameStart()
    {
        if (_isStarting) return;
        _isStarting = true;

        if (ppVolume == null || _bloom == null)
        {
            SceneManager.LoadScene("StageSelect");
            return;
        }
        _ppTween?.Kill();
        whitePanel.DOFade(1,fadeTime);
        _ppTween = DOTween.To(
                () => _bloom.intensity.value,
                v => _bloom.intensity.Override(v),
                targetBloomIntensity,
                fadeTime
            )
            .SetEase(Ease.InQuad)
            .OnComplete(() =>
            {
                SceneManager.LoadScene("StageSelect");
            });
    }

    public void GameStartCutScene()
    {
        director?.Play();
    }
}
