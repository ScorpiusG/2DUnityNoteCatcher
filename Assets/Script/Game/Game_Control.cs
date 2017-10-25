using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;
//using TMPro;

public class Game_Control : MonoBehaviour
{
    public static bool boolCustomSong = true;
    public static string stringSongFileName = "test";
    public static int intChartGameType = 0;
    public static int intChartGameChart = 0;
    public static string stringModList = "";
    public static MarathonMenu_Item marathonItem = null;
    public static int intMarathonItem = 0;

    public static bool boolAutoplay = false;

    public GameObject objectAutoplayIndicator;
    public GameObject[] objectGroupType;
    public GameObject objectMouseCrosshair;
    public GameObject[] objectCatcher;
    public SpriteRenderer[] spriteRendererCatcherHighlight;

    public Camera cameraMain;
    public Camera[] cameraGame;
    public float floatCameraRotation = 0f;
    public bool boolCameraRotationNoLerp = false;
    public float floatCameraRotationChangeRate = 0f;
    public float floatCameraRotationLerpRate = 8f;

    public Game_Note noteCatchPrefab;
    private List<Game_Note> listNoteCatch = new List<Game_Note>();
    public Game_Note noteTapPrefab;
    private List<Game_Note> listNoteTap = new List<Game_Note>();
    public Color[] noteColor = { Color.blue, Color.red, Color.green, Color.yellow };
    public float floatNoteDistanceSpawn = 3f;

    public Renderer rendererPlaneBackground;
    public TextMesh textMeshComboCurrent;
    public int floatTextComboAppearComboMinimum = 4;
    public float floatTextComboScaleOnChange = 3f;
    public float floatTextComboScaleMinimum = 1f;
    public float floatTextComboScaleChangeRate = 12f;
    private float floatTextComboScaleCurrent = 1f;
    public TextMesh textMeshRecordGhost;
    public float floatPreviousRecord = 0f;
    public Color colorRecordGhostNeutral = Color.white;
    public Color colorRecordGhostBetter = Color.green;
    public Color colorRecordGhostWorse = Color.red;
    
    public Animator animatorPopup;
    public List<float> listPopupTimer = new List<float>();
    public List<string> listPopupText = new List<string>();

    public Game_AnimationJudgment objectAnimationJudgmentPrefab;
    private List<Game_AnimationJudgment> listAnimationJudgment = new List<Game_AnimationJudgment>();
    public Sprite[] spriteJudgment;
    public Color[] colorJudgmentParticle = { Color.blue, Color.green, Color.yellow };

    public Text textSongAndArtistName;
    public Text textSongDetails;
    public Text textSongProgressTime;
    public Image imageSongProgressGauge;
    public GameObject objectGroupInterfaceAccuracy;
    public Image imageAccuracyGauge;
    public Image imageAccuracyNegativeGauge;
    public Text textAccuracy;
    public float floatAccuracyDisplay = 0f;
    public Text textNoteJudgeCount;
    public Image imageAccuracyTolerance;
    public float floatAccuracyGaugeWidth = 928f;

    public Animator animatorFullCombo;

    public Animator animatorResults;
    public Text textResultHeader;
    public Color colorResultHeaderPerfect = Color.yellow;
    public Color colorResultHeaderPass = Color.green;
    public Color colorResultHeaderFail = Color.red;
    public Text textResultAccuracy;
    public Text textResultBestCombo;
    public Text textResultJudgeBest;
    public Text textResultJudgeGreat;
    public Text textResultJudgeFine;
    public Text textResultJudgeMiss;
    public Text textResultOther;
    public Text textScoreDisabled;
    public Text textRecordAccuracy;
    public Animator animatorNewRecord;

    public Animator animatorPause;
    private bool boolIsPaused = false;
    private bool boolPauseMenuForceEnd = false;
    private float floatLastPaused = -999f;
    private bool boolPauseNotification = false;

    public Text textDebug;

    private ChartData chartData;
    private int chartTotalNotes = 0;
    private int chartJudgeDifficulty = 0;
    private float floatMusicPosition = 0f;
    private float floatMusicPositionEnd = 0f;
    private float floatMusicBeat = 0f;
    public float floatHighlightAlpha = 0.5f;
    private float movementHoriAlpha = 0f;
    private float movementVertAlpha = 0f;

    public Game_SongLoader mSongLoader;
    public AudioSource audioSourceMusic;
    public AudioSource audioSourceEffect;
    //public List<AudioClip> clipGameHitsound = new List<AudioClip>();
    public AudioClip clipGameEndFullCombo;
    public AudioClip clipGameEndPerfect;
    public AudioClip clipGameEndPass;
    public AudioClip clipGameEndFail;
    public AudioClip clipGameForceEnd;
    public AudioClip clipGameButtonPress;

    public float floatNoteScrollMultiplier = 0.05f;
    public float[] floatDistAccuracyCatchBest = { 0.12f, 0.11f, 0.1f, 0.09f, 0.81f };
    public float[] floatDistAccuracyCatchGreat = { 0.15f, 0.139f, 0.128f, 0.115f, 0.101f };
    public float[] floatDistAccuracyCatchFine = { 0.168f, 0.156f, 0.142f, 0.128f, 0.114f };
    public float[] floatDistAccuracyTapBest = { 0.12f, 0.11f, 0.1f, 0.09f, 0.81f };
    public float[] floatDistAccuracyTapGreat = { 0.15f, 0.139f, 0.128f, 0.115f, 0.101f };
    public float[] floatDistAccuracyTapFine = { 0.168f, 0.156f, 0.142f, 0.128f, 0.114f };
    public float[] floatDistAccuracyTapMiss = { 0.17f, 0.16f, 0.15f, 0.14f, 0.13f };

    private bool isForcedEnd = false;
    private bool isScoringDisabled = false;
    private float floatTimeEscapeHeld = 0f;
    private int currentAccuracy = 0;
    private int currentAccuracyNegative = 0;
    private int playerAccuracyBest = 0;
    private int playerAccuracyGreat = 0;
    private int playerAccuracyFine = 0;
    private int playerAccuracyMiss = 0;
    private int playerComboCurrent = 0;
    private int playerComboBest = 0;

    private bool lastHitNoteIsTap = false;
    private float lastHitNoteDistance = 0f;
    private int lastHitNoteType = 0;

    private const float NOTE_LERP_RATE_MULTIPLIER = 4f;

    public void ExitGameScene()
    {
        if (boolCustomSong)
        {
            LoadScene("SongMenuCustom");
        }
        else
        {
            LoadScene("SongMenuOfficial");
        }
    }
    public void LoadScene(string sceneName)
    {
        PlaySoundEffect(clipGameButtonPress);

        //SceneManager.LoadScene(sceneName);
        SceneTransition.LoadScene(sceneName);
    }

    private Game_Note SpawnNoteCatch()
    {
        foreach (Game_Note x in listNoteCatch)
        {
            if (!x.gameObject.activeSelf)
            {
                x.transform.position = new Vector3(0f, 100f, 0f);
                return x;
            }
        }

        Game_Note newNote = Instantiate(noteCatchPrefab);
        listNoteCatch.Add(newNote);
        return newNote;
    }
    private Game_Note SpawnNoteTap()
    {
        foreach (Game_Note x in listNoteTap)
        {
            if (!x.gameObject.activeSelf)
            {
                x.transform.position = new Vector3(0f, 100f, 0f);
                return x;
            }
        }

        Game_Note newNote = Instantiate(noteTapPrefab);
        listNoteTap.Add(newNote);
        return newNote;
    }

