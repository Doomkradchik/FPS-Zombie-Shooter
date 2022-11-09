using UnityEngine.UI;
using UnityEngine;

public class PlayerHealthView : MonoBehaviour
{
    [SerializeField]
    private Text _health;

    public void UpdateHealthText(float health)
    {
        _health.text = health.ToString();
    }
}
