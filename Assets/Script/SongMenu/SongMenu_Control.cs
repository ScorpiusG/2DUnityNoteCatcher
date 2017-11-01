using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongMenu_Control : MonoBehaviour
{
    public SongMenu_SongList[] listOfficialSongList = { };
    private bool isLoadCustomSongs = false;

    public string stringSceneNameTitle = "Title";
    public string stringSceneNameGame = "Game";

    public string stringSongDirectoryPath = "/Songs";
    //public string[] arrayStringSongDirectory;
    private List<string> listStringSongDirectory = new List<string>();

    public RectTransform rectSongListParent;
    public Button buttonSongIndividual;
    public Scrollbar scrollbarListSong;
    public float floatVertDistanceBetweenButtons = 80f;
    private static string stringSongSelectedCurrent = "";

    public Image imagePlayerLevelMax;
    public Text textPlayerLevel;
    public Text textPlayerScore;
    public Text textPlayerScoreNextLevel;
    public Image imageScoreGauge;

    public ScrollRect scrollViewChartList;
    public RectTransform rectChartListParent;
    private static int intChartListPage = 0;
    private int intGameChartHighest = 0;
    private int intChartListPageMaximum = 0;
    public Button[] arrayButtonChartListScroll;
    public SongMenu_ButtonChart buttonChartIndividual;
    public Vector2 sizePositionPerButton = Vector2.one;
    private static int intGameType = -1;
    private static int intGameChart = -1;

    public GameObject objectGroupDetails;
    public Vector3 positionGroupDetailsInit;
    public RawImage rawImageDetailsBackground;
    public Text textDetailsHeader;
    public Text textDetailsBody;
    public Text textDetailsWarning;
    public Text textDetailsRecord;
    public GameObject objectButtonPlay;

    public GameObject objectDetailsDescriptionIndicator;
    public GameObject objectDetailsDescription;
    private bool boolDetailsDescriptionEnable = false;
    public Text textDetailsDescription;

    public Slider sliderScrollSpeed;
    public Text textDisplayScrollSpeed;
    public Text textDisplayAccuracy;
    public Text textDisplayMods;

    public GameObject objectOptionsMenu;
    public GameObject[] groupOptionsMenuPage;
    private int intOptionsMenuPage = 0;
    public Text textOptionsAccuracyTolerance;
    public Slider sliderOptionsAccuracyTolerance;
    public Text textOptionsBackgroundBrightness;
    public Slider sliderOptionsBackgroundBrightness;
    public Slider sliderOptionsMouseSensitivity;
    public Text textOptionsMouseSensitivity;
    public Text textOptionsGameOffset;
    public Slider sliderOptionsVolumeMusic;
    public Text textOptionsVolumeMusic;
    public Slider sliderOptionsVolumeEffect;
    public Text textOptionsVolumeEffect;
    public Toggle toggleOptionsVerticalSync;
    public Toggle toggleOptionsInterfaceSongDetails;
    public Toggle toggleOptionsInterfaceAccuracy;
    public Toggle toggleOptionsDisplayCombo;
    public Toggle toggleOptionsDisplayRecordGhost;
    public Toggle toggleOptionsDisplayJudgmentPerHit;
    public Toggle toggleOptionsDisplayJudgmentCounter;
    public Toggle toggleOptionsObjectBeatPulse;
    public Toggle toggleOptionsAssistTickSound;

    public AudioSource audioSourcePreview;
    public PlaySoundEffect mSoundPlayer;
    public AudioClip clipRecordDelete;

    public bool isHighscoreDisabledByMods = false;
    public bool isHighscoreDisabledByChart = false;

    public float floatHighscoreDeleteTimer = 0f;

    void Start()
    {
        // Variable initialization
        positionGroupDetailsInit = objectGroupDetails.transform.position;

        // Object initialization
        objectGroupDetails.SetActive(false);
        objectOptionsMenu.SetActive(false);
        objectButtonPlay.SetActive(false);
        buttonChartIndividual.gameObject.SetActive(false);
        objectDetailsDescription.SetActive(false);
        rawImageDetailsBackground.gameObject.SetActive(false);
        foreach (Button x in arrayButtonChartListScroll)
        {
            x.interactable = false;
        }

        textPlayerScore.text = PlayerSetting.setting.GetScore();
        // Has already reached maximum level (100), display score only.
        if (PlayerSetting.setting.boolPlayerLevelMax)
        {
            imagePlayerLevelMax.gameObject.SetActive(true);
            textPlayerLevel.gameObject.SetActive(false);
            textPlayerScoreNextLevel.gameObject.SetActive(false);
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
            textPlayerScoreNextLevel.gameObject.SetActive(true);
            textPlayerScoreNextLevel.text = "(" + Translator.GetStringTranslation("SONGMENU_PLAYERNEXTLEVELSCORE", "Next Level:") + " " + (nextLevelScore - currentScore).ToString("n0") + ")";
            imageScoreGauge.fillAmount = 1f * (currentScore - prevLevelScore) / (nextLevelScore - prevLevelScore);
        }
        sliderScrollSpeed.value = PlayerSetting.setting.intScrollSpeed;
        sliderOptionsAccuracyTolerance.value = PlayerSetting.setting.intAccuracyTolerance;
        sliderOptionsBackgroundBrightness.value = PlayerSetting.setting.floatBackgroundBrightness;
        sliderOptionsMouseSensitivity.value = PlayerSetting.setting.floatMouseSensitivity;
        sliderOptionsVolumeMusic.value = PlayerSetting.setting.floatVolumeMusic;
        audioSourcePreview.volume = PlayerSetting.setting.floatVolumeMusic;
        sliderOptionsVolumeEffect.value = PlayerSetting.setting.floatVolumeEffect;
        toggleOptionsVerticalSync.isOn = PlayerSetting.setting.enableVSync;
        toggleOptionsInterfaceSongDetails.isOn = PlayerSetting.setting.enableInterfaceSongDetails;
        toggleOptionsInterfaceAccuracy.isOn = PlayerSetting.setting.enableInterfaceAccuracy;
        toggleOptionsDisplayCombo.isOn = PlayerSetting.setting.enableDisplayCombo;
        toggleOptionsDisplayRecordGhost.isOn = PlayerSetting.setting.enableDisplayRecordGhost;
        toggleOptionsDisplayJudgmentPerHit.isOn = PlayerSetting.setting.enableDisplayNoteJudgment;
        toggleOptionsDisplayJudgmentCounter.isOn = PlayerSetting.setting.enableDisplayNoteHitCounterSmall;
        toggleOptionsObjectBeatPulse.isOn = PlayerSetting.setting.enableNoteAndCatcherHighlightBeatPulse;
        toggleOptionsAssistTickSound.isOn = PlayerSetting.setting.enableAssistTickSound;

        string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath;
#if UNITY_EDITOR
        Debug.Log(path);
#endif

        // If it's a custom song list, get all the folders present
        if (listOfficialSongList.Length <= 0)
        {
            isLoadCustomSongs = true;

            // The "Songs" folder does not exist in the game directory
            if (!Directory.Exists(path))
            {
#if UNITY_EDITOR
                Debug.LogWarning("WARNING: \"Songs\" folder does not exist!");
#endif
                Notification.Display("WARNING\nThe \"Songs\" folder does not exist!", Color.white);

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
        }
        // Otherwise, use its own list
        else
        {
            isLoadCustomSongs = false;
            foreach (SongMenu_SongList x in listOfficialSongList)
            {
                foreach (string y in x.listStringSongList)
                {
                    listStringSongDirectory.Add(y);
                }
            }
        }
        // Sort the names
        listStringSongDirectory.Sort();

        // Last tapped button in same session
        Button firstBtn = null;
        Button prevBtn = null;
        float scrollbarListSongValue = 0f;
        // Each name gets its own button
        for (int i = 0; i < listStringSongDirectory.Count; i++)
        {
            Button newBtn = Instantiate(buttonSongIndividual);
            newBtn.name = listStringSongDirectory[i];
            // Use custom name via _customname.txt
            if (isLoadCustomSongs && File.Exists(path + "/" + listStringSongDirectory[i] + "/_customname.txt"))
            {
                StreamReader reader = new StreamReader(path + "/" + listStringSongDirectory[i] + "/_customname.txt");
                string buttonName = reader.ReadLine();
                reader.Close();

                newBtn.GetComponentInChildren<Text>().text = buttonName;
            }
            // Official songs
            else if (!isLoadCustomSongs)
            {
                TextAsset text = (TextAsset)Resources.Load(listStringSongDirectory[i] + "_customname", typeof(TextAsset));
                if (text != null)
                {
                    newBtn.GetComponentInChildren<Text>().text = text.text;
                }
                else
                {
                    newBtn.GetComponentInChildren<Text>().text = listStringSongDirectory[i];
                }
            }
            // Use default name if _customname.txt doesn't exist
            else
            {
                newBtn.GetComponentInChildren<Text>().text = listStringSongDirectory[i];
            }
            newBtn.transform.SetParent(rectSongListParent.transform);
            newBtn.transform.localPosition = Vector3.down * i * floatVertDistanceBetweenButtons;
            newBtn.transform.localScale = Vector3.one;

            if (firstBtn == null)
            {
                firstBtn = newBtn;
            }
            if (stringSongSelectedCurrent == listStringSongDirectory[i])
            {
                prevBtn = newBtn;
                scrollbarListSongValue = 1f - (1f * i / listStringSongDirectory.Count);
            }
        }
        // Destroy the template button
        Destroy(buttonSongIndividual.gameObject);
        buttonSongIndividual = null;

        // Auto-press the last remembered button
        if (prevBtn != null)
        {
            UseSongFolder(prevBtn);
            scrollbarListSong.value = scrollbarListSongValue;
        }
        // Otherwise, auto-press the first button in the list
        else
        {
            UseSongFolder(firstBtn);
        }

        //PlayerSetting.setting.Load();
        RefreshTexts();
    }

    private void Update()
    {
        rectChartListParent.transform.localPosition = Vector3.Lerp(
            rectChartListParent.transform.localPosition,
            Vector3.left * intChartListPage * sizePositionPerButton.x,
            Time.deltaTime * 16f);

        for (int i = 0; i < groupOptionsMenuPage.Length; i++)
        {
            groupOptionsMenuPage[i].SetActive(i == intOptionsMenuPage);
        }
    }
    private void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (objectOptionsMenu.activeSelf)
            {
                ToggleMenuOptions();
            }
            else
            {
                LoadTitleScene();
            }
        }

        // Show/Hide chart description by tapping F1.
        if (Input.GetKeyDown(KeyCode.F1) && textDetailsDescription.text.Length > 0)
        {
            boolDetailsDescriptionEnable = !boolDetailsDescriptionEnable;
        }
        if (boolDetailsDescriptionEnable && textDetailsDescription.text.Length <= 0)
        {
            boolDetailsDescriptionEnable = false;
        }
        objectDetailsDescription.SetActive(boolDetailsDescriptionEnable);
        //objectDetailsDescription.SetActive(Input.GetKey(KeyCode.F1) && textDetailsDescription.text.Length > 0);
        if (textDetailsDescription.text.Length > 0 && !objectDetailsDescriptionIndicator.activeSelf)
        {
            objectDetailsDescriptionIndicator.SetActive(true);
        }
        else if (textDetailsDescription.text.Length <= 0 && objectDetailsDescriptionIndicator.activeSelf)
        {
            objectDetailsDescriptionIndicator.SetActive(false);
        }

        // Erase high score records by holding F9 for five seconds.
        if (Input.GetKey(KeyCode.F9))
        {
            floatHighscoreDeleteTimer += Time.unscaledDeltaTime;
        }
        else
        {
            floatHighscoreDeleteTimer -= Time.unscaledDeltaTime * 4f;
            if (floatHighscoreDeleteTimer < 0f) floatHighscoreDeleteTimer = 0f;
        }
        if (floatHighscoreDeleteTimer > 5f)
        {
            floatHighscoreDeleteTimer = 0f;

            if (intGameType >= 0 && intGameChart >= 0)
            {
                mSoundPlayer.PlaySound(clipRecordDelete);
                PlayerPrefs.DeleteKey(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + "-recordaccuracy");
                PlayerPrefs.DeleteKey(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + "-playcount");
                PlayerPrefs.Save();
                textDetailsRecord.text = Translator.GetStringTranslation("SONGMENU_RECORDDELETE", "Records deleted.");
            }
        }
    }

    public void RefreshTexts()
    {
        isHighscoreDisabledByMods = false;
        textDisplayScrollSpeed.text = Translator.GetStringTranslation("SONGMENU_SCROLLSPEED", "Note Scroll Speed:") + " x" + (0.1f * PlayerSetting.setting.intScrollSpeed).ToString("f1");
        textDisplayAccuracy.text = Translator.GetStringTranslation("SONGMENU_ACCURACYTOLERANCE", "Accuracy Tolerance:") + " " + PlayerSetting.setting.intAccuracyTolerance.ToString() + "%";
        if (PlayerSetting.setting.intGameOffset != 0)
        {
            textDisplayAccuracy.text += " | " + Translator.GetStringTranslation("SONGMENU_GAMEOFFSET", "Offset:") + " " + PlayerSetting.setting.intGameOffset.ToString() + "ms";
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
        textOptionsBackgroundBrightness.text = (PlayerSetting.setting.floatBackgroundBrightness * 100f).ToString("f2") + "%";
        textOptionsMouseSensitivity.text = (PlayerSetting.setting.floatMouseSensitivity * 100f).ToString("f2") + "%";
        textOptionsGameOffset.text = PlayerSetting.setting.intGameOffset.ToString() + " ms";
        textOptionsVolumeMusic.text = (PlayerSetting.setting.floatVolumeMusic * 100f).ToString("f2") + "%";
        textOptionsVolumeEffect.text = (PlayerSetting.setting.floatVolumeEffect * 100f).ToString("f2") + "%";

        textDetailsRecord.gameObject.SetActive(!isHighscoreDisabledByMods && !isHighscoreDisabledByChart);
        textDetailsWarning.gameObject.SetActive(isHighscoreDisabledByMods || isHighscoreDisabledByChart);
    }

    public void PlaySong(bool isAutoplay)
    {
        // Store song name, game type, game stage, and custom song (bool) information for use in the game scene
        Game_Control.stringSongFileName = stringSongSelectedCurrent;
        Game_Control.intChartGameType = intGameType;
        Game_Control.intChartGameChart = intGameChart;
        Game_Control.stringModList = textDisplayMods.text;
        Game_Control.boolCustomSong = isLoadCustomSongs;

        Game_Control.boolAutoplay = isAutoplay;

        //SceneManager.LoadScene(stringSceneNameGame);
        LoadGameScene();
    }

    public void UseSongFolder(Button folder)
    {
#if UNITY_EDITOR
        Debug.Log("Clicked on \"" + folder.name + "\" folder.");
#endif
        stringSongSelectedCurrent = folder.name;
        //intGameType = -1;
        //intGameChart = -1;

        // The selected folder is darkened. The rest remain at normal color.
        Button[] listAllButtons = FindObjectsOfType<Button>();
        foreach(Button x in listAllButtons)
        {
            x.image.color = Color.white;
        }
        folder.image.color = Color.grey;

        // Clear all chart buttons on scene
        SongMenu_ButtonChart[] listOldButtons = FindObjectsOfType<SongMenu_ButtonChart>();
        foreach(SongMenu_ButtonChart x in listOldButtons)
        {
            if (x.gameObject.activeSelf)
            {
                Destroy(x.gameObject);
            }
        }

        // Display chart information from first chart in the folder

        // Check for each chart in the folder
        SongMenu_ButtonChart firstChart = null;
        StreamReader reader;
        ChartData chartData = ScriptableObject.CreateInstance(typeof(ChartData)) as ChartData;
        bool gameModeExist = false;
        int gameModeExists = 0;
        bool boolResetChartPage = false;
        intGameChartHighest = 0;
        // Game mode ID
        for (int gameModeID = 0; gameModeID < 5; gameModeID++)
        {
            gameModeExist = false;

            // Chart ID
            for (int chartID = 0; ; chartID++)
            {
                if (intGameChartHighest < chartID)
                {
                    intGameChartHighest = chartID;
                }

                string info = "";
                if (isLoadCustomSongs)
                {
                    string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath + "/" + folder.name + "/" + folder.name + "-" + gameModeID.ToString() + "-" + chartID.ToString() + ".txt";
                    if (File.Exists(path))
                    {
                        reader = new StreamReader(path);
                        info = reader.ReadToEnd();
                        reader.Close();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    string path = folder.name + "-" + gameModeID.ToString() + "-" + chartID.ToString();
                    TextAsset text = (TextAsset)Resources.Load(path, typeof(TextAsset));
                    if (text != null)
                    {
                        info = text.text;
                    }
                    else
                    {
                        break;
                    }
                }

                if (!gameModeExist)
                {
                    gameModeExist = true;
                }

                // Create button from chart information
                if (info.Length > 0)
                {
                    JsonUtility.FromJsonOverwrite(info, chartData);

                    // Button info and positioning
                    SongMenu_ButtonChart nb = Instantiate(buttonChartIndividual);
                    nb.gameObject.SetActive(true);
                    nb.transform.SetParent(rectChartListParent);
                    nb.transform.localScale = Vector3.one;
                    nb.transform.localPosition = (Vector3.right * ((sizePositionPerButton.x * chartID) + 4f)) + (Vector3.down * ((sizePositionPerButton.y * gameModeExists) + 2f));
                    nb.intGameMode = gameModeID;
                    nb.intChart = chartID;

                    // Button display
                    string stringMode = "";
                    switch (gameModeID)
                    {
                        case 0: stringMode = "LN"; break;
                        case 1: stringMode = "DB"; break;
                        case 2: stringMode = "QD"; break;
                        case 3: stringMode = "ND"; break;
                        /*
                        case 3: stringMode = "PWF"; break;
                        case 4: stringMode = "SP"; break;
                        case 5: stringMode = "ULT"; break;
                        case 6: stringMode = "BC"; break;
                        case 7: stringMode = "ND"; break;
                        */
                    }
                    nb.textButton.text = stringMode + " " + (chartID + 1).ToString() + " - *" + chartData.chartLevel.ToString();

                    // If it is the first chart found, it will be selected first
                    if (firstChart == null)
                    {
                        boolResetChartPage = true;
                        firstChart = nb;
                    }
                    // If it was previously selected before, select this again
                    if (intGameType >= 0 && intGameType == gameModeID && intGameChart >= 0 && intGameChart == chartID)
                    {
                        boolResetChartPage = false;
                        firstChart = nb;
                    }
                }
            }

            if (gameModeExist)
            {
                gameModeExists++;
            }
        }

        // Set maximum chart list scroll page
        intChartListPageMaximum = (intGameChartHighest - 3) / 2;
        if (intChartListPageMaximum < 0) intChartListPageMaximum = 0;
        foreach (Button x in arrayButtonChartListScroll)
        {
            x.interactable = intChartListPageMaximum > 0;
        }
        if (boolResetChartPage)
        {
            intGameType = -1;
            intGameChart = -1;
            intChartListPage = 0;
            rectChartListParent.transform.localPosition = Vector3.zero;
        }

        // Stop the song preview
        StopCoroutine("LoadClip");
        audioSourcePreview.Stop();

        // Automatically select the first chart in the list
        if (firstChart != null)
        {
            scrollViewChartList.gameObject.SetActive(true);
            foreach (Button x in arrayButtonChartListScroll)
            {
                x.gameObject.SetActive(true);
            }
            UseChartInfo(firstChart);

            // Load the song file (.ogg) in the folder
            StartCoroutine("LoadClip", folder.name);

            // Get background texture
            Texture textureBackground = null;
            if (isLoadCustomSongs)
            {
                string path = "/Songs/" + folder.name + "/background.jpg";
                if (!File.Exists(Directory.GetCurrentDirectory() + path))
                {
#if UNITY_EDITOR
                    Debug.LogWarning("WARNING: The image file does not exist! Path: " + path);
#endif
                }
                else
                {
                    WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + path);
                    //textureBackground = www.texture;
                    //www.LoadImageIntoTexture(textureBackground);
                    textureBackground = www.textureNonReadable;
                }
            }
            else
            {
                string path = "Songs/" + folder.name + "_background";
                textureBackground = Resources.Load(path) as Texture;
            }
            if (textureBackground != null)
            {
                rawImageDetailsBackground.gameObject.SetActive(true);
                rawImageDetailsBackground.texture = textureBackground;
            }
            else
            {
                rawImageDetailsBackground.gameObject.SetActive(false);
            }

            // Play button enable and animate
            objectButtonPlay.SetActive(true);
            objectButtonPlay.GetComponent<Animator>().Play("clip");
        }
        else
        {
            objectButtonPlay.SetActive(false);
            foreach (Button x in arrayButtonChartListScroll)
            {
                x.gameObject.SetActive(false);
            }
            rawImageDetailsBackground.gameObject.SetActive(false);
            scrollViewChartList.gameObject.SetActive(false);
            textDetailsHeader.text = "";
            textDetailsBody.text =
                Translator.GetStringTranslation("SONGMENU_CHARTNONEXISTS",
                "There are no map charts in\n" +
                "this song folder's contents.\n" +
                "\n" +
                "Please make sure your map\n" +
                "chart files are named in the\n" +
                "correct format.");
            textDetailsDescription.text = "";
            textDetailsRecord.text = "";
        }
        
        // iTween song window tween effect
        float tweenDuration = 0.31f;
        objectGroupDetails.SetActive(true);
        objectGroupDetails.transform.position = positionGroupDetailsInit + Vector3.right * 1000f;
        iTween.MoveTo(objectGroupDetails, positionGroupDetailsInit, tweenDuration);
        /*
        Text[] tGrpDts = objectGroupDetails.GetComponentsInChildren<Text>();
        foreach (Text x in tGrpDts)
        {
            x.color = new Color(1f, 1f, 1f, 0f);
            iTween.ColorTo(x.gameObject, Color.white, tweenDuration);
        }
        Image[] iGrpDts = objectGroupDetails.GetComponentsInChildren<Image>();
        foreach (Image x in iGrpDts)
        {
            x.color = new Color(1f, 1f, 1f, 0f);
            iTween.ColorTo(x.gameObject, Color.white, tweenDuration);
        }
        */
    }
    private IEnumerator LoadClip(string name)
    {
        yield return new WaitForSeconds(1f);
        AudioClip newClip = null;

        // Custom song
        if (isLoadCustomSongs)
        {
            string url = "Songs/" + name + "/preview.ogg";
            if (!File.Exists(url))
            {
                Debug.LogError("ERROR: " + url + " does not exist.");
                yield break;
            }

            WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + url);
#if UNITY_EDITOR
            Debug.Log("Load audio file path: " + www.url);
#endif
            while (!www.isDone)
            {
                yield return null;
            }
            newClip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
        }
        // Official song
        else
        {
            newClip = (AudioClip)Resources.Load(name + "_preview", typeof(AudioClip));
            if (newClip == null)
            {
                Debug.LogError("ERROR: " + name + "_preview resource does not exist.");
                yield break;
            }
        }

        // Play preview song
        audioSourcePreview.clip = newClip;
        audioSourcePreview.Play();
    }

    public void UseChartInfo(SongMenu_ButtonChart button)
    {
        SongMenu_ButtonChart[] listOldButtons = FindObjectsOfType<SongMenu_ButtonChart>();
        foreach (SongMenu_ButtonChart x in listOldButtons)
        {
            x.imageButton.color = Color.white;
        }
        button.imageButton.color = Color.grey;

        intGameType = button.intGameMode;
        intGameChart = button.intChart;
        string input = "";
        ChartData chartData = ScriptableObject.CreateInstance(typeof(ChartData)) as ChartData;
        if (isLoadCustomSongs)
        {
            string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath + "/" + stringSongSelectedCurrent + "/" +
                stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + ".txt";
            
            StreamReader reader = new StreamReader(path);

            input = reader.ReadToEnd();
            reader.Close();
        }
        else
        {
            string path = stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString();
            TextAsset text = (TextAsset)Resources.Load(path, typeof(TextAsset));
            input = text.text;
        }
        JsonUtility.FromJsonOverwrite(input, chartData);

        string stringMode = "";
        switch (chartData.chartGameType)
        {
            case 0: stringMode = "Linear"; break;
            case 1: stringMode = "Double"; break;
            case 2: stringMode = "Quad"; break;
            case 3: stringMode = "Note Dodge"; break;
            /*
            case 3: stringMode = "Powerful"; break;
            case 4: stringMode = "Super"; break;
            case 5: stringMode = "Ultra"; break;
            case 6: stringMode = "Black Core"; break;
            case 7: stringMode = "Note Dodge"; break;
            */
        }
        stringMode = Translator.GetStringTranslation("SONGMENU_CHARTGAMETYPEBODY" + chartData.chartGameType, stringMode);
        float actualLength = chartData.songLength * 60f / chartData.songTempo;

        // Display song details
        textDetailsHeader.text = chartData.songName;
        textDetailsBody.text =
            chartData.songArtist + "\n\n" +
            Translator.GetStringTranslation("SONGMENU_CHARTDEV", "Mapchart Producer:") + " " + chartData.chartDeveloper + "\n" +
            Translator.GetStringTranslation("SONGMENU_CHARTGAMETYPEBODY", "Mapchart Mode:") + " " + stringMode + "\n" +
            Translator.GetStringTranslation("SONGMENU_CHARTLEVEL", "Mapchart Level:") + " " + chartData.chartLevel.ToString() + "\n" +
            Translator.GetStringTranslation("SONGMENU_CHARTLENGTH", "Length:") + " " + Mathf.Floor(actualLength / 60f).ToString() + ":" + (actualLength % 60f).ToString("f2") + "\n" +
            Translator.GetStringTranslation("SONGMENU_CHARTBPM", "BPM:") + " " + chartData.songTempo.ToString("f0") + "\n" +
            Translator.GetStringTranslation("SONGMENU_CHARTJUDGELEVEL", "Judge Level:") + " " + (chartData.chartJudge + 1).ToString();

        float floatRecordAccuracy = 0f;
        int intRecordPlays = 0;
        if (isLoadCustomSongs)
        {
            floatRecordAccuracy = PlayerPrefs.GetFloat(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + "-recordaccuracy", 0f);
            intRecordPlays = PlayerPrefs.GetInt(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + "-playcount", 0);
        }
        else
        {
            floatRecordAccuracy = PlayerPrefs.GetFloat(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + "-recordaccuracy_official", 0f);
            intRecordPlays = PlayerPrefs.GetInt(stringSongSelectedCurrent + "-" + intGameType.ToString() + "-" + intGameChart.ToString() + "-playcount_official", 0);
        }
        textDetailsRecord.text =
            Translator.GetStringTranslation("SONGMENU_RECORDACCURACY", "Best Accuracy:") + " " + (100f * floatRecordAccuracy).ToString("f2") + "% | " +
            Translator.GetStringTranslation("SONGMENU_RECORDPLAYCOUNT", "Attempts:") + " " + intRecordPlays.ToString("n0");
        textDetailsDescription.text = chartData.chartDescription;

        isHighscoreDisabledByChart = !chartData.isHighScoreAllowed;
        RefreshTexts();
        /*
        objectButtonPlay.transform.rotation = Quaternion.Euler(0f, 0f, -90f);
        iTween.RotateTo(objectButtonPlay, iTween.Hash("rotation", Vector3.zero, "time", 0.31f, "isLocal", true, "easetype", iTween.EaseType.easeOutBack));
        */
    }

    public void RestartScene()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        SceneTransition.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadTitleScene()
    {
        //SceneManager.LoadScene(stringSceneNameTitle);
        SceneTransition.LoadScene(stringSceneNameTitle);
    }

    public void LoadGameScene()
    {
        //SceneManager.LoadScene(stringSceneNameGame);
        SceneTransition.LoadScene(stringSceneNameGame);
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

    public void ToggleChartListPage(int page)
    {
        intChartListPage += page;
        if (intChartListPage < 0)
        {
            intChartListPage = 0;
        }
        if (intChartListPage > intChartListPageMaximum)
        {
            intChartListPage = intChartListPageMaximum;
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
    public void AdjustBackgroundBrightness()
    {
        PlayerSetting.setting.floatBackgroundBrightness = sliderOptionsBackgroundBrightness.value;
    }
    public void AdjustMouseSensitivity()
    {
        PlayerSetting.setting.floatMouseSensitivity = sliderOptionsMouseSensitivity.value;
    }
    public void AdjustGameOffset(int mod)
    {
        PlayerSetting.setting.intGameOffset = Mathf.Clamp(PlayerSetting.setting.intGameOffset + mod, -100, 100);
    }
    public void AdjustVolumeMusic()
    {
        PlayerSetting.setting.floatVolumeMusic = sliderOptionsVolumeMusic.value;
        audioSourcePreview.volume = sliderOptionsVolumeMusic.value;
    }
    public void AdjustVolumeEffect()
    {
        PlayerSetting.setting.floatVolumeEffect = sliderOptionsVolumeEffect.value;
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
    public void AdjustDisplayRecordGhost()
    {
        PlayerSetting.setting.enableDisplayRecordGhost = toggleOptionsDisplayRecordGhost.isOn;
    }
    public void AdjustDisplayJudgmentPerHit()
    {
        PlayerSetting.setting.enableDisplayNoteJudgment = toggleOptionsDisplayJudgmentPerHit.isOn;
    }
    public void AdjustDisplayJudgmentCounter()
    {
        PlayerSetting.setting.enableDisplayNoteHitCounterSmall = toggleOptionsDisplayJudgmentCounter.isOn;
    }
    public void AdjustObjectBeatPulse()
    {
        PlayerSetting.setting.enableNoteAndCatcherHighlightBeatPulse = toggleOptionsObjectBeatPulse.isOn;
    }
    public void AdjustAssistTickSound()
    {
        PlayerSetting.setting.enableAssistTickSound = toggleOptionsAssistTickSound.isOn;
    }
}
