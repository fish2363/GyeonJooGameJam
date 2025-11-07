using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasGroupCompo : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetGroup(bool interact)
    {
        _canvasGroup.interactable = interact;
        _canvasGroup.blocksRaycasts = interact;
    }
    public void SetGroup(bool interact,int alpha)
    {
        _canvasGroup.alpha = alpha;
        _canvasGroup.interactable = interact;
        _canvasGroup.blocksRaycasts = interact;
    }
}
