using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using System.Linq;

public class GameOverView : MonoBehaviour
{
    [SerializeField]
    private Animator _crossfade;

    private readonly string _endingClipName = "cf_end";

    public void ReloadLevel()
    {
        var clips = _crossfade.runtimeAnimatorController.animationClips;
        var clip = clips
            .ToList()
            .Find(c => c.name == _endingClipName);

        if (clip == null)
            throw new System.InvalidOperationException();

        StartCoroutine(ReloadLevelRoutine(clip.length));
    }

    private IEnumerator ReloadLevelRoutine(float duration)
    {
        _crossfade.SetTrigger("End");
        yield return new WaitForSeconds(duration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // reloading the scene (level)
    }

}
