using UnityEngine;
using UnityEngine.Events;

public class Interaction : MonoBehaviour
{
    [SerializeField]
    private string _promptMessage;
    [SerializeField]
    private UnityEvent Interacted;

    public string PromptText => _promptMessage;
    private bool Lock() => CanInteract = false;

    public bool CanInteract { get; private set; } = true;
    public virtual void Interact()
    {
        if (CanInteract)
        {
            Interacted?.Invoke();
            OnInteracted();
            Lock();
        }
    }

    protected virtual void OnInteracted() { }
}
