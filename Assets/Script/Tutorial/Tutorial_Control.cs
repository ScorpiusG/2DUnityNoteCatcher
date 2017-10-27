using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Control : MonoBehaviour
{
    public float durationTutorial = 30f;

    private bool isLoadingScene = false;

    public void SceneLoad(string scene)
    {
        if (isLoadingScene) return;

        isLoadingScene = true;
        SceneTransition.LoadScene(scene);
    }

    private void Start()
    {
        StartCoroutine("_Start");
    }

    private IEnumerator _Start()
    {
        yield return null;

        // TODO: play video

        yield return new WaitForSecondsRealtime(durationTutorial);
        SceneLoad("Title");
    }
}
