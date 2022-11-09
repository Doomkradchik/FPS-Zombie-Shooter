using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class PlayerHealthView : MonoBehaviour
{
    [SerializeField]
    private Text _health;


    [Header("ScreenFX")]
    [SerializeField]
    private Image _bloodScreen;

    [SerializeField]
    private Image _healScreen;

    [SerializeField]
    private float _duration;

    [SerializeField]
    private float _expiredDuration;

    private void Start()
    {
        _healScreen.gameObject.SetActive(false);
        _bloodScreen.gameObject.SetActive(false);
    }

    public void UpdateHealthText(float health)
    {
        _health.text = health.ToString();
    }

    public void PerformScreenEffect(ScreenFXKind fXKind)
    {
        Image image;
        switch (fXKind)
        {
            case ScreenFXKind.Heal:
                image = _healScreen;
                break;
            case ScreenFXKind.Hurt:
                image = _bloodScreen;
                break;
            default:
                return;
        }

        StartCoroutine(PerfromScreenRoutine(image));
    }

    private IEnumerator PerfromScreenRoutine(Image imagefx)
    {
        var expiredTime = _expiredDuration;
        var progress = 1f;
        var color = imagefx.color;
        var startAlphaColor = color.a;

        imagefx.gameObject.SetActive(true);

        yield return new WaitForSeconds(_duration);

        while (progress > 0f)
        {
            expiredTime -= Time.deltaTime;
            progress = expiredTime / _expiredDuration;

            color.a = startAlphaColor * progress;
            imagefx.color = color;
            yield return null;
        }

        imagefx.gameObject.SetActive(false);
        color.a = startAlphaColor;
        imagefx.color = color;
    }

    public enum ScreenFXKind
    {
        Heal,
        Hurt
    }
}