    private void DespawnNote(Game_Note note)
    {
        note.gameObject.SetActive(false);
    }

    private void JudgeNote(Game_Note note, GameObject catcher, bool isTapNote)
    {
        bool playSoundEffect = false;

        lastHitNoteIsTap = isTapNote;
        lastHitNoteType = note.type % 4;

        // Catch note
        if (!isTapNote)
        {
            lastHitNoteDistance = note.transform.position.x - catcher.transform.position.x;
            float dist = Mathf.Abs(lastHitNoteDistance);
            switch (chartData.chartGameType)
            {
                default:
                    int animJudgeID = 0;
                    //int animSortingLayerID = 0;
                    // BEST
                    if (dist < floatDistAccuracyCatchBest[chartJudgeDifficulty] || boolAutoplay)
                    {
                        playerAccuracyBest++;
                        playerComboCurrent++;
                        animJudgeID = 0;
                        //animSortingLayerID = playerComboCurrent;
                        playSoundEffect = true;
#if UNITY_EDITOR
                        Debug.Log("Catch note judgment - distance: " + dist + " (BEST)");
#endif
                    }
                    // GREAT
                    else if (dist < floatDistAccuracyCatchGreat[chartJudgeDifficulty])
                    {
                        playerAccuracyGreat++;
                        playerComboCurrent++;
                        animJudgeID = 1;
                        //animSortingLayerID = playerComboCurrent;
                        playSoundEffect = true;
#if UNITY_EDITOR
                        Debug.Log("Catch note judgment - distance: " + dist + " (GREAT)");
#endif
                    }
                    // FINE
                    else if (dist < floatDistAccuracyCatchFine[chartJudgeDifficulty])
                    {
                        playerAccuracyFine++;
                        playerComboCurrent++;
                        animJudgeID = 2;
                        //animSortingLayerID = playerComboCurrent;
                        playSoundEffect = true;
#if UNITY_EDITOR
                        Debug.Log("Catch note judgment - distance: " + dist + " (FINE)");
#endif
                    }
                    // MISS
                    else
                    {
                        //animSortingLayerID = playerComboCurrent + 1;
                        playerAccuracyMiss++;
                        playerComboCurrent = 0;
                        animJudgeID = 3;
#if UNITY_EDITOR
                        Debug.Log("Catch note judgment - distance: " + dist + " (MISS)");
#endif
                    }

                    // Show judgment animation
                    if (PlayerSetting.setting.enableDisplayNoteJudgment)
                    {
                        Game_AnimationJudgment anim = SpawnJudgeAnimation();
                        anim.gameObject.SetActive(true);
                        anim.transform.position = Vector3.right * note.position;
                        anim.gameObject.layer = 9 + note.type;
                        anim.spriteRendererJudgment.sprite = spriteJudgment[animJudgeID];
                        //anim.spriteRendererJudgment.sortingLayerID = animSortingLayerID;
                        anim.animatorJudgment.Play("anim");
                        if (animJudgeID < colorJudgmentParticle.Length)
                        {
                            ParticleSystem.MainModule animParticleModule = anim.particleSystemJudgment.main;
                            animParticleModule.startColor = colorJudgmentParticle[animJudgeID];
                            anim.particleSystemJudgment.gameObject.layer = anim.gameObject.layer;
                            anim.particleSystemJudgment.Play();
                        }
                    }
                    break;
            }
        }
        // Tap note
        else
        {
            lastHitNoteDistance = (floatMusicBeat - note.time) / chartData.songTempo * 60f;
            float dist = Mathf.Abs(lastHitNoteDistance);
            switch (chartData.chartGameType)
            {
                default:
                    int animJudgeID = 0;
                    //int animSortingLayerID = 0;
                    // BEST
                    if (dist < floatDistAccuracyTapBest[chartJudgeDifficulty] || boolAutoplay)
                    {
                        playerAccuracyBest++;
                        playerComboCurrent++;
                        animJudgeID = 0;
                        //animSortingLayerID = playerComboCurrent;
                        playSoundEffect = true;
#if UNITY_EDITOR
                        Debug.Log("Tap note judgment - distance: " + dist + " (BEST)");
#endif
                    }
                    // GREAT
                    else if (dist < floatDistAccuracyTapGreat[chartJudgeDifficulty])
                    {
                        playerAccuracyGreat++;
                        playerComboCurrent++;
                        animJudgeID = 1;
                        //animSortingLayerID = playerComboCurrent;
                        playSoundEffect = true;
#if UNITY_EDITOR
                        Debug.Log("Tap note judgment - distance: " + dist + " (GREAT)");
#endif
                    }
                    // FINE
                    else if (dist < floatDistAccuracyTapFine[chartJudgeDifficulty])
                    {
                        playerAccuracyFine++;
                        playerComboCurrent++;
                        animJudgeID = 2;
                        //animSortingLayerID = playerComboCurrent;
                        playSoundEffect = true;
#if UNITY_EDITOR
                        Debug.Log("Tap note judgment - distance: " + dist + " (FINE)");
#endif
                    }
                    // MISS
                    else
                    {
                        //animSortingLayerID = playerComboCurrent + 1;
                        playerAccuracyMiss++;
                        playerComboCurrent = 0;
                        animJudgeID = 3;
#if UNITY_EDITOR
                        Debug.Log("Tap note judgment - distance: " + dist + " (MISS)");
#endif
                    }

                    // Show judgment animation
                    if (PlayerSetting.setting.enableDisplayNoteJudgment)
                    {
                        Game_AnimationJudgment anim = SpawnJudgeAnimation();
                        anim.gameObject.SetActive(true);
                        anim.transform.position = Vector3.right * note.position;
                        anim.gameObject.layer = 9 + note.type;
                        anim.spriteRendererJudgment.sprite = spriteJudgment[animJudgeID];
                        //anim.spriteRendererJudgment.sortingLayerID = animSortingLayerID;
                        anim.spriteRendererJudgment.gameObject.layer = anim.gameObject.layer;
                        anim.animatorJudgment.Play("anim");
                        if (animJudgeID < colorJudgmentParticle.Length)
                        {
                            ParticleSystem.MainModule animParticleModule = anim.particleSystemJudgment.main;
                            animParticleModule.startColor = colorJudgmentParticle[animJudgeID];
                            anim.particleSystemJudgment.gameObject.layer = anim.gameObject.layer;
                            anim.particleSystemJudgment.Play();
                        }
                    }
                    break;
            }
        }

        // Check for best combo
        if (playerComboCurrent > playerComboBest)
        {
            playerComboBest = playerComboCurrent;
        }
        // Display combo
        if (PlayerSetting.setting.enableDisplayCombo)
        {
            textMeshComboCurrent.gameObject.SetActive(playerComboCurrent >= floatTextComboAppearComboMinimum);
            textMeshComboCurrent.text = playerComboCurrent.ToString();
            if (textMeshComboCurrent.gameObject.activeSelf)
            {
                floatTextComboScaleCurrent = floatTextComboScaleOnChange;
                textMeshComboCurrent.transform.localScale = Vector3.one * floatTextComboScaleCurrent;
            }
        }

        // Play sound effect
        //  FORMAT:  sound:x
        //  where x is a number of the sound effect
        if (playSoundEffect)
        {
            foreach (string x in note.other)
            {
                if (x.StartsWith("sound"))
                {
                    string[] y = x.Split(':');
                    int soundID = int.Parse(y[1]);
                    //if (clipGameHitsound.Count > soundID)
                    if (mSongLoader.listClipEffect.Count > soundID)
                    {
                        //PlaySoundEffect(clipGameHitsound[soundID]);
                        //PlaySoundEffect(mSongLoader.listClipEffect[soundID]);
                        mSongLoader.PlaySoundEffect(mSongLoader.listClipEffect[soundID]);
                    }
                    break;
                }
            }
        }

        // Animate full combo (combo = number of notes)
        if (playerAccuracyBest + playerAccuracyGreat + playerAccuracyFine == chartTotalNotes && animatorFullCombo != null)
        {
#if UNITY_EDITOR
            Debug.Log(playerAccuracyBest.ToString() + " + " + playerAccuracyGreat.ToString() + " + " + playerAccuracyFine.ToString() + " = " + chartTotalNotes.ToString());
#endif

            // Perfect accuracy
            if (playerAccuracyBest == chartTotalNotes)
            {
                animatorFullCombo.Play("perfect");
            }
            // Non-perfect accuracy
            else
            {
                animatorFullCombo.Play("normal");
            }
            PlaySoundEffect(clipGameEndFullCombo);
        }

        DespawnNote(note);
    }

