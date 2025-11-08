using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EPOOutline;

public class CharacterBtn : MonoBehaviour
{
    public Outlinable outlinable;
    public ESequenceCharacter characterId;
    public SequenceManager sequenceManager;

    public bool IsSelected { get; private set; }

    [SerializeField] private Image selectImage;
    [SerializeField] private TextMeshProUGUI numText;

    public void OnClick()
    {
        if (sequenceManager == null)
        {
            Debug.LogWarning($"{name}: SequenceManager∞° º≥¡§ æ» µ ");
            return;
        }

        sequenceManager.OnButtonClicked(this);
    }

    public void OnSelected()
    {
        IsSelected = true;
        outlinable.enabled = true;
        Debug.Log($"{name} º±≈√µ ");

        if (selectImage != null)
            selectImage.gameObject.SetActive(true);
    }

    public void OnDeselected()
    {
        IsSelected = false;
        outlinable.enabled = false;
        Debug.Log($"{name} «ÿ¡¶µ ");

        if (selectImage != null)
            selectImage.gameObject.SetActive(false);

        SetClearOrder();
    }

    public void SetOrder(int order)
    {
        if (numText == null)
            return;

        switch (order)
        {
            case 1: numText.text = "I"; break;
            case 2: numText.text = "II"; break;
            case 3: numText.text = "III"; break;
            case 4: numText.text = "IV"; break;
            case 5: numText.text = "V"; break;
            default:
                numText.text = order.ToString();
                break;
        }
    }

    public void SetClearOrder()
    {
        if (numText != null)
            numText.text = "";
    }
}
