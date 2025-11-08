using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EPOOutline;
using System.Collections.Generic;
using DG.Tweening;
using Ami.BroAudio;

public class CharacterBtn : MonoBehaviour
{
    [SerializeField, Header("뒤집는 시간")] private float duration = 0.1f;

    public Outlinable outlinable;
    public ESequenceCharacter characterId;
    public SequenceManager sequenceManager;
    public Sprite[] icons;
    private bool isFront = true;

    public bool IsSelected { get; private set; }

    [SerializeField] private Image selectImage;
    [SerializeField] private Image back;
    [SerializeField] private Image front;

    [SerializeField] private SoundID meow;

    private void Start()
    {
        front.rectTransform.localScale = Vector3.one;
        back.rectTransform.localScale = new Vector3(0, 1, 1);
    }

    public void Flip(bool toFront,float speed=1f)
    {
        isFront = toFront;
        BroAudio.Play(meow);
        back.DOKill();
        front.DOKill();

        (isFront ? back : front).rectTransform
           .DOScaleX(0, duration/speed).SetEase(Ease.OutQuad)
           .OnComplete(() =>
           {
               (isFront ? front : back).rectTransform
                  .DOScaleX(1, duration / speed).SetEase(Ease.OutQuad);
           });
    }

    /// <summary>
    /// flipCount 회수만큼 플립하면서 커졌다가,
    /// 앞면에서 멈추고 "탕" 내려놓듯이 원래 크기로 빠르게 줄어드는 연출
    /// </summary>
    public void SpinAndDisappear(int flipCount)
    {
        if (flipCount <= 0) flipCount = 1;

        // 혹시 남아있는 트윈들 정리
        transform.DOKill();
        front.DOKill();
        back.DOKill();

        Vector3 originalScale = transform.localScale;
        Vector3 bigScale = originalScale * 1.2f;   // 얼마나 커질지 (원하면 조절)

        float oneFlipTime = duration * 2f;             // Flip 안에서 0 -> 1 두 번 써서 대충 2*duration
        float totalFlipTime = oneFlipTime * flipCount;

        Sequence seq = DOTween.Sequence();

        for (int i = 0; i < flipCount; i++)
        {
            seq.AppendCallback(() =>
            {
                // 현재 상태 기준으로 반전
                Flip(!isFront,2);
            });

            // Flip 내부에서 duration * 2 정도 쓰니까 그만큼 기다려줌
            seq.AppendInterval(oneFlipTime/2);
        }

        // 플립하는 동안 점점 스케일 업
        seq.Join(
            transform.DOScale(bigScale, totalFlipTime/2)
                     .SetEase(Ease.OutQuad)
        );

        seq.AppendCallback(() =>
        {
            if (!isFront)
            {
                Flip(true);
            }
        });

        seq.Append(
            transform.DOScale(originalScale, 0.18f)
                     .SetEase(Ease.OutBack)
        );

        seq.OnComplete(() =>
        {
            // 필요하면 여기서 뭔가 더…
            // 예: 카드 고정, 클릭 막기 등
        });
    }

    public void OnClick()
    {
        if (sequenceManager == null)
        {
            Debug.LogWarning($"{name}: SequenceManager가 설정 안 됨");
            return;
        }

        sequenceManager.OnButtonClicked(this);
    }

    public void OnSelected()
    {
        IsSelected = true;
        outlinable.enabled = true;
        Debug.Log($"{name} 선택됨");
    }

    public void OnDeselected()
    {
        IsSelected = false;
        outlinable.enabled = false;
        Debug.Log($"{name} 해제됨");

        SetClearOrder();
    }

    public void SetOrder(int order)
    {
        selectImage.sprite = icons[order-1];
    }

    public void SetClearOrder()
    {
    }
}