    private Game_AnimationJudgment SpawnJudgeAnimation()
    {
        foreach (Game_AnimationJudgment x in listAnimationJudgment)
        {
            if (!x.gameObject.activeSelf)
            {
                return x;
            }
        }

        Game_AnimationJudgment newAnim = Instantiate(objectAnimationJudgmentPrefab);
        listAnimationJudgment.Add(newAnim);
        return newAnim;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (audioSourceEffect != null && clip != null)
        {
            audioSourceEffect.PlayOneShot(clip, PlayerSetting.setting.floatVolumeEffect);
        }
    }

    public void UnpauseGame()
    {
        StopCoroutine("_UnpauseGame");
        StartCoroutine("_UnpauseGame");
    }
    private IEnumerator _UnpauseGame()
    {
        PlaySoundEffect(clipGameButtonPress);
        animatorPause.Play("unpause");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        yield return new WaitForSeconds(1f);
        boolIsPaused = false;
        mSongLoader.audioSourceMusic.UnPause();
    }

    public void PauseMenuForceEnd()
    {
        boolPauseMenuForceEnd = true;
    }

    void Start()
    {
        // Cursor lock
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        // Vertical synchronization
        /*
        if (PlayerSetting.setting.enableVSync)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 999999;
        }
        */

        // Enable/Disable groups depending on game type
        objectGroupType[0].SetActive(true);
        objectGroupType[1].SetActive(intChartGameType >= 1);
        objectGroupType[2].SetActive(intChartGameType >= 2);
        objectGroupType[3].SetActive(intChartGameType >= 2);

        // Variable initialization
        if (boolCustomSong)
        {
            floatPreviousRecord = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy", 0f);
        }
        else
        {
            floatPreviousRecord = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy_official", 0f);
        }

        float textAlpha = textMeshRecordGhost.color.a;
        colorRecordGhostBetter.a = textAlpha;
        colorRecordGhostNeutral.a = textAlpha;
        colorRecordGhostWorse.a = textAlpha;

        // Object initialization
        textDebug.gameObject.SetActive(Debug.isDebugBuild || Application.isEditor);
        rendererPlaneBackground.gameObject.SetActive(false);
        objectAutoplayIndicator.SetActive(boolAutoplay);
        imageSongProgressGauge.fillAmount = 0f;
        if (PlayerSetting.setting.intAccuracyTolerance > 0)
        {
            imageAccuracyTolerance.transform.localPosition += Vector3.right * floatAccuracyGaugeWidth * 0.01f * PlayerSetting.setting.intAccuracyTolerance;
        }
        else
        {
            imageAccuracyTolerance.gameObject.SetActive(false);
        }
        foreach (GameObject x in objectCatcher)
        {
            x.transform.localPosition = Vector3.zero;
        }
        foreach (SpriteRenderer x in spriteRendererCatcherHighlight)
        {
            x.color = Color.clear;
        }
        textNoteJudgeCount.gameObject.SetActive(PlayerSetting.setting.enableDisplayNoteHitCounterSmall);
        textNoteJudgeCount.text = "B 0\nG 0\nF 0\nM 0";
        animatorResults.gameObject.SetActive(false);
        animatorNewRecord.gameObject.SetActive(false);
        textMeshComboCurrent.gameObject.SetActive(false);
        textMeshComboCurrent.text = "";
        textMeshRecordGhost.gameObject.SetActive(PlayerSetting.setting.enableDisplayRecordGhost && floatPreviousRecord > Mathf.Epsilon && !boolAutoplay);
        if (PlayerSetting.setting.enableDisplayRecordGhost && !PlayerSetting.setting.enableDisplayCombo)
        {
            textMeshRecordGhost.transform.position = Vector3.zero;
        }
        audioSourceMusic.volume = PlayerSetting.setting.floatVolumeMusic;
        //audioSourceEffect.volume = PlayerSetting.setting.floatVolumeEffect;

        // Read chart from the text file
        string input = "";
        // Chart is custom-made
        if (boolCustomSong)
        {
            string chartFileName = stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString();
            string path = Directory.GetCurrentDirectory() + "/Songs/" + stringSongFileName + "/" + chartFileName + ".txt";
            if (!File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.LogWarning("WARNING: The chart file does not exist! Path: " + path);
#endif
                isForcedEnd = true;
            }
            StreamReader reader = new StreamReader(path);
            input = reader.ReadToEnd();
            reader.Close();
#if UNITY_EDITOR
            Debug.Log(input);
#endif
        }
        // Chart is built-in
        else
        {
            string chartFileName = stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString();
            //string path = "Songs/" + chartFileName + ".txt";
            TextAsset info = Resources.Load(chartFileName) as TextAsset;
            input = info.text;
        }

        chartData = ScriptableObject.CreateInstance<ChartData>();
        JsonUtility.FromJsonOverwrite(input, chartData);

        //FileStream fileMusic = File.Open(Directory.GetCurrentDirectory() + "Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg", FileMode.Open);
        //audioSourceMusic.clip = (AudioClip)fileMusic as AudioClip;

        // Get background texture
        Texture textureBackground = null;
        if (boolCustomSong)
        {
            string path = "/Songs/" + stringSongFileName + "/background.jpg";
            if (!File.Exists(Directory.GetCurrentDirectory() + path))
            {
#if UNITY_EDITOR
                Debug.LogWarning("WARNING: The chart file does not exist! Path: " + path);
#endif
            }
            else
            {
                WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + path);
                textureBackground = www.texture;
            }
        }
        else
        {
            string path = stringSongFileName + "_background";
            textureBackground = Resources.Load(path) as Texture;
        }
        if (textureBackground != null)
        {
            rendererPlaneBackground.gameObject.SetActive(true);
            rendererPlaneBackground.material.mainTexture = textureBackground;
            rendererPlaneBackground.material.color = new Color(0.7f, 0.7f, 0.7f);
        }

        string gameTypeAbbr = "";
        //switch (chartData.chartGameType)
        switch (intChartGameType)
        {
            case 0: gameTypeAbbr = "LN"; break;
            case 1: gameTypeAbbr = "DB"; break;
            case 2: gameTypeAbbr = "QD"; break;
            case 3: gameTypeAbbr = "ND"; break;
            /*
            case 3: gameTypeAbbr = "PWF"; break;
            case 4: gameTypeAbbr = "SP"; break;
            case 5: gameTypeAbbr = "ULT"; break;
            case 6: gameTypeAbbr = "BC"; break;
            case 7: gameTypeAbbr = "ND"; break;
            */
        }

