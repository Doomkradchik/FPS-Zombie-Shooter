using UnityEngine;
using System.Linq;
using System;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioData[] _datas;

    public static AudioManager Instance;

    private void OnEnable()
    {
        if (Instance != null)
            throw new System.InvalidOperationException();

        Instance = this;
    }

    public void PlaySound(AudioData.Kind kind)
    {
        var sound = _datas
            .ToList()
            .Find(data => data.AudioKind == kind);

        if (sound == null)
            throw new System.InvalidOperationException();

        sound.Source.Play();
    }

    public void StopSound(AudioData.Kind kind)
    {
        var sound = _datas
            .ToList()
            .Find(data => data.AudioKind == kind);

        if (sound == null)
            throw new System.InvalidOperationException();

        if (sound.Source.isPlaying)
            sound.Source.Stop();
    }

}

[Serializable]
public sealed class AudioData
{
    [SerializeField]
    private Kind _kind;

    [SerializeField]
    private AudioSource _source;
    public Kind AudioKind => _kind;
    public AudioSource Source => _source;

    public enum Kind
    {
        Shoot,
        Perform,
        Alarm,
    }
}

