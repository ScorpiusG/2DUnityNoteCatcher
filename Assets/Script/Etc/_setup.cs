using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class _setup : MonoBehaviour
{
    public string stringNextScene = "Title";

	void Start ()
    {
        StartCoroutine("RunMe");
    }

    IEnumerator RunMe ()
    {
        yield return new WaitForSeconds(0.5f);

        if (PlayerPrefs.HasKey("PlayerSetting"))
            SceneTransition.LoadScene(stringNextScene);
        else
            SceneTransition.LoadScene("Tutorial");
        //SceneManager.LoadScene(stringNextScene);
    }
}