        if (PlayerSetting.setting.enableInterfaceSongDetails)
        {
            textSongAndArtistName.gameObject.SetActive(true);
            textSongDetails.gameObject.SetActive(true);
            textSongProgressTime.gameObject.SetActive(true);
            imageSongProgressGauge.gameObject.SetActive(true);

            textSongAndArtistName.text = chartData.songArtist + " - " + chartData.songName;
            textSongDetails.text = Translator.GetStringTranslation("GAME_CHARTDEV", "Mapchart by") + " " + chartData.chartDeveloper + " [" + gameTypeAbbr + " #" + (intChartGameChart + 1).ToString() + "] (" + Translator.GetStringTranslation("GAME_CHARTLEVEL", "Level") + " " + chartData.chartLevel + ")";
            textSongProgressTime.text = "--:-- / --:--";
            imageSongProgressGauge.fillAmount = 0f;
        }
        else
        {
            textSongAndArtistName.gameObject.SetActive(false);
            textSongDetails.gameObject.SetActive(false);
            textSongProgressTime.gameObject.SetActive(false);
            imageSongProgressGauge.gameObject.SetActive(false);
        }
        objectGroupInterfaceAccuracy.SetActive(PlayerSetting.setting.enableInterfaceAccuracy);
        imageAccuracyGauge.fillAmount = 0f;
        imageAccuracyNegativeGauge.fillAmount = 0f;
        textAccuracy.text = "0.00%";
        chartTotalNotes = chartData.listNoteCatchInfo.Count + chartData.listNoteTapInfo.Count;
        for (int i = 0; i < chartData.listNoteCatchInfo.Count; i++)
        {
            string[] noteInfo = chartData.listNoteCatchInfo[i].Split('|');
            float longNoteLength = float.Parse(noteInfo[4]);
            if (longNoteLength > 0.01f - Mathf.Epsilon)
            {
                chartTotalNotes++;
            }
        }
        /*
        for (int i = 0; i < chartData.listNoteTapInfo.Count; i++)
        {
            string[] noteInfo = chartData.listNoteTapInfo[i].Split('|');
            float longNoteLength = float.Parse(noteInfo[4]);
            if (longNoteLength > 0.01f)
            {
                chartTotalNotes++;
            }
        }
        */
        chartJudgeDifficulty = chartData.chartJudge;
        if (chartJudgeDifficulty >= floatDistAccuracyCatchBest.Length) chartJudgeDifficulty = floatDistAccuracyCatchBest.Length - 1;

