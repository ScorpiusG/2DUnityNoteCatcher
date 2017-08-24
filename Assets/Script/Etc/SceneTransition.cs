using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    private static SceneTransition transition;
    public Animator animatorTransition;
    private string stringSceneName = "";

    /// <summary>
    /// Plays an animation that transitions to the next scene.
    /// </summary>
    /// <param name="sceneName">Name of the scene to go to during transition.</param>
    public static void LoadScene(string sceneName)
    {
        transition.stringSceneName = sceneName;
        if (transition != null)
        {
            transition.animatorTransition.Play("clip");
        }
        else
        {
            transition.GoToNextScene();
        }
    }

    /// <summary>
    /// Called by the animator.
    /// </summary>
    public void GoToNextScene()
    {
        SceneManager.LoadScene(stringSceneName);
    }

    private void Start()
    {
        if (transition != null)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        transition = this;

        if (animatorTransition == null)
        {
            animatorTransition = GetComponent<Animator>();
        }
    }
}
