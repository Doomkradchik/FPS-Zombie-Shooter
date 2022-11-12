using UnityEngine.AI;
using UnityEngine;
using System.Collections;

public class StartPoint : MonoBehaviour
{
    [SerializeField]
    private Animator[] _doors;

    [SerializeField]
    private Animator[] _alarms;

    [SerializeField]
    private NavMeshSurface _navMesh;

    private readonly string _animationName = "Activate";
    private readonly string _playKey = "isPlaying";

    private void Start()
    {
        _navMesh.BuildNavMesh();
    }

    public void ActivateDoors()
    {
        foreach(var door in _doors)
        {
            door.Play(_animationName);
        }
    }

    public void WaitForEnding(float duration) => StartCoroutine(WaitForEndRoutine(duration));

    private IEnumerator WaitForEndRoutine(float duration)
    {
        OnStarted();
        yield return new WaitForSeconds(duration);
        OnFinished();     
    }

    private void OnStarted()
    {
        AudioManager.Instance.PlaySound(AudioData.Kind.Alarm);

        foreach (var alarm in _alarms)
            alarm.SetBool(_playKey, true);
    }

    private void OnFinished()
    {
        foreach (var alarm in _alarms)
            alarm.SetBool(_playKey, false);

        _navMesh.BuildNavMesh();
        AudioManager.Instance.StopSound(AudioData.Kind.Alarm);
    }
}
