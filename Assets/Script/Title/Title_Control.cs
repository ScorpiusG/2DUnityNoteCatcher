using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title_Control : MonoBehaviour
{
    public Camera cameraMain;
    public Color colorCameraMainBGColor = Color.black;
    public float floatDurationBetweenColor = 4f;

    public string stringSceneNameSongMenuOfficial = "SongMenuOfficial";
    public string stringSceneNameSongMenuCustom = "SongMenuCustom";
    public string stringSceneNameChartEditor = "Creator";

    void Start()
    {
        cameraMain.backgroundColor = Random.ColorHSV(0f, 1f, 0.3f, 0.6f, 1f, 1f);
        StartCoroutine("CameraMainColorCycle");
    }

    void Update()
    {
        cameraMain.backgroundColor = Color.Lerp(cameraMain.backgroundColor, colorCameraMainBGColor, Time.deltaTime * 2f / floatDurationBetweenColor);
    }

    public void ButtonSceneTransferSongMenuOfficial()
    {
        StartCoroutine("_ButtonSceneTransferSongMenuOfficial");
    }
    private IEnumerator _ButtonSceneTransferSongMenuOfficial()
    {
        yield return null;
        //SceneManager.LoadScene(stringSceneNameSongMenuOfficial);
        SceneTransition.LoadScene(stringSceneNameSongMenuOfficial);
    }

    public void ButtonSceneTransferSongMenuCustom()
    {
        StartCoroutine("_ButtonSceneTransferSongMenuCustom");
    }
    private IEnumerator _ButtonSceneTransferSongMenuCustom()
    {
        yield return null;
        //SceneManager.LoadScene(stringSceneNameSongMenuCustom);
        SceneTransition.LoadScene(stringSceneNameSongMenuCustom);
    }

    public void ButtonSceneTransferChartEditor()
    {
        StartCoroutine("_ButtonSceneTransferChartEditor");
    }
    private IEnumerator _ButtonSceneTransferChartEditor()
    {
        yield return null;
        //SceneManager.LoadScene(stringSceneNameChartEditor);
        SceneTransition.LoadScene(stringSceneNameChartEditor);
    }

    public void ButtonGameExit()
    {
        StartCoroutine("_ButtonGameExit");
    }
    private IEnumerator _ButtonGameExit()
    {
        yield return null;
        Application.Quit();
    }

    private IEnumerator CameraMainColorCycle()
    {
        yield return null;
        while(true)
        {
            colorCameraMainBGColor = Random.ColorHSV(0f, 1f, 0.3f, 0.6f, 1f, 1f);
            yield return null;
            yield return new WaitForSeconds(floatDurationBetweenColor);
        }
    }
}