        StartCoroutine("GameLoop");
    }

    void Update()
    {
        // Skip while game is paused
        if (boolIsPaused)
        {
            return;
        }

        // Camera rotation
        floatCameraRotation += floatCameraRotationChangeRate * Time.deltaTime * chartData.songTempo / 60f;
        for (int i = 0; i < cameraGame.Length; i++)
        {
            if (boolCameraRotationNoLerp)
            {
                cameraGame[i].transform.rotation = Quaternion.Lerp(cameraGame[i].transform.rotation, Quaternion.Euler(0f, 0f, (i * 90f) + floatCameraRotation), Time.deltaTime * floatCameraRotationLerpRate);
            }
            else
            {
                cameraGame[i].transform.rotation = Quaternion.Euler(0f, 0f, (i * 90f) + floatCameraRotation);
            }
        }
        cameraMain.transform.rotation = cameraGame[0].transform.rotation;

        // Debug info
        if (textDebug.gameObject.activeInHierarchy)
        {
            textDebug.text =
                "Accuracy Points: " + currentAccuracy.ToString() + " / " + (chartTotalNotes * 4).ToString() + "\n" +
                "N Accuracy Points: " + currentAccuracyNegative.ToString() + " / " + (chartTotalNotes * 4).ToString() + "\n" +
                "Combo (Current/Best): " + playerComboCurrent.ToString() + " / " + playerComboBest.ToString() + "\n" +
                "   (of " + chartTotalNotes.ToString() + " chart total)\n" +
                "\n" +
                "Last Hit Note: ";

            switch(lastHitNoteType)
            {
                case 0: textDebug.text += "BLUE  "; break;
                case 1: textDebug.text += "RED   "; break;
                case 2: textDebug.text += "GREEN "; break;
                case 3: textDebug.text += "YELLOW"; break;
            }
            textDebug.text += " (" + lastHitNoteType.ToString() + ")\n";

            if (!lastHitNoteIsTap)
            {
                if (lastHitNoteDistance < -Mathf.Epsilon)
                {
                    textDebug.text += " <<< " + Mathf.Abs(lastHitNoteDistance).ToString("f3") + "\n";
                }
                else if (lastHitNoteDistance > Mathf.Epsilon)
                {
                    textDebug.text += "     " + Mathf.Abs(lastHitNoteDistance).ToString("f3") + " >>>\n";
                }
                else
                {
                    textDebug.text += "     " + Mathf.Abs(lastHitNoteDistance).ToString("f3") + "\n";
                }
            }
            else
            {
                if (lastHitNoteDistance < -Mathf.Epsilon)
                {
                    textDebug.text += "     " + Mathf.Abs(lastHitNoteDistance).ToString("f3") + " (TAP EARLY)\n";
                }
                else if (lastHitNoteDistance > Mathf.Epsilon)
                {
                    textDebug.text += "     " + Mathf.Abs(lastHitNoteDistance).ToString("f3") + " (TAP LATE)\n";
                }
                else
                {
                    textDebug.text += "     " + Mathf.Abs(lastHitNoteDistance).ToString("f3") + " (TAP =)\n";
                }
            }

            textDebug.text +=
                "\n" +
                "Escape held: " + floatTimeEscapeHeld.ToString("f2");
        }
    }
	
	private IEnumerator GameLoop ()
    {
        //yield return new WaitForSeconds(0.1f);

        /*
        // Load audio music file
        if (boolCustomSong)
        {
            string file = "/Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg";
            WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + file);
            while (!www.isDone)
            {
                yield return null;
            }
#if UNITY_EDITOR
            Debug.Log(www.url);
            if (www.error != "")
            {
                Debug.Log("Error message from reading audio file: " + www.error);
            }
#endif
            audioSourceMusic.clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
            while (audioSourceMusic.clip == null)
            {
                yield return null;
            }
        }
        else
        {
            string path = "Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg";
            audioSourceMusic.clip = Resources.Load(path) as AudioClip;
        }

        yield return null;

        // Load hitsound files
        for (int i = 0; true; i++)
        {
            if (boolCustomSong)
            {
                string file = "/Songs/" + stringSongFileName + "/sound" + i.ToString() + ".ogg";
                WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + file);
                while (!www.isDone)
                {
                    yield return null;
                }
                if (www.error != "")
                {
#if UNITY_EDITOR
                    Debug.Log("Error message from reading audio file: " + www.error);
                    break;
#endif
                }

                AudioClip newSound = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
                while (!www.isDone && newSound == null)
                {
                    yield return null;
                }
                if (newSound != null)
                {
                    clipGameHitsound.Add(newSound);
                }
                else
                {
                    break;
                }
            }
            else
            {
                string path = "Songs/" + stringSongFileName + "/sound" + i.ToString() + ".ogg";
                AudioClip newSound = Resources.Load(path) as AudioClip;
                if (newSound != null)
                {
                    clipGameHitsound.Add(newSound);
                }
                else
                {
                    break;
                }
            }
            yield return null;
        }
        */

        yield return new WaitForSeconds(1.0f);

        //do
        //{
        //    yield return null;
        //} while (mSongLoader.clipSong == null);

        // Play music and begin the game
        //audioSourceMusic.clip = clipSong;
        //audioSourceMusic.clip = mSongLoader.clipSong;
        //audioSourceMusic.Play();
        mSongLoader.audioSourceMusic.Play();

        floatMusicPositionEnd = chartData.songLength / chartData.songTempo * 60f;
        floatMusicPositionEnd -= (chartData.chartOffset + PlayerSetting.setting.intGameOffset) * 0.001f;

        yield return null;
        yield return new WaitUntil(() => mSongLoader.audioSourceMusic.isPlaying);

        // Actual game loop
        while (floatMusicPosition < floatMusicPositionEnd && (mSongLoader.audioSourceMusic.isPlaying || boolIsPaused))
        //while (floatMusicPosition < floatMusicPositionEnd && audioSourceMusic.isPlaying)
        {
            // Skip while game is paused
            if (boolIsPaused)
            {
                yield return null;
                continue;
            }

            // Time update
            floatMusicPosition = mSongLoader.audioSourceMusic.time - ((chartData.chartOffset + PlayerSetting.setting.intGameOffset) * 0.001f);
            //floatMusicPosition = audioSourceMusic.time - ((chartData.chartOffset + PlayerSetting.setting.intGameOffset) * 0.001f);
            floatMusicBeat = floatMusicPosition * chartData.songTempo / 60f;

            // Highlight flash on beat
            float floatHighlightAlphaCurrent = 0f;
            if (PlayerSetting.setting.enableNoteAndCatcherHighlightBeatPulse)
            {
                floatHighlightAlphaCurrent = (1f - (floatMusicBeat - Mathf.Floor(floatMusicBeat))) * floatHighlightAlpha;
            }
            Color colorBeatFlash = Color.white;
            colorBeatFlash.a = floatHighlightAlphaCurrent;

            // Crosshair position
            Vector3 mouseCursorPos = objectMouseCrosshair.transform.position;
            // Normal play
            if (!boolAutoplay)
            {
                // Mouse movement
                if (PlayerSetting.setting.boolUseMouseRawInput)
                {
                    movementHoriAlpha = Input.GetAxisRaw("MouseX");
                    movementVertAlpha = Input.GetAxisRaw("MouseY");
                }
                else
                {
                    movementHoriAlpha = Input.GetAxis("MouseX");
                    movementVertAlpha = Input.GetAxis("MouseY");
                }

                mouseCursorPos.x = Mathf.Clamp(mouseCursorPos.x + movementHoriAlpha * PlayerSetting.setting.floatMouseSensitivity, -1f, 1f);
                mouseCursorPos.y = Mathf.Clamp(mouseCursorPos.y + movementVertAlpha * PlayerSetting.setting.floatMouseSensitivity, -1f, 1f);
                objectMouseCrosshair.transform.position = mouseCursorPos;
            }
            // Automatic play (always perfect)
            else
            {
                Game_Note nextNoteCatcherHori = null;
                Game_Note nextNoteCatcherVert = null;
                foreach (Game_Note x in listNoteCatch)
                {
                    if (x.gameObject.activeSelf)
                    {
                        switch (x.type)
                        {
                            case 0:
                            case 1:
                                if (nextNoteCatcherHori == null || x.transform.position.y < nextNoteCatcherHori.transform.position.y)
                                {
                                    nextNoteCatcherHori = x;
                                }
                                break;
                            case 2:
                            case 3:
                                if (nextNoteCatcherVert == null || x.transform.position.y < nextNoteCatcherVert.transform.position.y)
                                {
                                    nextNoteCatcherVert = x;
                                }
                                break;
                        }
                    }
                }

                float posLerpRate = 4f;
                if (nextNoteCatcherHori != null)
                {
                    switch(nextNoteCatcherHori.type)
                    {
                        case 0:
                            mouseCursorPos.x = Mathf.Lerp(mouseCursorPos.x, nextNoteCatcherHori.position, Time.deltaTime * posLerpRate / (nextNoteCatcherHori.time - floatMusicBeat));
                            break;
                        case 1:
                            mouseCursorPos.x = Mathf.Lerp(mouseCursorPos.x, -nextNoteCatcherHori.position, Time.deltaTime * posLerpRate / (nextNoteCatcherHori.time - floatMusicBeat));
                            break;
                    }
                }
                if (nextNoteCatcherVert != null)
                {
                    switch (nextNoteCatcherVert.type)
                    {
                        case 2:
                            mouseCursorPos.y = Mathf.Lerp(mouseCursorPos.y, -nextNoteCatcherVert.position, Time.deltaTime * posLerpRate / (nextNoteCatcherVert.time - floatMusicBeat));
                            break;
                        case 3:
                            mouseCursorPos.y = Mathf.Lerp(mouseCursorPos.y, nextNoteCatcherVert.position, Time.deltaTime * posLerpRate / (nextNoteCatcherVert.time - floatMusicBeat));
                            break;
                    }
                }
            }
            objectMouseCrosshair.transform.position = mouseCursorPos;

            // Catcher position
            objectCatcher[0].transform.position = Vector3.right * objectMouseCrosshair.transform.position.x;
            if (objectCatcher[1].activeInHierarchy)
            {
                objectCatcher[1].transform.position = Vector3.left * objectMouseCrosshair.transform.position.x;
            }
            if (objectCatcher[2].activeInHierarchy)
            {
                objectCatcher[2].transform.position = Vector3.left * objectMouseCrosshair.transform.position.y;
            }
            if (objectCatcher[3].activeInHierarchy)
            {
                objectCatcher[3].transform.position = Vector3.right * objectMouseCrosshair.transform.position.y;
            }
            // Catcher flash
            foreach(SpriteRenderer x in spriteRendererCatcherHighlight)
            {
                x.color = colorBeatFlash;
            }

            // Text and gauge update
            // Top - Song details and progress
            if (textSongProgressTime.gameObject.activeSelf)
            {
                textSongProgressTime.text =
                    Mathf.Floor(mSongLoader.audioSourceMusic.time / 60f).ToString() + ":" + Mathf.Floor(mSongLoader.audioSourceMusic.time % 60f).ToString("00") + " / " +
                    Mathf.Floor(chartData.songLength / chartData.songTempo).ToString() + ":" + Mathf.Floor((chartData.songLength / chartData.songTempo * 60f) % 60f).ToString("00");
            }
            if (imageSongProgressGauge.gameObject.activeSelf)
            {
                imageSongProgressGauge.fillAmount = floatMusicPosition / floatMusicPositionEnd;
            }
            // Bottom - Accuracy
            currentAccuracy = (playerAccuracyBest * 4) + (playerAccuracyGreat * 2) + playerAccuracyFine;
            currentAccuracyNegative = (playerAccuracyGreat * 2) + (playerAccuracyFine * 3) + (playerAccuracyMiss * 4);
            if (objectGroupInterfaceAccuracy.activeSelf)
            {
                floatAccuracyDisplay = Mathf.Lerp(floatAccuracyDisplay, 1f * currentAccuracy / (chartTotalNotes * 4), Time.deltaTime * 8f);
                imageAccuracyGauge.fillAmount = floatAccuracyDisplay;
                imageAccuracyNegativeGauge.fillAmount = 1f * currentAccuracyNegative / (chartTotalNotes * 4);
                textAccuracy.text = (floatAccuracyDisplay * 100f).ToString("f2") + "%";
            }
            if (PlayerSetting.setting.enableDisplayNoteHitCounterSmall)
            {
                textNoteJudgeCount.text =
                    "B " + playerAccuracyBest.ToString() + "\n" +
                    "G " + playerAccuracyGreat.ToString() + "\n" +
                    "F " + playerAccuracyFine.ToString() + "\n" +
                    "M " + playerAccuracyMiss.ToString();
            }
            // Center - Combo and judgment
            if (textMeshComboCurrent.gameObject.activeSelf)
            {
                floatTextComboScaleCurrent = Mathf.Clamp(floatTextComboScaleCurrent - Time.deltaTime * floatTextComboScaleChangeRate, floatTextComboScaleMinimum, floatTextComboScaleOnChange);
                textMeshComboCurrent.transform.localScale = Vector3.one * floatTextComboScaleCurrent;
            }
            if (textMeshRecordGhost.gameObject.activeSelf)
            {
                float currentAccuracyPercentage = 1f * currentAccuracy / (chartTotalNotes * 4);
                float ghostAccuracy = 1f * floatPreviousRecord * (playerAccuracyBest + playerAccuracyGreat + playerAccuracyFine + playerAccuracyMiss) / chartTotalNotes;

                if (currentAccuracyPercentage > ghostAccuracy + Mathf.Epsilon)
                {
                    textMeshRecordGhost.color = colorRecordGhostBetter;
                    textMeshRecordGhost.text = "+" + (100f * (currentAccuracyPercentage - ghostAccuracy)).ToString("f2") + "%";
                }
                else if (currentAccuracyPercentage < ghostAccuracy - Mathf.Epsilon)
                {
                    textMeshRecordGhost.color = colorRecordGhostWorse;
                    textMeshRecordGhost.text = (100f * (currentAccuracyPercentage - ghostAccuracy)).ToString("f2") + "%";
                }
                else
                {
                    textMeshRecordGhost.color = colorRecordGhostNeutral;
                    textMeshRecordGhost.text = "+" + (100f * (currentAccuracyPercentage - ghostAccuracy)).ToString("f2") + "%";
                }
            }

            // Note spawning
            // Catch note
            for (int j = 0; j < chartData.listNoteCatchInfo.Count; j++)
            {
                string[] noteInfo = chartData.listNoteCatchInfo[j].Split('|');
                float time = float.Parse(noteInfo[2]);
                float speed = float.Parse(noteInfo[5]);
                if (speed < 0.01f)
                {
                    speed = 1f;
                }
                // Spawn note if position of note < current beat pos / FOV (scroll speed)
                if (time < floatMusicBeat + (floatNoteDistanceSpawn / floatNoteScrollMultiplier / PlayerSetting.setting.intScrollSpeed / speed))
                {
                    Game_Note note = SpawnNoteCatch();
                    note.type = int.Parse(noteInfo[0]);
                    note.size = int.Parse(noteInfo[1]);
                    note.time = time;
                    note.position = Mathf.Clamp(float.Parse(noteInfo[3]), -1f, 1f);
                    note.length = 0f;
                    note.speed = speed;
                    note.other = new List<string>();
                    if (noteInfo.Length > 6)
                    {
                        for (int i = 6; i < noteInfo.Length; i++)
                        {
                            note.other.Add(noteInfo[i]);
                        }
                    }
                    note.gameObject.layer = 9 + note.type;
                    note.spriteRendererNote.color = noteColor[note.type];
                    note.gameObject.SetActive(true);
                    note.spriteRendererLength.gameObject.SetActive(false);

                    // If note has length, create a second note with a line below it
                    float longNoteLength = float.Parse(noteInfo[4]);
                    if (longNoteLength > 0.01f)
                    {
                        note = SpawnNoteCatch();
                        note.type = int.Parse(noteInfo[0]);
                        note.size = int.Parse(noteInfo[1]);
                        note.time = time + longNoteLength;
                        note.position = Mathf.Clamp(float.Parse(noteInfo[3]), -1f, 1f);
                        note.length = longNoteLength;
                        note.speed = speed;
                        note.other = new List<string>();
                        note.gameObject.layer = 9 + note.type;
                        note.spriteRendererNote.color = noteColor[note.type];
                        note.gameObject.SetActive(true);

                        note.spriteRendererLength.gameObject.SetActive(true);
                        note.spriteRendererLength.gameObject.layer = 9 + note.type;
                        note.spriteRendererLength.transform.localPosition = Vector3.down * longNoteLength * floatNoteScrollMultiplier * speed * PlayerSetting.setting.intScrollSpeed / 2f;
                        note.spriteRendererLength.transform.localScale = new Vector3(
                            note.spriteRendererLength.transform.localScale.x,
                            longNoteLength * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * speed,
                            1f);
                        note.spriteRendererLength.color = noteColor[note.type];
                    }

                    Vector3 notePos = new Vector3(note.position, (note.time - floatMusicBeat) * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * note.speed);
                    note.transform.position = notePos;

                    chartData.listNoteCatchInfo.RemoveAt(j);
                    break;
                }
            }
            // Tap note
            for (int j = 0; j < chartData.listNoteTapInfo.Count; j++)
            {
                string[] noteInfo = chartData.listNoteTapInfo[j].Split('|');
                float time = float.Parse(noteInfo[2]);
                float speed = float.Parse(noteInfo[5]);
                if (speed < 0.01f)
                {
                    speed = 1f;
                }
                if (time < floatMusicBeat + (floatNoteDistanceSpawn / floatNoteScrollMultiplier / PlayerSetting.setting.intScrollSpeed / speed))
                {
                    Game_Note note = SpawnNoteTap();
                    note.type = int.Parse(noteInfo[0]);
                    note.size = int.Parse(noteInfo[1]);
                    note.time = time;
                    note.position = Mathf.Clamp(float.Parse(noteInfo[3]), -1f, 1f);
                    note.length = 0f;
                    note.speed = speed;
                    note.other = new List<string>();
                    if (noteInfo.Length > 6)
                    {
                        for (int i = 6; i < noteInfo.Length; i++)
                        {
                            note.other.Add(noteInfo[i]);
                        }
                    }
                    note.gameObject.layer = 13 + note.type;
                    note.spriteRendererNote.color = noteColor[note.type];
                    note.gameObject.SetActive(true);
                    note.spriteRendererLength.gameObject.SetActive(false);

                    float longNoteLength = float.Parse(noteInfo[4]);
                    if (longNoteLength > 0.01f)
                    {
                        note = SpawnNoteTap();
                        note.type = int.Parse(noteInfo[0]);
                        note.size = int.Parse(noteInfo[1]);
                        note.time = time;
                        note.position = Mathf.Clamp(float.Parse(noteInfo[3]), -1f, 1f);
                        note.length = longNoteLength;
                        note.speed = speed;
                        note.other = new List<string>();
                        note.gameObject.layer = 13 + note.type;
                        note.spriteRendererNote.color = noteColor[note.type];
                        note.gameObject.SetActive(true);

                        note.spriteRendererLength.gameObject.SetActive(true);
                        note.spriteRendererLength.transform.localPosition = Vector3.down * longNoteLength * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * speed / 2f;
                        note.spriteRendererLength.transform.localScale = new Vector3(
                            note.spriteRendererLength.transform.localScale.x,
                            longNoteLength * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * speed,
                            1f);
                        note.spriteRendererLength.color = noteColor[note.type];
                    }

                    Vector3 notePos = new Vector3(note.position, (note.time - floatMusicBeat) * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * note.speed);
                    note.transform.position = notePos;

                    chartData.listNoteTapInfo.RemoveAt(j);
                }
            }

            // Note positioning
            // Catch note
            foreach (Game_Note x in listNoteCatch)
            {
                if (x.gameObject.activeSelf)
                {
                    Vector3 notePos = new Vector3(x.position, (x.time - floatMusicBeat) * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * x.speed);
                    if (Vector3.Distance(x.transform.position, notePos) < chartData.songTempo / 24f)
                    {
                        x.transform.position = Vector3.Lerp(x.transform.position, notePos, Time.deltaTime * NOTE_LERP_RATE_MULTIPLIER * PlayerSetting.setting.intScrollSpeed * chartData.songTempo / 600f);
                    }
                    else
                    {
                        x.transform.position = notePos;
                    }

                    // Note flash
                    x.spriteRendererNoteHighlight.color = colorBeatFlash;
                    if (x.length > 0.01f)
                    {
                        x.spriteRendererLengthHighlight.color = colorBeatFlash;
                    }

                    // Note judgment
                    // Normal note or long note end - Go below pos 0 vertically
                    // Long note length - Sway too far from the note's center enough to get a "Miss"
                    if (floatMusicBeat >= x.time || 
                        (x.length > 0.01f &&
                        floatMusicBeat >= x.time - x.length &&
                        Mathf.Abs(x.transform.position.x - objectCatcher[x.type].transform.position.x) > floatDistAccuracyCatchFine[chartJudgeDifficulty])
                        )
                    {
                        JudgeNote(x, objectCatcher[x.type], false);
                    }
                }
            }
            // Tap note
            foreach (Game_Note x in listNoteTap)
            {
                if (x.gameObject.activeSelf)
                {
                    Vector3 notePos = new Vector3(x.position, (x.time - floatMusicBeat) * floatNoteScrollMultiplier * PlayerSetting.setting.intScrollSpeed * x.speed);
                    if (Vector3.Distance(x.transform.position, notePos) < chartData.songTempo / 240f)
                    {
                        x.transform.position = Vector3.Lerp(x.transform.position, notePos, Time.deltaTime * NOTE_LERP_RATE_MULTIPLIER * PlayerSetting.setting.intScrollSpeed * chartData.songTempo / 600f);
                    }
                    else
                    {
                        x.transform.position = notePos;
                    }

                    float dist = Mathf.Abs(floatMusicBeat - x.time) / chartData.songTempo * 60f;

                    // Note flash
                    x.spriteRendererNoteHighlight.color = colorBeatFlash;
                    if (x.length > 0.01f)
                    {
                        x.spriteRendererLengthHighlight.color = colorBeatFlash;
                    }

                    // Note judgment
                    // Judge if below "fine" for "miss"
                    if (x.transform.position.y < 0f && dist > floatDistAccuracyTapFine[chartJudgeDifficulty])
                    {
                        JudgeNote(x, objectCatcher[x.type], true);
                    }
                    // Autoplay
                    if (boolAutoplay && floatMusicBeat >= x.time - floatDistAccuracyTapBest[chartJudgeDifficulty])
                    {
                        JudgeNote(x, objectCatcher[x.type], true);
                    }
                }
            }

            // Tap note input (and which is not the Escape key being used)
            if (Input.anyKeyDown && !Input.GetKeyDown(KeyCode.Escape) && !boolAutoplay)
            {
                // Get the lowest positioned note
                Game_Note lowestNote = null;
                foreach (Game_Note x in listNoteTap)
                {
                    if (x.gameObject.activeSelf)
                    {
                        if (lowestNote == null || x.transform.position.y < lowestNote.transform.position.y)
                        {
                            lowestNote = x;
                        }
                    }
                }

                // Note judgment
                if (lowestNote != null)
                {
                    float dist = Mathf.Abs(floatMusicBeat - lowestNote.time) / chartData.songTempo * 60f;
                    if (dist < floatDistAccuracyTapMiss[chartJudgeDifficulty])
                    {
                        JudgeNote(lowestNote, objectCatcher[lowestNote.type % 4], true);
                    }
                }
            }

            // Hold [Escape] to end the game if it's in autoplay
            if (boolAutoplay && Input.GetKey(KeyCode.Escape))
            {
                floatTimeEscapeHeld += Time.deltaTime;
            }
            else
            {
                floatTimeEscapeHeld = 0f;
            }
            if (floatTimeEscapeHeld > 1f)
            {
                isForcedEnd = true;
            }

            // Press [Escape] to pause the game in a normal game
            if (!boolAutoplay && Input.GetKeyDown(KeyCode.Escape))
            {
                // Check if the last pause is at least 3 seconds before
                if (floatMusicPosition > floatLastPaused + 3f)
                {
                    boolIsPaused = true;
                    floatLastPaused = floatMusicPosition;
                    mSongLoader.audioSourceMusic.Pause();
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Locked;
                    animatorPause.Play("pause");
                    PlaySoundEffect(clipGameButtonPress);
                    yield return null;
                    Cursor.lockState = CursorLockMode.Confined;
                }
                // Display notification if the pause is too early
                else if (!boolPauseNotification)
                {
                    boolPauseNotification = true;
                    Notification.Display(Translator.GetStringTranslation("GAME_PAUSEFAILNOTIFICATION", "Wait at least three seconds from your last pause to pause again."), Color.white);
                }
            }

            // Force end condition: Current negative accuracy is below tolerance
            if (1f * currentAccuracyNegative / (4f * chartTotalNotes) >
                1f - (0.01f * PlayerSetting.setting.intAccuracyTolerance))
            {
                isForcedEnd = true;
            }

            // Force end consequences: Miss all remaining notes
            if (isForcedEnd || boolPauseMenuForceEnd)
            {
                isForcedEnd = false;
                foreach (string s in chartData.listNoteCatchInfo)
                {
                    string[] noteInfo = s.Split('|');
                    playerAccuracyMiss++;
                    float longNoteLength = float.Parse(noteInfo[4]);
                    if (longNoteLength > 0.01f)
                    {
                        playerAccuracyMiss++;
                    }
                    isForcedEnd = true;
                }
                foreach (string s in chartData.listNoteTapInfo)
                {
                    playerAccuracyMiss++;
                    isForcedEnd = true;
                }
                foreach (Game_Note x in listNoteCatch)
                {
                    if (x.gameObject.activeSelf)
                    {
                        isForcedEnd = true;
                        playerAccuracyMiss++;
                        DespawnNote(x);
                    }
                }
                foreach (Game_Note x in listNoteTap)
                {
                    if (x.gameObject.activeSelf)
                    {
                        isForcedEnd = true;
                        playerAccuracyMiss++;
                        DespawnNote(x);
                    }
                }
                audioSourceMusic.Stop();
                break;
            }

            yield return null;
        }
        // End game loop

        imageSongProgressGauge.fillAmount = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        yield return null;

        float finalAccuracy = 1f * ((playerAccuracyBest * 4) + (playerAccuracyGreat * 2) + playerAccuracyFine) / (chartTotalNotes * 4);
#if UNITY_EDITOR
        Debug.Log("Song finished. Accuracy: " + (finalAccuracy * 100f).ToString("f2") + "%");
#endif
        // Revert settings (cursor, v-sync, etc.)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        /*
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        */
        objectAutoplayIndicator.SetActive(false);
        textMeshComboCurrent.gameObject.SetActive(false);
        yield return null;

        // If the game was being autoplayed, skip result screen.
        if (boolAutoplay)
        {
            ExitGameScene();
            yield break;
        }

        // Check score validity
        isScoringDisabled =
            // No mods enabled
            PlayerSetting.setting.modChartBerserk ||
            PlayerSetting.setting.modChartCluster ||
            PlayerSetting.setting.modChartFlip ||
            PlayerSetting.setting.modChartHell ||
            PlayerSetting.setting.modChartMirror ||
            PlayerSetting.setting.modChartRandom ||
            PlayerSetting.setting.modDisableScore ||
            PlayerSetting.setting.modScreenFlip ||
            PlayerSetting.setting.modScreenMirror ||
            // Actual gameplay length is over a minute (from first to last note)
            chartData.gameplayLength < 60f ||
            // 25 or more notes present in chart
            chartTotalNotes < 25;

        // Add and record score
        textScoreDisabled.gameObject.SetActive(isScoringDisabled);
        int finalScore = 0;
        float oldRecordAccuracy = 0f;
        //int initialPlayerLevel = PlayerSetting.setting.GetPlayerLevel();
        if (!isScoringDisabled)
        {
            float gameModeScoreMultiplier = 1f;
            if (boolCustomSong)
            {
                // Accuracy record
                oldRecordAccuracy = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy", 0f);
                if (finalAccuracy > oldRecordAccuracy)
                {
                    PlayerPrefs.SetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy", finalAccuracy);
                }

                // Mapchart play count increase
                int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", 0);
                songPlayCount++;
                PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", songPlayCount);
            }
            else
            {
                // Score based on accuracy, best combo, chart level, and game mode.
                finalScore = Mathf.FloorToInt(
                    (1f * playerComboBest / chartTotalNotes) * finalAccuracy *  // Base accuracy
                    Mathf.Pow(4 + chartData.chartLevel, 2f) *                   // Chart level
                    10 * gameModeScoreMultiplier *                              // Game mode
                    chartData.gameplayLength / 60f                              // Chart gameplay length
                    );
                // Additional score gained by achieving perfect accuracy or full combo respectively.
                int additionalScore = 0;
                if (playerAccuracyBest == chartTotalNotes)
                {
                    additionalScore += finalScore / 4;
                }
                else if (playerAccuracyBest + playerAccuracyGreat + playerAccuracyFine == chartTotalNotes)
                {
                    additionalScore += finalScore / 10;
                }
                finalScore += additionalScore;
                PlayerSetting.setting.ScoreAdd(finalScore);
                
                // Accuracy record
                oldRecordAccuracy = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy_official", 0f);
                if (finalAccuracy > oldRecordAccuracy)
                {
                    PlayerPrefs.SetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy_official", finalAccuracy);
                }

                // Mapchart play count increase
                int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount_official", 0);
                songPlayCount++;
                PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount_official", songPlayCount);
            }
            switch (chartData.chartGameType)
            {
                case 0: gameModeScoreMultiplier = 1.0f; break;
                case 1: gameModeScoreMultiplier = 1.4f; break;
                case 2: gameModeScoreMultiplier = 1.8f; break;
                case 3: gameModeScoreMultiplier = 0.0f; break;
                case 4: gameModeScoreMultiplier = 2.2f; break;
                case 5: gameModeScoreMultiplier = 3.0f; break;
                case 6: gameModeScoreMultiplier = 5.0f; break;
                case 7: gameModeScoreMultiplier = 0.0f; break;
            }

            // Display old record
            textRecordAccuracy.gameObject.SetActive(true);
            textRecordAccuracy.text = (oldRecordAccuracy * 100f).ToString("f2") + "%";

            PlayerSetting.setting.Save();
        }
        else
        {
            textRecordAccuracy.gameObject.SetActive(false);
        }
        yield return null;

        // Update texts
        if (playerAccuracyFine == 0 && playerAccuracyGreat == 0 && playerAccuracyMiss == 0)
        {
            textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYPERFECT", "PERFECT RHYTHM");
            textResultHeader.color = colorResultHeaderPerfect;
            PlaySoundEffect(clipGameEndPerfect);
        }
        else if (finalAccuracy >= 0.01f * PlayerSetting.setting.intAccuracyTolerance)
        {
            textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYCLEAR", "COMPLETE RHYTHM");
            textResultHeader.color = colorResultHeaderPass;
            PlaySoundEffect(clipGameEndPass);
        }
        else
        {
            textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYFAIL", "UNDESIRED RHYTHM");
            textResultHeader.color = colorResultHeaderFail;

            if (isForcedEnd)
            {
                PlaySoundEffect(clipGameForceEnd);
            }
            else
            {
                PlaySoundEffect(clipGameEndFail);
            }
        }
        if (isScoringDisabled)
        {
            textResultOther.text = stringModList;
        }

        // Show result screen
        animatorResults.gameObject.SetActive(true);
        animatorResults.Play("clip");

        // Click a mouse button or hold space to speed up animation
        StartCoroutine(AnimatorResultsSpeedUp());

        for (float f = 0; f < 0.5f; f += Time.deltaTime * animatorResults.speed) yield return null;
        StartCoroutine(TextFloatGradualIncrease(textResultAccuracy, finalAccuracy * 100f, 0.8f));
        StartCoroutine(TextIntGradualIncrease(textResultBestCombo, playerComboBest, 0.8f));
        for (float f = 0; f < 0.5f; f += Time.deltaTime * animatorResults.speed) yield return null;
        StartCoroutine(TextIntGradualIncrease(textResultJudgeBest, playerAccuracyBest, 0.6f));
        for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
        StartCoroutine(TextIntGradualIncrease(textResultJudgeGreat, playerAccuracyGreat, 0.5f));
        for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
        StartCoroutine(TextIntGradualIncrease(textResultJudgeFine, playerAccuracyFine, 0.4f));
        for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
        StartCoroutine(TextIntGradualIncrease(textResultJudgeMiss, playerAccuracyMiss, 0.3f));

        for (float f = 0; f < 1.2f; f += Time.deltaTime * animatorResults.speed) yield return null;
        if (!isScoringDisabled)
        {
            if (finalAccuracy > oldRecordAccuracy)
            {
                animatorNewRecord.gameObject.SetActive(true);
                animatorNewRecord.Play("clip");
            }
            textResultOther.text = Translator.GetStringTranslation("GAME_RESULTSCOREADD", "Score:") + " " + finalScore.ToString();
        }
        /*
        int newPlayerLevel = PlayerSetting.setting.GetPlayerLevel();
        if (initialPlayerLevel < newPlayerLevel)
        {
            Notification.Display(Translator.GetStringTranslation("GAME_RESULTLEVELUP", "LEVEL UP!\nYour Play Level has rose to") + " " + newPlayerLevel.ToString(), Color.cyan);
        }
        */
    }
    IEnumerator AnimatorResultsSpeedUp()
    {
        yield return null;
        while (true)
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1) ||
                Input.GetKey(KeyCode.Space))
            {
                animatorResults.speed = 4f;
            }
            yield return null;
        }
    }
    IEnumerator TextFloatGradualIncrease(Text text, float value, float duration)
    {
        yield return null;

        for (float f = 0f; f < duration; f += Time.deltaTime * animatorResults.speed)
        {
            text.text = ((f / duration) * value).ToString("f2") + "%";
            yield return null;
        }

        text.text = value.ToString("f2") + "%";
    }
    IEnumerator TextIntGradualIncrease(Text text, int value, float duration)
    {
        yield return null;

        for (float f = 0f; f < duration; f += Time.deltaTime * animatorResults.speed)
        {
            text.text = Mathf.Floor((f / duration) * value).ToString("f0");
            yield return null;
        }

        text.text = value.ToString();
    }
}
