using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Control : MonoBehaviour
{
    public string stringSceneNameSongMenuOfficial = "SongMenuOfficial";
    public string stringSceneNameSongMenuCustom = "SongMenuCustom";
    public string stringSceneNameChartEditor = "Creator";

    void Start()
    {

    }

    public void ButtonSceneTransferSongMenuOfficial()
    {
        StartCoroutine("_ButtonSceneTransferSongMenuOfficial");
    }
    private IEnumerator _ButtonSceneTransferSongMenuOfficial()
    {
        yield return null;
        SceneManager.LoadScene(stringSceneNameSongMenuOfficial);
    }

    public void ButtonSceneTransferSongMenuCustom()
    {
        StartCoroutine("_ButtonSceneTransferSongMenuCustom");
    }
    private IEnumerator _ButtonSceneTransferSongMenuCustom()
    {
        yield return null;
        SceneManager.LoadScene(stringSceneNameSongMenuCustom);
    }

    public void ButtonSceneTransferChartEditor()
    {
        StartCoroutine("_ButtonSceneTransferChartEditor");
    }
    private IEnumerator _ButtonSceneTransferChartEditor()
    {
        yield return null;
        SceneManager.LoadScene(stringSceneNameChartEditor);
    }
}
