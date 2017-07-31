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
    private string stringSongSelectedCurrent = "";

    public Image imagePlayerLevelMax;
    public Text textPlayerLevel;
    public Text textPlayerScore;
    public Image imageScoreGauge;

    public ScrollRect scrollViewChartList;
    public RectTransform rectChartListParent;
    public SongMenu_ButtonChart buttonChartIndividual;
    public Vector2 sizePositionPerButton = Vector2.one;
    private int intGameType = 0;
    private int intGameChart = 0;

    public GameObject objectGroupDetails;
    public Text textDetailsHeader;
    public Text textDetailsBody;
    public Text textDetailsWarning;
    public Text textDetailsRecord;

    public Slider sliderScrollSpeed;
    public Text textDisplayScrollSpeed;
    public Text textDisplayAccuracy;
    public Text textDisplayMods;

    public GameObject objectOptionsMenu;
    public GameObject[] groupOptionsMenuPage;
    private int intOptionsMenuPage = 0;
    public Text textOptionsAccuracyTolerance;
    public Slider sliderOptionsAccuracyTolerance;
    public Slider sliderOptionsMouseSensitivity;
    public Text textOptionsMouseSensitivity;
    public Text textOptionsGameOffset;
    public Toggle toggleOptionsVerticalSync;
    public Toggle toggleOptionsInterfaceSongDetails;
    public Toggle toggleOptionsInterfaceAccuracy;
    public Toggle toggleOptionsDisplayCombo;
    public Toggle toggleOptionsDisplayJudgmentPerHit;
    public Toggle toggleOptionsDisplayJudgmentCounter;

    public bool isHighscoreDisabledByMods = false;
    public bool isHighscoreDisabledByChart = false;

    void Start()
    {
        objectGroupDetails.SetActive(false);
        objectOptionsMenu.SetActive(false);

        // Has already reached maximum level (100), display score only.
        if (PlayerSetting.setting.boolPlayerLevelMax)
        {
            imagePlayerLevelMax.gameObject.SetActive(true);
            textPlayerLevel.gameObject.SetActive(false);
            textPlayerScore.text = PlayerSetting.setting.GetScore();
            imageScoreGauge.fillAmount = 1f;
        }
        // Otherwise; show level, score, and score required to next level.
        else
        {
            int currentLevel = PlayerSetting.setting.GetPlayerLevel();
            int prevLevelScore = PlayerSetting.setting.GetLevelScoreRequirement(currentLevel - 1);
            int currentScore = PlayerSetting.setting.ConvertListIntToInt(PlayerSetting.setting.intPlayerTotalScore);
            int nextLevelScore = PlayerSetting.setting.GetLevelScoreRequirement(currentLevel);

            imagePlayerLevelMax.gameObject.SetActive(false);
            textPlayerLevel.gameObject.SetActive(true);
            textPlayerLevel.text = currentLevel.ToString();
            textPlayerScore.text = PlayerSetting.setting.GetScore() + " (Next Level: " +
                (nextLevelScore - currentScore).ToString("n0") + ")";
            imageScoreGauge.fillAmount = 1f * (currentScore - prevLevelScore) / (nextLevelScore - prevLevelScore);
        }
        sliderScrollSpeed.value = PlayerSetting.setting.intScrollSpeed;
        sliderOptionsAccuracyTolerance.value = PlayerSetting.setting.intAccuracyTolerance;
        sliderOptionsMouseSensitivity.value = PlayerSetting.setting.floatMouseSensitivity;
        toggleOptionsVerticalSync.isOn = PlayerSetting.setting.enableVSync;
        toggleOptionsInterfaceSongDetails.isOn = PlayerSetting.setting.enableInterfaceSongDetails;
        toggleOptionsInterfaceAccuracy.isOn = PlayerSetting.setting.enableInterfaceAccuracy;
        toggleOptionsDisplayCombo.isOn = PlayerSetting.setting.enableDisplayCombo;
        toggleOptionsDisplayJudgmentPerHit.isOn = PlayerSetting.setting.enableDisplayNoteJudgment;
        toggleOptionsDisplayJudgmentCounter.isOn = PlayerSetting.setting.enableDisplayNoteHitCounterSmall;

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

        //PlayerSetting.setting.Load();
        RefreshTexts();
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
        isHighscoreDisabledByMods = false;
        textDisplayScrollSpeed.text = "Game Scroll Speed: x" + (0.1f * PlayerSetting.setting.intScrollSpeed).ToString("f1");
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
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modChartCluster)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "Cluster";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modChartFlip)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ChartFlip";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modChartHell)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "Hell";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modChartMirror)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ChartMirror";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modChartRain)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "Rain";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modChartRandom)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ChartRandom";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modScreenFlip)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ScreenFlip";
            isHighscoreDisabledByMods = true;
        }
        if (PlayerSetting.setting.modScreenMirror)
        {
            if (textDisplayMods.text != "")
            {
                textDisplayMods.text += ", ";
            }
            textDisplayMods.text += "ScreenMirror";
            isHighscoreDisabledByMods = true;
        }

        textOptionsAccuracyTolerance.text = PlayerSetting.setting.intAccuracyTolerance.ToString() + "%";
        textOptionsMouseSensitivity.text = (PlayerSetting.setting.floatMouseSensitivity * 100f).ToString("f2") + "%";
        textOptionsGameOffset.text = PlayerSetting.setting.intGameOffset.ToString() + " ms";

        textDetailsRecord.gameObject.SetActive(!isHighscoreDisabledByMods && !isHighscoreDisabledByChart);
        textDetailsWarning.gameObject.SetActive(isHighscoreDisabledByMods || isHighscoreDisabledByChart);
    }

    public void PlaySong()
    {
        // Store song name, game type, game stage, and custom song (bool) information for use in the game scene
        Game_Control.stringSongFileName = stringSongSelectedCurrent;
        Game_Control.intChartGameType = intGameType;
        Game_Control.intChartGameChart = intGameChart;
        Game_Control.stringModList = textDisplayMods.text;
        Game_Control.boolCustomSong = isLoadCustomSongs;

        SceneManager.LoadScene(stringSceneNameGame);
    }

    public void UseSongFolder(Button folder)
    {
#if UNITY_EDITOR
        Debug.Log("Clicked on \"" + folder.name + "\" folder.");
#endif
        stringSongSelectedCurrent = folder.name;
        return;

        // Clear all chart buttons on scene
        SongMenu_ButtonChart[] listOldButtons = FindObjectsOfType<SongMenu_ButtonChart>();
        foreach(SongMenu_ButtonChart x in listOldButtons)
        {
            Destroy(x.gameObject);
        }

        // Display chart information from first chart in the folder

        // Check for each chart in the folder
        SongMenu_ButtonChart firstChart = null;
        StreamReader reader;
        ChartData chartData = ScriptableObject.CreateInstance(typeof(ChartData)) as ChartData;
        bool gameModeExist = false;
        int gameModeExists = 0;
        // Game mode ID
        for (int gameModeID = 0; gameModeID < 5; gameModeID++)
        {
            gameModeExist = false;

            // Chart ID
            for (int chartID = 0; ; chartID++)
            {
                string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath + "/" + folder.name + "/" + folder.name + "-" + gameModeID.ToString() + "-" + chartID.ToString() + ".txt";
                if (File.Exists(path))
                {
                    if (!gameModeExist)
                    {
                        gameModeExist = true;
                        gameModeExists++;
                    }

                    reader = new StreamReader(path);
                    string input = reader.ReadToEnd();
                    reader.Close();
                    JsonUtility.FromJsonOverwrite(input, chartData);

                    // Button info and positioning
                    SongMenu_ButtonChart nb = Instantiate(buttonChartIndividual);
                    nb.transform.SetParent(rectChartListParent);
                    nb.transform.localScale = Vector3.one;
                    nb.transform.localPosition = (Vector3.right * sizePositionPerButton.x * chartID) + (Vector3.down * sizePositionPerButton.y * gameModeExists);
                    nb.intGameMode = gameModeID;
                    nb.intChart = chartID;

                    // Button display
                    string stringText = "";
                    switch(gameModeID)
                    {
                        case 0: stringText = "LN"; break;
                        case 1: stringText = "DB"; break;
                        case 2: stringText = "QD"; break;
                        case 3: stringText = "PWF"; break;
                        case 4: stringText = "C&T"; break;
                    }
                    stringText += " " + (chartID + 1).ToString() + "\nLvl " + chartData.chartLevel.ToString();
                    nb.textButton.text = stringText;

                    // If it is the first chart found, it will be selected first
                    if (firstChart == null)
                    {
                        firstChart = nb;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        UseChartInfo(firstChart);
    }

    public void UseChartInfo(SongMenu_ButtonChart button)
    {
        intGameType = button.intGameMode;
        intGameChart = button.intChart;
        string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath + "/" + stringSongSelectedCurrent + "/" +
            stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + ".txt";

        ChartData chartData = ScriptableObject.CreateInstance(typeof(ChartData)) as ChartData;
        StreamReader reader = new StreamReader(path);
        string input = reader.ReadToEnd();
        reader.Close();
        JsonUtility.FromJsonOverwrite(input, chartData);

        string stringMode = "";
        switch (chartData.chartGameType)
        {
            case 0: stringMode = "LN"; break;
            case 1: stringMode = "DB"; break;
            case 2: stringMode = "QD"; break;
            case 3: stringMode = "PWF"; break;
            case 4: stringMode = "C&T"; break;
        }
        float actualLength = chartData.songLength * 60f / chartData.songTempo;

        textDetailsHeader.text = chartData.songName;
        textDetailsBody.text =
            chartData.songArtist + "\n\n" +
            "Chart Producer: " + chartData.chartDeveloper + "\n" +
            "Mode: " + stringMode + "\n" +
            "Level: " + chartData.chartLevel.ToString() + "\n" +
            "Length: " + Mathf.Floor(actualLength / 60f).ToString() + ":" + (actualLength % 60f).ToString("f2") + "\n" +
            "BPM: " + chartData.songTempo.ToString("f0") + "\n" +
            "Judge Level: " + (chartData.chartJudge + 1).ToString();
        textDetailsRecord.text = "Best Accuracy: " + PlayerPrefs.GetFloat(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString(), 0f).ToString("f2") + "%";

        isHighscoreDisabledByChart = !chartData.isHighScoreAllowed;
        RefreshTexts();
        objectGroupDetails.SetActive(true);
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

    public void AdjustScrollSpeed()
    {
        PlayerSetting.setting.intScrollSpeed = Mathf.RoundToInt(sliderScrollSpeed.value);
    }
    public void AdjustAccuracyTolerance(int mod)
    {
        sliderOptionsAccuracyTolerance.value = PlayerSetting.setting.intAccuracyTolerance = Mathf.Clamp(PlayerSetting.setting.intAccuracyTolerance + mod, 0, 100);
    }
    public void AdjustAccuracyToleranceAlt()
    {
        PlayerSetting.setting.intAccuracyTolerance = Mathf.RoundToInt(sliderOptionsAccuracyTolerance.value);
    }
    public void AdjustMouseSensitivity()
    {
        PlayerSetting.setting.floatMouseSensitivity = sliderOptionsMouseSensitivity.value;
    }
    public void AdjustGameOffset(int mod)
    {
        PlayerSetting.setting.intGameOffset = Mathf.Clamp(PlayerSetting.setting.intGameOffset + mod, -100, 100);
    }
    public void AdjustVerticalSync()
    {
        PlayerSetting.setting.enableVSync = toggleOptionsVerticalSync.isOn;
    }
    public void AdjustInterfaceSongDetails()
    {
        PlayerSetting.setting.enableInterfaceSongDetails = toggleOptionsInterfaceSongDetails.isOn;
    }
    public void AdjustInterfaceAccuracy()
    {
        PlayerSetting.setting.enableInterfaceAccuracy = toggleOptionsInterfaceAccuracy.isOn;
    }
    public void AdjustDisplayCombo()
    {
        PlayerSetting.setting.enableDisplayCombo = toggleOptionsDisplayCombo.isOn;
    }
    public void AdjustDisplayJudgmentPerHit()
    {
        PlayerSetting.setting.enableDisplayNoteJudgment = toggleOptionsDisplayJudgmentPerHit.isOn;
    }
    public void AdjustDisplayJudgmentCounter()
    {
        PlayerSetting.setting.enableDisplayNoteHitCounterSmall = toggleOptionsDisplayJudgmentCounter.isOn;
    }
}
