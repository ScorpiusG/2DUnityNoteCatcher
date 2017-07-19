using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongMenu_Control : MonoBehaviour
{
    public bool isLoadCustomSongs = true;

    public string stringSceneNameTitle = "Title";
    public string stringSceneNameGame = "Game";

    public string stringSongDirectoryPath = "/Songs";
    //public string[] arrayStringSongDirectory;
    public List<string> listStringSongDirectory;

    public RectTransform rectSongListParent;
    public Button buttonSongIndividual;
    public float floatVertDistanceBetweenButtons = 80f;

    public Text textDisplayAccuracy;
    public Text textDisplayMods;

    public GameObject objectOptionsMenu;
    public GameObject[] groupOptionsMenuPage;
    private int intOptionsMenuPage = 0;

    void Start()
    {
        objectOptionsMenu.SetActive(false);

        string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath;
#if UNITY_EDITOR
        Debug.Log(path);
#endif

        // The "Songs" folder does not exist in the game directory
        if (!Directory.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: \"Songs\" folder does not exist!");
#endif

            Destroy(buttonSongIndividual.gameObject);
            buttonSongIndividual = null;
            return;
        }

        // Returns the full path of the directory
        //arrayStringSongDirectory = Directory.GetDirectories(path);

        // Gets the folder names only
        DirectoryInfo main = new DirectoryInfo(path);
        DirectoryInfo[] sub = main.GetDirectories();
        foreach (DirectoryInfo x in sub)
        {
            listStringSongDirectory.Add(x.Name);
        }
        // Sort the names
        listStringSongDirectory.Sort();

        // Each name gets its own button
        for (int i = 0; i < listStringSongDirectory.Count; i++)
        {
            Button newBtn = Instantiate(buttonSongIndividual);
            // Use custom name via _customname.txt
            if (File.Exists(path + "/" + listStringSongDirectory[i] + "/_customname.txt"))
            {
                StreamReader reader = new StreamReader(path + "/" + listStringSongDirectory[i] + "/_customname.txt");
                string buttonName = reader.ReadLine();
                reader.Close();

                newBtn.name = buttonName;
                newBtn.GetComponentInChildren<Text>().text = buttonName;
            }
            // Use default name if _customname.txt doesn't exist
            else
            {
                newBtn.name = listStringSongDirectory[i];
                newBtn.GetComponentInChildren<Text>().text = listStringSongDirectory[i];
            }
            newBtn.transform.SetParent(rectSongListParent.transform);
            newBtn.transform.localPosition = Vector3.down * i * floatVertDistanceBetweenButtons;
            newBtn.transform.localScale = Vector3.one;
        }
        // Destroy the template button
        Destroy(buttonSongIndividual.gameObject);
        buttonSongIndividual = null;
    }

    private void Update()
    {
        for (int i = 0; i < groupOptionsMenuPage.Length; i++)
        {
            groupOptionsMenuPage[i].SetActive(i == intOptionsMenuPage);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (objectOptionsMenu.activeSelf)
            {
                ToggleMenuOptions();
            }
        }
    }

    public void RefreshTexts()
    {
        textDisplayAccuracy.text = "Accuracy Tolerance: " + PlayerSetting.setting.intAccuracyTolerance.ToString() + "%";
        if (PlayerSetting.setting.intGameOffset != 0)
        {
            textDisplayAccuracy.text += " | Offset: " + PlayerSetting.setting.intGameOffset.ToString() + "ms";
        }
        textDisplayMods.text = "";
        if (PlayerSetting.setting.modDisableScore)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "DisableScore";
        }
        if (PlayerSetting.setting.modChartCluster)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "Cluster";
        }
        if (PlayerSetting.setting.modChartFlip)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ChartFlip";
        }
        if (PlayerSetting.setting.modChartHell)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "Hell";
        }
        if (PlayerSetting.setting.modChartMirror)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ChartMirror";
        }
        if (PlayerSetting.setting.modChartRain)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "Rain";
        }
        if (PlayerSetting.setting.modScreenFlip)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ScreenFlip";
        }
        if (PlayerSetting.setting.modScreenMirror)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ScreenMirror";
        }
    }

    public void PlaySong(string songName, int gameType, int gameStage)
    {
        // Store song name, game type, game stage, and custom song (bool) information for use in the game scene
        Game_Control.stringSongFileName = songName;
        Game_Control.intChartGameType = gameType;
        Game_Control.intChartGameChart = gameStage;
        Game_Control.stringModList = textDisplayMods.text;
        Game_Control.boolCustomSong = isLoadCustomSongs;

        SceneManager.LoadScene(stringSceneNameGame);
    }

    public void UseSongFolder(Button folder)
    {
#if UNITY_EDITOR
        Debug.Log("Clicked on \"" + folder.name + "\" folder.");
#endif
        // TODO: Display chart information from first chart in the folder
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene(stringSceneNameTitle);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(stringSceneNameGame);
    }

    public void ToggleMenuOptions()
    {
        if (objectOptionsMenu.activeSelf)
        {
            PlayerSetting.setting.Save();
        }

        objectOptionsMenu.SetActive(!objectOptionsMenu.activeSelf);
    }
    public void ToggleMenuPage(int page)
    {
        intOptionsMenuPage += page;
        if (intOptionsMenuPage < 0)
        {
            intOptionsMenuPage = 0;
        }
        if (intOptionsMenuPage >= groupOptionsMenuPage.Length)
        {
            intOptionsMenuPage = groupOptionsMenuPage.Length - 1;
        }
    }

    public void AdjustAccuracyTolerance(int mod)
    {
        PlayerSetting.setting.intAccuracyTolerance += mod;
    }
}
