using UnityEngine;
using UnityEngine.UI;

public sealed class InteractionMessageView : MonoBehaviour
{
    [SerializeField]
    private Text _message;

    [SerializeField]
    private GameObject _interactionClue;

    private void Start() => _interactionClue.SetActive(false);

    public void UpdateText(string text)
    {
        _interactionClue.SetActive(true);
        if (string.IsNullOrEmpty(text))
        {
            _interactionClue.SetActive(false);
            _message.text = string.Empty;
        }
        else
            _message.text = $"[{text}]";
    }
}