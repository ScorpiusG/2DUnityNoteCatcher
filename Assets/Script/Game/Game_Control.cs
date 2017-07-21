﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Control : MonoBehaviour
{
    public static bool boolCustomSong = true;
    public static string stringSongFileName = "test";
    public static int intChartGameType = 0;
    public static int intChartGameChart = 0;
    public static string stringModList = "";

    public static bool boolAutoplay = false;

    public GameObject objectAutoplayIndicator;
    public GameObject[] objectGroupType;
    public GameObject objectMouseCrosshair;
    public GameObject[] objectCatcher;
    public SpriteRenderer[] spriteRendererCatcherHighlight;

    public Game_Note notePrefab;
    private List<Game_Note> listNote;
    public Color[] noteColor = { Color.blue, Color.red, Color.green, Color.yellow };
    public float floatNoteDistanceSpawn = 3f;

    public AudioSource audioSourceMusic;

    public TextMesh textMeshComboCurrent;
    public int floatTextComboAppearComboMinimum = 4;
    public float floatTextComboScaleOnChange = 3f;
    public float floatTextComboScaleMinimum = 1f;
    public float floatTextComboScaleChangeRate = 12f;
    private float floatTextComboScaleCurrent = 1f;

    public Game_AnimationJudgment objectAnimationJudgmentPrefab;
    private List<Game_AnimationJudgment> listAnimationJudgment;
    public Sprite[] spriteJudgment;

    public Text textSongAndArtistName;
    public Text textSongDetails;
    public Text textSongProgressTime;
    public Image imageSongProgressGauge;
    public GameObject objectGroupInterfaceAccuracy;
    public Image imageAccuracyGauge;
    public Image imageAccuracyNegativeGauge;
    public Text textAccuracy;
    public Text textNoteJudgeCount;
    public Image imageAccuracyTolerance;
    public float floatAccuracyGaugeWidth = 928f;

    public Animator animatorResults;
    public Text textResultHeader;
    public Color colorResultHeaderPass = Color.green;
    public Color colorResultHeaderFail = Color.red;
    public Text textResultAccuracy;
    public Text textResultJudgeBest;
    public Text textResultJudgeGreat;
    public Text textResultJudgeFine;
    public Text textResultJudgeMiss;
    public Text textResultOther;
    public Text textScoreDisabled;
    public Text textRecordAccuracy;
    public Animator animatorNewRecord;

    private ChartData chartData;
    private int chartTotalNotes = 0;
    private int chartJudgeDifficulty = 0;
    private float floatMusicPosition = 0f;
    private float floatMusicBeat = 0f;
    public float floatHighlightAlpha = 0.5f;
    private float movementHoriAlpha = 0f;
    private float movementVertAlpha = 0f;

    public float[] floatDistAccuracyBest = { 0.16f, 0.14f, 0.125f, 0.11f, 0.1f };
    public float[] floatDistAccuracyGreat = { 0.19f, 0.165f, 0.15f, 0.125f, 0.113f };
    public float[] floatDistAccuracyFine = { 0.21f, 0.183f, 0.164f, 0.138f, 0.122f };

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
        SceneManager.LoadScene(sceneName);
    }

    public Game_Note SpawnNote()
    {
        foreach (Game_Note x in listNote)
        {
            if (x.gameObject.activeSelf)
            {
                return x;
            }
        }

        Game_Note newNote = Instantiate(notePrefab);
        listNote.Add(newNote);
        return newNote;
    }

    public void DespawnNote(Game_Note note)
    {
        note.gameObject.SetActive(false);
    }

    public void JudgeNote(Game_Note note, GameObject catcher)
    {
        float dist = Mathf.Abs(note.transform.position.x - catcher.transform.position.x);
        switch (chartData.chartGameType)
        {
            default:
                int animJudgeSprite = 0;
                int animSortingLayerID = 0;
                // BEST
                if (dist < floatDistAccuracyBest[chartJudgeDifficulty] || boolAutoplay)
                {
                    playerAccuracyBest++;
                    playerComboCurrent++;
                    animJudgeSprite = 0;
                    animSortingLayerID = playerComboCurrent;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (BEST)");
#endif
                }
                // GREAT
                else if (dist < floatDistAccuracyGreat[chartJudgeDifficulty])
                {
                    playerAccuracyGreat++;
                    playerComboCurrent++;
                    animJudgeSprite = 1;
                    animSortingLayerID = playerComboCurrent;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (GREAT)");
#endif
                }
                // FINE
                else if (dist < floatDistAccuracyFine[chartJudgeDifficulty])
                {
                    playerAccuracyFine++;
                    playerComboCurrent++;
                    animJudgeSprite = 2;
                    animSortingLayerID = playerComboCurrent;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (FINE)");
#endif
                }
                // MISS
                else
                {
                    animSortingLayerID = playerComboCurrent + 1;
                    playerAccuracyMiss++;
                    playerComboCurrent = 0;
                    animJudgeSprite = 3;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (MISS)");
#endif
                }

                // Show judgment animation
                if (PlayerSetting.setting.enableDisplayNoteJudgment)
                {
                    Game_AnimationJudgment anim = SpawnJudgeAnimation();
                    anim.gameObject.SetActive(true);
                    anim.transform.position = Vector3.right * note.position;
                    anim.gameObject.layer = 9 + note.type;
                    anim.spriteRendererJudgment.sprite = spriteJudgment[animJudgeSprite];
                    anim.spriteRendererJudgment.sortingLayerID = animSortingLayerID;
                    anim.animatorJudgment.Play("anim");
                }
                break;
        }

        if (playerComboCurrent > playerComboBest)
        {
            playerComboBest = playerComboCurrent;
        }
        if (PlayerSetting.setting.enableDisplayCombo)
        {
            textMeshComboCurrent.gameObject.SetActive(playerComboCurrent >= floatTextComboAppearComboMinimum);
            if (textMeshComboCurrent.gameObject.activeSelf)
            {
                floatTextComboScaleCurrent = floatTextComboScaleOnChange;
                textMeshComboCurrent.transform.localScale = Vector3.one * floatTextComboScaleCurrent;
            }
        }

        DespawnNote(note);
    }

    public Game_AnimationJudgment SpawnJudgeAnimation()
    {
        foreach (Game_AnimationJudgment x in listAnimationJudgment)
        {
            if (x.gameObject.activeSelf)
            {
                return x;
            }
        }

        Game_AnimationJudgment newAnim = Instantiate(objectAnimationJudgmentPrefab);
        listAnimationJudgment.Add(newAnim);
        return newAnim;
    }

    void Start ()
    {
        // Cursor lock
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        // Vertical synchronization
        if (PlayerSetting.setting.enableVSync)
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 9999;
        }

        // Enable/Disable groups depending on game type
        objectGroupType[0].SetActive(true);
        objectGroupType[1].SetActive(intChartGameType >= 1);
        objectGroupType[2].SetActive(intChartGameType >= 2);
        objectGroupType[3].SetActive(intChartGameType >= 2);

        // Object initialization
        objectAutoplayIndicator.SetActive(boolAutoplay);
        imageSongProgressGauge.fillAmount = 0f;
        if (PlayerSetting.setting.intAccuracyTolerance > 0)
        {
            imageAccuracyTolerance.transform.localPosition = Vector3.right * floatAccuracyGaugeWidth * 0.01f * PlayerSetting.setting.intAccuracyTolerance;
        }
        else
        {
            imageAccuracyTolerance.gameObject.SetActive(false);
        }
        foreach (GameObject x in objectCatcher)
        {
            x.transform.localPosition = Vector3.zero;
        }
        textNoteJudgeCount.gameObject.SetActive(PlayerSetting.setting.enableDisplayNoteHitCounterSmall);
        animatorResults.gameObject.SetActive(false);
        animatorNewRecord.gameObject.SetActive(false);
        textMeshComboCurrent.gameObject.SetActive(PlayerSetting.setting.enableDisplayCombo);

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
            string path = "Songs/" + stringSongFileName + "/" + chartFileName + ".txt";
            TextAsset info = Resources.Load(path) as TextAsset;
            input = info.text;
        }

        chartData = ScriptableObject.CreateInstance<ChartData>();
        JsonUtility.FromJsonOverwrite(input, chartData);

        //FileStream fileMusic = File.Open(Directory.GetCurrentDirectory() + "Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg", FileMode.Open);
        //audioSourceMusic.clip = (AudioClip)fileMusic as AudioClip;

        string gameTypeAbbr = "";

        if (PlayerSetting.setting.enableInterfaceSongDetails)
        {
            textSongAndArtistName.gameObject.SetActive(true);
            textSongDetails.gameObject.SetActive(true);
            textSongProgressTime.gameObject.SetActive(true);
            imageSongProgressGauge.gameObject.SetActive(true);
            textSongAndArtistName.text = chartData.songArtist + " - " + chartData.songName;
            textSongDetails.text = chartData.chartDeveloper + " - " + gameTypeAbbr + " #" + (intChartGameChart + 1).ToString() + " - Level" + chartData.chartLevel;
        }
        else
        {
            textSongAndArtistName.gameObject.SetActive(false);
            textSongDetails.gameObject.SetActive(false);
            textSongProgressTime.gameObject.SetActive(false);
            imageSongProgressGauge.gameObject.SetActive(false);
        }
        objectGroupInterfaceAccuracy.SetActive(PlayerSetting.setting.enableInterfaceAccuracy);
        chartTotalNotes = chartData.listNoteInfo.Count;
        chartJudgeDifficulty = chartData.chartJudge;
        if (chartJudgeDifficulty >= floatDistAccuracyBest.Length) chartJudgeDifficulty = floatDistAccuracyBest.Length - 1;

        StartCoroutine("GameLoop");
    }
	
	private IEnumerator GameLoop ()
    {
        // Load audio file
        if (boolCustomSong)
        {
            WWW www = new WWW("file://" + Directory.GetCurrentDirectory() + "/Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg");
            while (!www.isDone)
            {
                yield return null;
            }
#if UNITY_EDITOR
            if (www.error != "")
            {
                Debug.Log("Error message from reading audio file: " + www.error);
            }
#endif
            audioSourceMusic.clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
        }
        else
        {
            string path = "Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg";
            audioSourceMusic.clip = Resources.Load(path) as AudioClip;
        }

        yield return new WaitForSeconds(0.5f);
        // Play music and begin the game
        if (audioSourceMusic.clip == null)
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: " + stringSongFileName + ".ogg is not present or is corrupted.");
#endif
        }
        else
        {
            audioSourceMusic.Play();
#if UNITY_EDITOR
            Debug.Log("Playing song. The game begins.");
#endif
        }

        while (floatMusicPosition < chartData.songLength)
        {
            // Time update
            floatMusicPosition = audioSourceMusic.time + ((chartData.chartOffset - PlayerSetting.setting.intGameOffset) * 0.001f);
            floatMusicBeat =  floatMusicPosition * chartData.songTempo / 60f;

            // Highlight flash on beat
            float floatHighlightAlphaCurrent = 0f;
            if (PlayerSetting.setting.enableNoteAndCatcherHighlightBeatPulse)
            {
                floatHighlightAlphaCurrent = (floatMusicBeat - Mathf.Floor(floatMusicBeat)) * floatHighlightAlpha;
            }
            Color colorBeatFlash = Color.white;
            colorBeatFlash.a = floatHighlightAlphaCurrent;

            // Crosshair position
            Vector3 mouseCursorPos = objectMouseCrosshair.transform.position;
            // Normal play
            if (!boolAutoplay)
            {
                // Mouse movement
                movementHoriAlpha = Input.GetAxisRaw("MouseX");
                movementVertAlpha = Input.GetAxisRaw("MouseY");

                mouseCursorPos.x = Mathf.Clamp(mouseCursorPos.x + movementHoriAlpha * PlayerSetting.setting.floatMouseSensitivity, -1f, 1f);
                mouseCursorPos.y = Mathf.Clamp(mouseCursorPos.y + movementVertAlpha * PlayerSetting.setting.floatMouseSensitivity, -1f, 1f);
                objectMouseCrosshair.transform.position = mouseCursorPos;
            }
            // Automatic play (always perfect)
            else
            {
                Game_Note nextNoteCatcherHori = null;
                Game_Note nextNoteCatcherVert = null;
                foreach (Game_Note x in listNote)
                {
                    if (x.gameObject.activeSelf)
                    {
                        switch (x.type)
                        {
                            case 0:
                            case 1:
                                if (nextNoteCatcherHori == null || x.transform.position.y > nextNoteCatcherHori.transform.position.y)
                                {
                                    nextNoteCatcherHori = x;
                                }
                                break;
                            case 2:
                            case 3:
                                if (nextNoteCatcherVert == null || x.transform.position.y > nextNoteCatcherVert.transform.position.y)
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
                            mouseCursorPos.x = Mathf.Lerp(mouseCursorPos.x, nextNoteCatcherHori.position, Time.deltaTime * posLerpRate / nextNoteCatcherVert.transform.position.y);
                            break;
                        case 1:
                            mouseCursorPos.x = Mathf.Lerp(mouseCursorPos.x, -nextNoteCatcherHori.position, Time.deltaTime * posLerpRate / nextNoteCatcherVert.transform.position.y);
                            break;
                    }
                }
                if (nextNoteCatcherVert != null)
                {
                    switch (nextNoteCatcherVert.type)
                    {
                        case 2:
                            mouseCursorPos.x = Mathf.Lerp(mouseCursorPos.x, nextNoteCatcherVert.position, Time.deltaTime * posLerpRate / nextNoteCatcherVert.transform.position.y);
                            break;
                        case 3:
                            mouseCursorPos.x = Mathf.Lerp(mouseCursorPos.x, -nextNoteCatcherVert.position, Time.deltaTime * posLerpRate / nextNoteCatcherVert.transform.position.y);
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
                    Mathf.Floor(floatMusicPosition / 60f).ToString() + ":" + Mathf.Floor(floatMusicPosition % 60f).ToString("00") + " / " +
                    Mathf.Floor(chartData.songLength / 60f).ToString() + ":" + Mathf.Floor(chartData.songLength % 60f).ToString("00");
            }
            if (imageSongProgressGauge.gameObject.activeSelf)
            {
                imageSongProgressGauge.fillAmount = floatMusicPosition / chartData.songLength;
            }
            // Bottom - Accuracy
            currentAccuracy = (playerAccuracyBest * 4) + (playerAccuracyGreat * 3) + (playerAccuracyFine * 2);
            currentAccuracyNegative = playerAccuracyGreat + (playerAccuracyFine * 2) + (playerAccuracyMiss * 4);
            if (objectGroupInterfaceAccuracy.activeSelf)
            {
                imageAccuracyGauge.fillAmount = 1f * currentAccuracy / chartTotalNotes;
                imageAccuracyNegativeGauge.fillAmount = 1f * currentAccuracyNegative / chartTotalNotes;
                textAccuracy.text = (imageAccuracyGauge.fillAmount * 100f).ToString("f2") + "%";
            }
            if (PlayerSetting.setting.enableDisplayNoteHitCounterSmall)
            {
                textNoteJudgeCount.text =
                    "B" + playerAccuracyBest.ToString() + "\n" +
                    "G" + playerAccuracyGreat.ToString() + "\n" +
                    "F" + playerAccuracyFine.ToString() + "\n" +
                    "M" + playerAccuracyMiss.ToString();
            }
            // Center - Combo and judgment
            floatTextComboScaleCurrent = Mathf.Clamp(floatTextComboScaleCurrent - Time.deltaTime * floatTextComboScaleChangeRate, floatTextComboScaleMinimum, floatTextComboScaleOnChange);
            textMeshComboCurrent.text = playerComboCurrent.ToString();
            textMeshComboCurrent.transform.localScale = Vector3.one * floatTextComboScaleCurrent;

            // Note spawning
            for (int j = 0; j < chartData.listNoteInfo.Count; j++)
            {
                string[] noteInfo = chartData.listNoteInfo[j].Split('|');
                float time = float.Parse(noteInfo[2]);
                // Spawn note if position of note < current beat pos / FOV (scroll speed)
                if (time < floatMusicBeat + (floatNoteDistanceSpawn / (0.01f * PlayerSetting.setting.intScrollSpeed)))
                {
                    Game_Note note = SpawnNote();
                    note.type = int.Parse(noteInfo[0]);
                    note.size = int.Parse(noteInfo[1]);
                    note.time = time;
                    note.position = float.Parse(noteInfo[3]);
                    note.other = new List<string>();
                    if (noteInfo.Length > 5)
                    {
                        for (int i = 5; i < noteInfo.Length; i++)
                        {
                            note.other.Add(noteInfo[i]);
                        }
                    }
                    note.gameObject.layer = 9 + note.type;
                    note.spriteRendererNote.color = noteColor[note.type];
                    note.gameObject.SetActive(true);

                    // If note has length, create a second note with a line below it
                    float longNoteLength = float.Parse(noteInfo[4]);
                    if (longNoteLength > 0.01f)
                    {
                        note = SpawnNote();
                        note.type = int.Parse(noteInfo[0]);
                        note.size = int.Parse(noteInfo[1]);
                        note.time = time;
                        note.position = float.Parse(noteInfo[3]) + longNoteLength;
                        note.length = longNoteLength;
                        note.other = new List<string>();
                        note.gameObject.layer = 9 + note.type;
                        note.spriteRendererNote.color = noteColor[note.type];

                        note.spriteRendererLength.gameObject.SetActive(true);
                        note.spriteRendererLength.transform.localPosition = Vector3.down * longNoteLength * (0.01f * PlayerSetting.setting.intScrollSpeed) / 2f;
                        note.spriteRendererLength.transform.localScale = new Vector3(
                            note.spriteRendererLength.transform.localScale.x,
                            longNoteLength * (0.01f * PlayerSetting.setting.intScrollSpeed),
                            1f);
                        note.spriteRendererLength.color = noteColor[note.type];
                    }
                    else
                    {
                        note.spriteRendererLength.gameObject.SetActive(false);
                    }

                    chartData.listNoteInfo.RemoveAt(j);
                }
            }

            // Note positioning
            foreach (Game_Note x in listNote)
            {
                if (x.gameObject.activeSelf)
                {
                    x.transform.position = new Vector3(x.position, (floatMusicBeat - x.time) * (0.01f * PlayerSetting.setting.intScrollSpeed));

                    // Note flash
                    x.spriteRendererNoteHighlight.color = colorBeatFlash;
                    if (x.length > 0.01f)
                    {
                        x.spriteRendererLengthHighlight.color = colorBeatFlash;
                    }

                    // Note judgment
                    // Normal note or long note end - Go below pos 0 vertically
                    // Long note length - Sway too far from the note's center enough to get a "Miss"
                    if (x.transform.position.y <= 0f || 
                        (x.transform.position.y - (x.length * (0.01f * PlayerSetting.setting.intScrollSpeed)) < 0f &&
                        Mathf.Abs(x.transform.position.x - objectCatcher[x.type].transform.position.x) > floatDistAccuracyFine[chartJudgeDifficulty])
                        )
                    {
                        JudgeNote(x, objectCatcher[x.type]);
                    }
                }
            }

            // Player force end: Hold [Escape]
            if (Input.GetKey(KeyCode.Escape))
            {
                floatTimeEscapeHeld += Time.deltaTime;
            }
            else
            {
                floatTimeEscapeHeld = 0f;
            }
            if (floatTimeEscapeHeld > 2f)
            {
                isForcedEnd = true;
            }

            // Alternate force end condition: Current negative accuracy is below tolerance
            if (1f * currentAccuracyNegative / chartTotalNotes > 0.01f * PlayerSetting.setting.intAccuracyTolerance)
            {
                isForcedEnd = true;
            }

            // Force end consequences: Miss all remaining notes
            if (isForcedEnd)
            {
                if (floatTimeEscapeHeld > 2f)
                {
                    foreach (string s in chartData.listNoteInfo)
                    {
                        string[] noteInfo = s.Split('|');
                        playerAccuracyMiss++;
                        float longNoteLength = float.Parse(noteInfo[4]);
                        if (longNoteLength > 0.01f)
                        {
                            playerAccuracyMiss++;
                        }
                    }
                }
                foreach (Game_Note x in listNote)
                {
                    if (x.gameObject.activeSelf)
                    {
                        playerAccuracyMiss++;
                        DespawnNote(x);
                    }
                }
                audioSourceMusic.Stop();
                break;
            }

            yield return null;
        }
        imageSongProgressGauge.fillAmount = 1f;

        float finalAccuracy = ((playerAccuracyBest * 4) + (playerAccuracyGreat * 3) + (playerAccuracyFine * 2)) / (chartTotalNotes * 4);
#if UNITY_EDITOR
        Debug.Log("Song finished. Accuracy: " + (finalAccuracy * 100f).ToString("f2") + "%");
#endif
        // Revert settings (cursor, v-sync, etc.)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        objectAutoplayIndicator.SetActive(false);
        textMeshComboCurrent.gameObject.SetActive(false);
        yield return null;

        // Add and record score
        textScoreDisabled.gameObject.SetActive(isScoringDisabled);
        int finalScore = 0;
        float oldRecordAccuracy = 0f;
        if (!isScoringDisabled)
        {
            float gameModeScoreMultiplier = 1f;
            switch(chartData.chartGameType)
            {
                case 0: gameModeScoreMultiplier = 1f; break;
                case 1: gameModeScoreMultiplier = 1.4f; break;
                case 2: case 3: case 4: gameModeScoreMultiplier = 1.8f; break;
            }
            // Score based on accuracy, best combo, chart level, and game mode.
            finalScore = Mathf.FloorToInt(
                (1f * playerComboBest / chartTotalNotes) * finalAccuracy *
                Mathf.Pow(4 + chartData.chartLevel, 2f) *
                10 * gameModeScoreMultiplier
                );
            // Additional score gained by achieving full combo and perfect accuracy.
            int additionalScore = 0;
            if (playerComboBest == chartTotalNotes)
            {
                additionalScore += finalScore / 10;
            }
            if (playerAccuracyBest == chartTotalNotes)
            {
                additionalScore += finalScore / 10;
            }
            finalScore += additionalScore;
            PlayerSetting.setting.ScoreAdd(finalScore);

            oldRecordAccuracy = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString(), 0f);
            textRecordAccuracy.gameObject.SetActive(true);
            textRecordAccuracy.text = (oldRecordAccuracy * 100f).ToString("f2");

            if (finalAccuracy > oldRecordAccuracy)
            {
                PlayerPrefs.SetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString(), finalAccuracy);
            }
            PlayerSetting.setting.Save();
        }
        else
        {
            textRecordAccuracy.gameObject.SetActive(false);
        }
        yield return null;

        // Update texts
        if (1f * finalAccuracy / chartTotalNotes >= 0.01f * PlayerSetting.setting.intAccuracyTolerance)
        {
            textResultHeader.text = "STAGE PASSED";
            textResultHeader.color = colorResultHeaderPass;
        }
        else
        {
            textResultHeader.text = "STAGE FAILED";
            textResultHeader.color = colorResultHeaderFail;
        }

        // Show result screen
        animatorResults.gameObject.SetActive(true);
        animatorResults.Play("clip");

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TextFloatGradualIncrease(textResultAccuracy, finalAccuracy, 0.8f));
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(TextIntGradualIncrease(textResultJudgeBest, playerAccuracyBest, 0.6f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(TextIntGradualIncrease(textResultJudgeGreat, playerAccuracyGreat, 0.6f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(TextIntGradualIncrease(textResultJudgeFine, playerAccuracyFine, 0.6f));
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(TextIntGradualIncrease(textResultJudgeMiss, playerAccuracyMiss, 0.6f));

        yield return new WaitForSeconds(1.5f);
        if (finalAccuracy > oldRecordAccuracy)
        {
            animatorNewRecord.Play("clip");
        }
        if (isScoringDisabled)
        {
            textResultOther.text = stringModList;
        }
        else
        {
            textResultOther.text = "Score: " + finalScore.ToString();
        }
    }
    IEnumerator TextFloatGradualIncrease(Text text, float value, float duration)
    {
        yield return null;

        for (float f = 0f; f < duration; f += Time.deltaTime)
        {
            text.text = ((f / duration) * value).ToString("f2") + "%";
            yield return null;
        }

        text.text = value.ToString("f2") + "%";
    }
    IEnumerator TextIntGradualIncrease(Text text, int value, float duration)
    {
        yield return null;

        for (float f = 0f; f < duration; f += Time.deltaTime)
        {
            text.text = Mathf.Floor((f / duration) * value).ToString("f0");
            yield return null;
        }

        text.text = value.ToString();
    }
}
