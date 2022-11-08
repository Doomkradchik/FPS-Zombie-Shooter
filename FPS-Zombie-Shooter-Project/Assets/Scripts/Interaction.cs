using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private string _promptMessage;
    [SerializeField]
    private UnityEvent Interacted;

    public string PromptText => _promptMessage;
    public virtual void Interact()
    {
        Interacted?.Invoke();
        Debug.Log("Interacted");
    }
}
