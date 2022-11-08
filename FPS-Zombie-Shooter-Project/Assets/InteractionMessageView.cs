using UnityEngine;
using UnityEngine.UI;

public sealed class InteractionMessageView : MonoBehaviour
{
    [SerializeField]
    private Text _message;

    public void UpdateText(string text)
    {
        if (string.IsNullOrEmpty(text))
            _message.text = string.Empty;
        else
            _message.text = $"[{text}]";
    }
}