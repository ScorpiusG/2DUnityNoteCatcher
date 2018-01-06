using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Title_Control : MonoBehaviour
{
    public Camera cameraMain;
    public Color colorCameraMainBGColor = Color.black;
    public float floatDurationBetweenColor = 4f;

    public GameObject objectTranslationButton;

    public string stringSceneNameSongMenuOfficial = "SongMenuOfficial";
    public string stringSceneNameSongMenuCustom = "SongMenuCustom";
    public string stringSceneNameChartEditor = "Creator";
    public string stringSceneNameTranslationMenu = "TranslationMenu";
    public string stringSceneNameMarathonMenu = "MarathonMenu";
    public string stringSceneNameTutorial = "Tutorial";

    private bool boolUpdateOther = false;

    void Start()
    {
        cameraMain.backgroundColor = Random.ColorHSV(0f, 1f, 0.1f, 0.5f, 1f, 1f);
        StartCoroutine("CameraMainColorCycle");
    }

    void Update()
    {
        cameraMain.backgroundColor = Color.Lerp(cameraMain.backgroundColor, colorCameraMainBGColor, Time.deltaTime * 2f / floatDurationBetweenColor);
    }

    void FixedUpdate()
    {
        // Update on every other frame
        boolUpdateOther = !boolUpdateOther;
        if (boolUpdateOther) return;

        // Enable translation button if "Translation" folder exists
        if (!objectTranslationButton.activeSelf)
        {
            if (System.IO.Directory.Exists("Translation"))
            {
                objectTranslationButton.SetActive(true);
            }
        }
    }

    public void ButtonSceneTransferTranslationMenu()
    {
        StartCoroutine("_ButtonSceneTransferTranslationMenu");
    }
    private IEnumerator _ButtonSceneTransferTranslationMenu()
    {
        yield return null;
        SceneTransition.LoadScene(stringSceneNameTranslationMenu);
    }

    public void ButtonSceneTransferMarathonMenu()
    {
        StartCoroutine("_ButtonSceneTransferMarathonMenu");
    }
    private IEnumerator _ButtonSceneTransferMarathonMenu()
    {
        yield return null;
        SceneTransition.LoadScene(stringSceneNameMarathonMenu);
    }

    public void ButtonSceneTransferSongMenuOfficial()
    {
        StartCoroutine("_ButtonSceneTransferSongMenuOfficial");
    }
    private IEnumerator _ButtonSceneTransferSongMenuOfficial()
    {
        yield return null;
        SceneTransition.LoadScene(stringSceneNameSongMenuOfficial);
    }

    public void ButtonSceneTransferSongMenuCustom()
    {
        StartCoroutine("_ButtonSceneTransferSongMenuCustom");
    }
    private IEnumerator _ButtonSceneTransferSongMenuCustom()
    {
        yield return null;
        SceneTransition.LoadScene(stringSceneNameSongMenuCustom);
    }

    public void ButtonSceneTransferChartEditor()
    {
        StartCoroutine("_ButtonSceneTransferChartEditor");
    }
    private IEnumerator _ButtonSceneTransferChartEditor()
    {
        yield return null;
        SceneTransition.LoadScene(stringSceneNameChartEditor);
    }

    public void ButtonSceneTransferTutorial()
    {
        StartCoroutine("_ButtonSceneTransferTutorial");
    }
    private IEnumerator _ButtonSceneTransferTutorial()
    {
        yield return null;
        SceneTransition.LoadScene(stringSceneNameTutorial);
    }

    public void ButtonGameExit()
    {
        StartCoroutine("_ButtonGameExit");
    }
    private IEnumerator _ButtonGameExit()
    {
        if (Input.GetKey(KeyCode.Delete) && Input.GetKey(KeyCode.D))
        {
            PlayerPrefs.DeleteAll();
        }

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
