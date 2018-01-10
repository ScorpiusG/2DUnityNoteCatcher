using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
//using UnityEngine.Networking;
using UnityEngine.UI;
//using TMPro;

public class Game_Control : MonoBehaviour
{
    public static Game_Control control;
    public static bool boolCustomSong = true;
    public static string stringSongFileName = "test";
    public static int intChartGameType = 0;
    public static int intChartGameChart = 0;
    public static string stringModList = "";

    public static MarathonMenu_Item marathonItem = null;
    public static int marathonItemID = 0;
    public static int intMarathonItem = 0;
    private static int marathonAccuracyBest = 0;
    private static int marathonAccuracyGreat = 0;
    private static int marathonAccuracyFine = 0;
    private static int marathonAccuracyMiss = 0;
    private static int marathonNoteDodgeHit = 0;
    private static int marathonComboCurrent = 0;
    private static int marathonComboBest = 0;
    private static float marathonLength = 0f;

    public static bool boolAutoplay = false;

    public GameObject objectAutoplayIndicator;
    public GameObject[] objectGroupType;
    public GameObject objectMouseCursor;
    public GameObject objectMouseCursorCrosshair;
    public GameObject objectMouseCursorDodger;
    public GameObject[] objectCatcher;
    public SpriteRenderer[] spriteRendererCatcherHighlight;
    private int intNoteDodgeHit = 0;
    public int intNoteDodgeHitMaximum = 25;

    public GameObject[] objectEnableGameModeAllCatchAndTap;
    public GameObject[] objectEnableGameModeNoteDodge;

    public Camera cameraMain;
    public Camera cameraObjectPlayer;
    public Camera[] cameraGame;
    public float floatCameraRotation = 0f;
    public bool boolCameraRotationNoLerp = false;
    public float floatCameraRotationChangeRate = 0f;
    public float floatCameraRotationLerpRate = 8f;

    public Game_Note noteCatchPrefab;
    private List<Game_Note> listNoteCatch = new List<Game_Note>();
    public Game_Note noteTapPrefab;
    private List<Game_Note> listNoteTap = new List<Game_Note>();
    public Game_NoteBullet noteBulletPrefab;
    private List<Game_NoteBullet> listNoteBullet = new List<Game_NoteBullet>();
    public Game_NoteItem noteItemPrefab;
    private List<Game_NoteItem> listNoteItem = new List<Game_NoteItem>();
    public Color[] noteColor = { Color.blue, Color.red, Color.green, Color.yellow };
    public float floatNoteDistanceSpawn = 3f;

    public Renderer rendererPlaneBackground;
    public SpriteRenderer rendererBackgroundCover;
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
    public Text textChartChecksum;

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
    [HideInInspector] public bool boolIsPaused = false;
    private bool boolPauseMenuForceEnd = false;
    private float floatLastPaused = -999f;
    private bool boolPauseNotification = false;

    public Text textDebug;

    private ChartData chartData;
    private float chartCurrentTempo = 60f;
    private int chartTotalNotes = 0;
    private int chartJudgeDifficulty = 0;
    private float floatMusicPosition = 0f;
    private float floatMusicPositionEnd = 0f;
    private float floatMusicBeat = 0f;
    public float floatHighlightAlpha = 0.5f;
    private float movementHoriAlpha = 0f;
    private float movementVertAlpha = 0f;
    private const float movementAlphaMultiplier = 0.5f;

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
    public AudioClip clipGameNoteAssistTick;
    public AudioClip clipGameBulletHit;

    public float floatNoteScrollMultiplier = 0.05f;
    public float floatNoteCatchLongHealthDropDuration = 0.4f;
    public float[] floatDistAccuracyCatchBest = { 0.12f, 0.11f, 0.1f, 0.09f, 0.81f };
    public float[] floatDistAccuracyCatchGreat = { 0.15f, 0.139f, 0.128f, 0.115f, 0.101f };
    public float[] floatDistAccuracyCatchFine = { 0.168f, 0.156f, 0.142f, 0.128f, 0.114f };
    public float[] floatDistAccuracyTapBest = { 0.12f, 0.11f, 0.1f, 0.09f, 0.81f };
    public float[] floatDistAccuracyTapGreat = { 0.15f, 0.139f, 0.128f, 0.115f, 0.101f };
    public float[] floatDistAccuracyTapFine = { 0.168f, 0.156f, 0.142f, 0.128f, 0.114f };
    public float[] floatDistAccuracyTapMiss = { 0.17f, 0.16f, 0.15f, 0.14f, 0.13f };
    public float floatNoteDodgeBulletHitboxRadius = 0.08f;
    public float floatNoteDodgeItemHitboxRadius = 0.2f;
    public float floatNoteDodgeItemSpawnFrequency = 4f;
    public float floatNoteDodgeInvincibilityPeriod = 1f;
    private float floatNoteDodgeInvincibilityPeriodCurrent = 2f;
    public int[] intNoteDodgeBulletRingSpawnQuantity = { 12, 14, 16, 18, 20, 22, 24, 26, 28, 30 };
    public float[] floatNoteDodgeBulletLongNoteSpawnFrequency = { 1f, 1f, 1f, 2f, 2f, 2f, 4f, 4f, 4f, 8f, 8f };

    [HideInInspector] public bool isForcedEnd = false;
    private bool boolIsTutorial = false;
    private bool isScoringDisabled = false;
    private float floatTimeEscapeHeld = 0f;
    private int currentScrollSpeed = 0;
    private int currentAccuracy = 0;
    private int currentAccuracyNegative = 0;
    private int playerAccuracyBest = 0;
    private int playerAccuracyGreat = 0;
    private int playerAccuracyFine = 0;
    private int playerAccuracyMiss = 0;
    private int playerComboCurrent = 0;
    private int playerComboBest = 0;
    //private float timeItemLastSpawn = 0f;
    //private int itemQuantityTotal = 0;

    private bool lastHitNoteIsTap = false;
    private float lastHitNoteDistance = 0f;
    private int lastHitNoteType = 0;

    private const float NOTE_LERP_RATE_MULTIPLIER = 4f;

    public void RestartGameScene()
    {
        if (marathonItem != null)
        {
            intMarathonItem = 0;
        }
        LoadScene("Game");
    }
    public void ExitGameScene()
    {
        if (boolIsTutorial)
        {
            LoadScene("Title");
        }
        else if (marathonItem != null)
        {
            LoadScene("MarathonMenu");
        }
        else
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
    }
    public void LoadScene(string sceneName)
    {
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
    private Game_NoteBullet SpawnNoteBullet()
    {
        foreach (Game_NoteBullet x in listNoteBullet)
        {
            if (!x.gameObject.activeSelf)
            {
                x.transform.position = new Vector3(0f, 100f, 0f);
                return x;
            }
        }

        Game_NoteBullet newNote = Instantiate(noteBulletPrefab);
        listNoteBullet.Add(newNote);
        return newNote;
    }
    private Game_NoteItem SpawnNoteItem()
    {
        foreach (Game_NoteItem x in listNoteItem)
        {
            if (!x.gameObject.activeSelf)
            {
                x.transform.position = new Vector3(0f, 100f, 0f);
                return x;
            }
        }

        Game_NoteItem newNote = Instantiate(noteItemPrefab);
        listNoteItem.Add(newNote);
        return newNote;
    }

    private void DespawnNote(Game_Note note)
    {
        note.gameObject.SetActive(false);
    }
    private void DespawnNote(Game_NoteBullet note)
    {
        note.gameObject.SetActive(false);
    }
    private void DespawnNote(Game_NoteItem note)
    {
        note.gameObject.SetActive(false);
    }

    private void JudgeNote(Game_Note note, GameObject catcher, bool isTapNote, bool forceMiss = false)
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
                    if (!forceMiss && (dist < floatDistAccuracyCatchBest[chartJudgeDifficulty] || boolAutoplay))
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
                    else if (!forceMiss && dist < floatDistAccuracyCatchGreat[chartJudgeDifficulty])
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
                    else if (!forceMiss && dist < floatDistAccuracyCatchFine[chartJudgeDifficulty])
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
                    if (!forceMiss && (dist < floatDistAccuracyTapBest[chartJudgeDifficulty] || boolAutoplay))
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
                    else if (!forceMiss && dist < floatDistAccuracyTapGreat[chartJudgeDifficulty])
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
                    else if (!forceMiss && dist < floatDistAccuracyTapFine[chartJudgeDifficulty])
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
            if (PlayerSetting.setting.enableAssistTickSound)
            {
                PlaySoundEffect(clipGameNoteAssistTick);
            }

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

    void Awake()
    {
        control = this;

        // Check marathon item
        if (marathonItem != null)
        {
            string[] x = marathonItem.itemChartList[intMarathonItem].Split('|');
            stringSongFileName = x[0];
            intChartGameType = int.Parse(x[1]);
            intChartGameChart = int.Parse(x[2]);

            // Reset all variables
            if (intMarathonItem == 0)
            {
                int marathonAttempts = PlayerPrefs.GetInt("marathon-" + marathonItemID + "-attempts", 0);
                marathonAttempts++;
                PlayerPrefs.SetInt("marathon-" + marathonItemID + "-attempts", marathonAttempts);
                PlayerPrefs.Save();

                marathonAccuracyBest = 0;
                marathonAccuracyGreat = 0;
                marathonAccuracyFine = 0;
                marathonAccuracyMiss = 0;
                marathonComboCurrent = 0;
                marathonComboBest = 0;
                marathonLength = 0f;
            }
            else
            {
                playerComboCurrent = marathonComboCurrent;
                playerComboBest = marathonComboBest;
            }
        }
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
        objectMouseCursorCrosshair.SetActive(intChartGameType != 3);
        objectMouseCursorCrosshair.GetComponent<SpriteRenderer>().enabled = PlayerSetting.setting.enableMouseCrosshair;
        objectMouseCursorDodger.SetActive(intChartGameType == 3);

        if (intChartGameType == 3)
        {
            foreach (GameObject x in objectCatcher)
            {
                x.SetActive(false);
            }
        }

        // Variable initialization
        if (boolCustomSong)
        {
            floatPreviousRecord = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy", 0f);

            if (marathonItem == null && !boolAutoplay)
            {
                // Mapchart play count increase
                int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", 0);
                songPlayCount++;
                PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", songPlayCount);
                PlayerPrefs.Save();
            }
        }
        else
        {
            floatPreviousRecord = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy_official", 0f);

            if (marathonItem == null && !boolAutoplay)
            {
                // Mapchart play count increase
                int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount_official", 0);
                songPlayCount++;
                PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount_official", songPlayCount);
                PlayerPrefs.Save();
            }
        }

        textMeshRecordGhost.text = "";
        float textAlpha = textMeshRecordGhost.color.a;
        colorRecordGhostBetter.a = textAlpha;
        colorRecordGhostNeutral.a = textAlpha;
        colorRecordGhostWorse.a = textAlpha;
        audioSourceMusic.volume = PlayerSetting.setting.floatVolumeMusic;
        currentScrollSpeed = PlayerSetting.setting.intScrollSpeed;
        //audioSourceEffect.volume = PlayerSetting.setting.floatVolumeEffect;

        // Set certain variables based modifiers
        if (marathonItem != null && marathonItem.itemModList.Length > 0)
        {
            foreach (string x in marathonItem.itemModList)
            {
                string[] y = x.Split(':');
                switch (y[0])
                {
                    case "tutorial":
                        boolIsTutorial = true;
                        Instantiate(Resources.Load("CanvasTutorial" + intMarathonItem, typeof(GameObject)) as GameObject);
                        break;
                    case "scrollspeed":
                        currentScrollSpeed = int.Parse(y[1]);
                        break;
                }
            }
        }

        // Object initialization
        textDebug.gameObject.SetActive(Debug.isDebugBuild || Application.isEditor);
        rendererPlaneBackground.gameObject.SetActive(false);
        objectAutoplayIndicator.SetActive(boolAutoplay);
        imageSongProgressGauge.fillAmount = 0f;
        if (marathonItem == null && PlayerSetting.setting.intAccuracyThreshold > 0)
        {
            imageAccuracyTolerance.transform.localPosition += Vector3.right * floatAccuracyGaugeWidth * 0.01f * PlayerSetting.setting.intAccuracyThreshold;
        }
        else if (marathonItem != null && marathonItem.itemAccuracyThreshold > 0)
        {
            imageAccuracyTolerance.transform.localPosition += Vector3.right * floatAccuracyGaugeWidth * 0.01f * marathonItem.itemAccuracyThreshold;
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
        if (intChartGameType == 3)
        {
            cameraObjectPlayer.depth += 100;
        }
        //cameraMain.backgroundColor *= PlayerSetting.setting.floatBackgroundBrightness;
        if (PlayerSetting.setting.floatBackgroundBrightness > 0.9999f)
        {
            rendererBackgroundCover.gameObject.SetActive(false);
        }
        else
        {
            rendererBackgroundCover.gameObject.SetActive(true);
            rendererBackgroundCover.color = new Color(0f, 0f, 0f, 1f - PlayerSetting.setting.floatBackgroundBrightness);
        }
        textNoteJudgeCount.gameObject.SetActive(PlayerSetting.setting.enableDisplayNoteHitCounterSmall);
        textNoteJudgeCount.text = "";
        animatorResults.gameObject.SetActive(false);
        animatorNewRecord.gameObject.SetActive(false);
        textMeshComboCurrent.gameObject.SetActive(false);
        textMeshComboCurrent.text = "";

        // Ghost accuracy display
        if (marathonItem == null)
        {
            // Normal
            textMeshRecordGhost.gameObject.SetActive(PlayerSetting.setting.enableDisplayRecordGhost && floatPreviousRecord > Mathf.Epsilon && !boolAutoplay);
            foreach (GameObject x in objectEnableGameModeAllCatchAndTap)
            {
                x.SetActive(intChartGameType != 3);
            }
            foreach (GameObject x in objectEnableGameModeNoteDodge)
            {
                x.SetActive(intChartGameType == 3);
            }
        }
        else
        {
            // Marathon - Show MISS count
            textMeshRecordGhost.gameObject.SetActive(marathonItem.itemNoteMissThreshold > 0);
            textMeshRecordGhost.color = colorRecordGhostWorse;

            foreach (GameObject x in objectEnableGameModeAllCatchAndTap)
            {
                x.SetActive(true);
            }
            foreach (GameObject x in objectEnableGameModeNoteDodge)
            {
                x.SetActive(false);
            }
        }
        if (PlayerSetting.setting.enableDisplayRecordGhost && !PlayerSetting.setting.enableDisplayCombo)
        {
            textMeshRecordGhost.transform.position = Vector3.zero;
        }

        textChartChecksum.gameObject.SetActive(false);

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

            reader = new StreamReader(path);
            MD5 md5checksum = MD5.Create();
            byte[] byteChecksum = md5checksum.ComputeHash(reader.BaseStream);
            textChartChecksum.gameObject.SetActive(true);
            textChartChecksum.text = Translator.GetStringTranslation("GAME_CHARTCHECKSUM", "Chart MD5 Checksum:") + "\n";
            foreach (byte x in byteChecksum)
            {
                textChartChecksum.text += x;
            }
            reader.Close();
        }
        // Chart is built-in
        else
        {
            string chartFileName = "Songs/" + stringSongFileName + "/" + stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString();
            //string path = "Songs/" + chartFileName + ".txt";
            TextAsset info = Resources.Load(chartFileName) as TextAsset;
            input = info.text;

            textChartChecksum.gameObject.SetActive(false);
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
            string path = "Songs/" + stringSongFileName + "/background";
            textureBackground = Resources.Load(path) as Texture;
        }
        if (textureBackground != null)
        {
            rendererPlaneBackground.gameObject.SetActive(true);
            rendererPlaneBackground.material.mainTexture = textureBackground;
            //rendererPlaneBackground.material.color = new Color(PlayerSetting.setting.floatBackgroundBrightness, PlayerSetting.setting.floatBackgroundBrightness, PlayerSetting.setting.floatBackgroundBrightness);
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

        if (intChartGameType == 3)
        {
            for (int i = 0; i < noteColor.Length; i++)
            {
                Color c = noteColor[i];
                c.a *= 0.6f;
                noteColor[i] = c;
            }
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
        textAccuracy.text = "";
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
        floatCameraRotation += floatCameraRotationChangeRate * Time.deltaTime * chartCurrentTempo / 60f;
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

        chartCurrentTempo = chartData.songTempo;
        floatMusicPositionEnd = (chartData.songLength * 60f / chartData.songTempo);
        if (chartData.listTempoChanges.Count > 0)
        {
            Vector3 lastChange = new Vector3(0f, chartData.songTempo, 0f);
            foreach (Vector3 x in chartData.listTempoChanges)
            {
                if (lastChange.x < x.x)
                {
                    lastChange = x;
                }
            }

            floatMusicPositionEnd = ((chartData.songLength - lastChange.x) * 60f / lastChange.y) + lastChange.z;
        }
        floatMusicPositionEnd += (chartData.chartOffset + PlayerSetting.setting.intGameOffset) * 0.001f;
        //timeItemLastSpawn = -4f;
        //itemQuantityTotal = Mathf.FloorToInt((chartData.gameplayLength - ((floatNoteDodgeItemSpawnFrequency * 4f) + Mathf.Epsilon)) / 4f) * 1000;

        Vector3 currentTempoChange = new Vector3(0f, chartCurrentTempo, 0f);

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
            if (chartData.listTempoChanges.Count > 0)
            {
                foreach (Vector3 x in chartData.listTempoChanges)
                {
                    if (floatMusicPosition > x.z)
                    {
                        currentTempoChange = x;
                    }
                }
            }

            floatMusicPosition = mSongLoader.audioSourceMusic.time - ((chartData.chartOffset + PlayerSetting.setting.intGameOffset) * 0.001f);
            //floatMusicPosition = audioSourceMusic.time - ((chartData.chartOffset + PlayerSetting.setting.intGameOffset) * 0.001f);
            chartCurrentTempo = currentTempoChange.y;
            floatMusicBeat = ((floatMusicPosition - currentTempoChange.z) * chartCurrentTempo / 60f) + currentTempoChange.x;

            // Invincibility timer for Note Dodge
            floatNoteDodgeInvincibilityPeriodCurrent -= Time.deltaTime;

            // Highlight flash on beat
            float floatHighlightAlphaCurrent = 0f;
            if (PlayerSetting.setting.enableNoteAndCatcherHighlightBeatPulse)
            {
                floatHighlightAlphaCurrent = (1f - (floatMusicBeat - Mathf.Floor(floatMusicBeat))) * floatHighlightAlpha;
            }
            Color colorBeatFlash = Color.white;
            colorBeatFlash.a = floatHighlightAlphaCurrent;

            // Crosshair position
            Vector3 mouseCursorPos = objectMouseCursor.transform.position;
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

                mouseCursorPos.x += movementHoriAlpha * PlayerSetting.setting.floatMouseSensitivity * movementAlphaMultiplier;
                mouseCursorPos.y += movementVertAlpha * PlayerSetting.setting.floatMouseSensitivity * movementAlphaMultiplier;
                objectMouseCursor.transform.position = mouseCursorPos;
            }
            // Automatic play (always perfect)
            else
            {
                if (intChartGameType != 3)
                {
                    Game_Note nextNoteCatcherHori = null;
                    Game_Note nextNoteCatcherVert = null;
                    foreach (Game_Note x in listNoteCatch)
                    {
                        if (x.gameObject.activeInHierarchy)
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
                        switch (nextNoteCatcherHori.type)
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
                else
                {
                    bool isDodging = false;
                    foreach (Game_NoteBullet x in listNoteBullet)
                    {
                        if (x.gameObject.activeInHierarchy &&
                            Vector3.Distance(objectMouseCursorDodger.transform.position, x.transform.position) < floatNoteDodgeBulletHitboxRadius + (chartCurrentTempo / 30f) * Time.deltaTime * x.speed)
                        {
                            isDodging = true;
                            mouseCursorPos -= (x.transform.position - objectMouseCursorDodger.transform.position).normalized * (chartCurrentTempo / 60f) * Time.deltaTime;
                        }
                    }
                    if (!isDodging &&
                        (Mathf.Abs(objectMouseCursorDodger.transform.position.x) > 0.2f ||
                        Mathf.Abs(objectMouseCursorDodger.transform.position.y) > 0.2f))
                    {
                        mouseCursorPos += -objectMouseCursorDodger.transform.position * (chartCurrentTempo / 300f) * Time.deltaTime;
                    }
                    Game_NoteItem closestItem = null;
                    foreach (Game_NoteItem x in listNoteItem)
                    {
                        if (x.gameObject.activeInHierarchy && (closestItem == null ||
                            Vector3.Distance(x.transform.position, objectMouseCursorDodger.transform.position) < Vector3.Distance(closestItem.transform.position, objectMouseCursorDodger.transform.position)))
                        {
                            closestItem = x;
                        }
                    }
                    if (closestItem != null)
                    {
                        mouseCursorPos += (closestItem.transform.position - objectMouseCursorDodger.transform.position).normalized * (chartCurrentTempo / 60f) * Time.deltaTime;
                    }
                }
            }
            mouseCursorPos.x = Mathf.Clamp(mouseCursorPos.x, -1f, 1f);
            mouseCursorPos.y = Mathf.Clamp(mouseCursorPos.y, -1f, 1f);
            objectMouseCursor.transform.position = mouseCursorPos;

            // Catcher position
            objectCatcher[0].transform.position = Vector3.right * objectMouseCursor.transform.position.x;
            if (objectCatcher[1].activeInHierarchy)
            {
                objectCatcher[1].transform.position = Vector3.left * objectMouseCursor.transform.position.x;
            }
            if (objectCatcher[2].activeInHierarchy)
            {
                objectCatcher[2].transform.position = Vector3.left * objectMouseCursor.transform.position.y;
            }
            if (objectCatcher[3].activeInHierarchy)
            {
                objectCatcher[3].transform.position = Vector3.right * objectMouseCursor.transform.position.y;
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
                    Mathf.Floor(floatMusicPositionEnd / 60f).ToString() + ":" + Mathf.Floor(floatMusicPositionEnd % 60f).ToString("00");
            }
            if (imageSongProgressGauge.gameObject.activeSelf)
            {
                imageSongProgressGauge.fillAmount = floatMusicPosition / floatMusicPositionEnd;
            }
            // Bottom - Accuracy
            if (intChartGameType != 3)
            {
                currentAccuracy = (playerAccuracyBest * 4) + (playerAccuracyGreat * 2) + playerAccuracyFine;
                currentAccuracyNegative = (playerAccuracyGreat * 2) + (playerAccuracyFine * 3) + (playerAccuracyMiss * 4);

                if (PlayerSetting.setting.enableDisplayNoteHitCounterSmall)
                {
                    textNoteJudgeCount.text =
                        "B " + playerAccuracyBest.ToString() + "\n" +
                        "G " + playerAccuracyGreat.ToString() + "\n" +
                        "F " + playerAccuracyFine.ToString() + "\n" +
                        "M " + playerAccuracyMiss.ToString();
                }

                if (objectGroupInterfaceAccuracy.activeSelf)
                {
                    floatAccuracyDisplay = Mathf.Lerp(floatAccuracyDisplay, 1f * currentAccuracy / (chartTotalNotes * 4), Time.deltaTime * 8f);
                    imageAccuracyGauge.fillAmount = floatAccuracyDisplay;
                    imageAccuracyNegativeGauge.fillAmount = 1f * currentAccuracyNegative / (chartTotalNotes * 4);
                    
                    textAccuracy.text = (floatAccuracyDisplay * 100f).ToString("f2") + "%";
                }
            }
            else
            {
                currentAccuracy = Mathf.CeilToInt(1000000 * Mathf.Pow(0.95f, 1f * intNoteDodgeHit));
                //currentAccuracy = Mathf.FloorToInt((1000f * playerAccuracyBest / itemQuantityTotal) * Mathf.Pow(0.95f, 1f * Mathf.Clamp(intNoteDodgeHit, 0, intNoteDodgeHitMaximum)));
                currentAccuracyNegative = 1000000 - Mathf.CeilToInt(1000000 * Mathf.Pow(0.95f, 1f * intNoteDodgeHit));

                if (PlayerSetting.setting.enableDisplayNoteHitCounterSmall)
                {
                    textNoteJudgeCount.text = "H " + intNoteDodgeHit;
                }

                if (objectGroupInterfaceAccuracy.activeSelf)
                {
                    floatAccuracyDisplay = Mathf.Lerp(floatAccuracyDisplay, 1f * currentAccuracy / 1000000, Time.deltaTime * 8f);
                    imageAccuracyGauge.fillAmount = floatAccuracyDisplay;
                    imageAccuracyNegativeGauge.fillAmount = 1f * currentAccuracyNegative / 1000000;
                    
                    textAccuracy.text = "Hits: " + intNoteDodgeHit;
                }

                /*
                currentAccuracyNegative = (chartTotalNotes * 4) - Mathf.FloorToInt((chartTotalNotes * 4) * Mathf.Pow(0.95f, 1f * Mathf.Clamp(intNoteDodgeHit, 0, intNoteDodgeHitMaximum)));
                currentAccuracyNegative += Mathf.FloorToInt(((playerAccuracyGreat * 2) + (playerAccuracyFine * 3) + (playerAccuracyMiss * 4)) * Mathf.Pow(0.95f, 1f * Mathf.Clamp(intNoteDodgeHit, 0, intNoteDodgeHitMaximum)));

                currentAccuracy = Mathf.RoundToInt(currentAccuracy * Mathf.Pow(0.95f, 1f * Mathf.Clamp(intNoteDodgeHit, 0, intNoteDodgeHitMaximum)));
                */
            }

            // Center - Combo and judgment
            if (textMeshComboCurrent.gameObject.activeSelf)
            {
                floatTextComboScaleCurrent = Mathf.Clamp(floatTextComboScaleCurrent - Time.deltaTime * floatTextComboScaleChangeRate, floatTextComboScaleMinimum, floatTextComboScaleOnChange);
                textMeshComboCurrent.transform.localScale = Vector3.one * floatTextComboScaleCurrent;
            }
            if (textMeshRecordGhost.gameObject.activeSelf)
            {
                if (marathonItem == null)
                {
                    // Show ghost accuracy
                    float currentAccuracyPercentage = 0f;
                    float ghostAccuracy = 0f;

                    if (intChartGameType != 3)
                    {
                        currentAccuracyPercentage = 1f * currentAccuracy / (chartTotalNotes * 4);
                        ghostAccuracy = 1f * floatPreviousRecord * (playerAccuracyBest + playerAccuracyGreat + playerAccuracyFine + playerAccuracyMiss) / chartTotalNotes;
                    }
                    else
                    {
                        currentAccuracyPercentage = 1f * currentAccuracy / 1000000;
                        ghostAccuracy = 1f - ((1f - floatPreviousRecord) * (floatMusicPosition / floatMusicPositionEnd));
                    }

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
                else
                {
                    // Show MISS count
                    textMeshRecordGhost.text = "MISS  " + (playerAccuracyMiss + intNoteDodgeHit + marathonAccuracyMiss + marathonNoteDodgeHit).ToString() + " / " + marathonItem.itemNoteMissThreshold.ToString();
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
                if (time < floatMusicBeat + (floatNoteDistanceSpawn / floatNoteScrollMultiplier / currentScrollSpeed / speed))
                {
                    Game_Note note = SpawnNoteCatch();
                    note.health = 1f;
                    note.type = int.Parse(noteInfo[0]);
                    if (intChartGameType == 0 && note.type > 0)
                    {
                        note.type = 0;
                    }
                    else if (intChartGameType == 1 && note.type > 1)
                    {
                        note.type %= 2;
                    }
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
                    if (intChartGameType == 3) note.gameObject.layer += 4;
                    note.spriteRendererNote.color = noteColor[note.type];
                    note.gameObject.SetActive(true);
                    note.spriteRendererLength.gameObject.SetActive(false);

                    Vector3 notePos = new Vector3(note.position, (note.time - floatMusicBeat) * floatNoteScrollMultiplier * currentScrollSpeed * note.speed);
                    note.transform.position = notePos;

                    // If note has length, create a second note with a line below it
                    float longNoteLength = float.Parse(noteInfo[4]);
                    if (longNoteLength > 0.01f)
                    {
                        note = SpawnNoteCatch();
                        note.health = 1f;
                        note.type = int.Parse(noteInfo[0]);
                        note.size = int.Parse(noteInfo[1]);
                        note.time = time + longNoteLength;
                        note.position = Mathf.Clamp(float.Parse(noteInfo[3]), -1f, 1f);
                        note.length = longNoteLength;
                        note.speed = speed;
                        note.other = new List<string>();
                        note.gameObject.layer = 9 + note.type;
                        if (intChartGameType == 3) note.gameObject.layer += 4;
                        note.spriteRendererNote.color = noteColor[note.type];
                        note.gameObject.SetActive(true);

                        note.spriteRendererLength.gameObject.SetActive(true);
                        note.spriteRendererLength.gameObject.layer = 9 + note.type;
                        note.spriteRendererLength.transform.localPosition = Vector3.down * longNoteLength * floatNoteScrollMultiplier * speed * currentScrollSpeed / 2f;
                        note.spriteRendererLength.transform.localScale = new Vector3(
                            note.spriteRendererLength.transform.localScale.x,
                            longNoteLength * floatNoteScrollMultiplier * currentScrollSpeed * speed,
                            1f);
                        note.spriteRendererLength.color = noteColor[note.type];

                        notePos = new Vector3(note.position, (note.time - floatMusicBeat) * floatNoteScrollMultiplier * currentScrollSpeed * note.speed);
                        note.transform.position = notePos;
                    }

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
                if (time < floatMusicBeat + (floatNoteDistanceSpawn / floatNoteScrollMultiplier / currentScrollSpeed / speed))
                {
                    Game_Note note = SpawnNoteTap();
                    note.health = 1f;
                    note.type = int.Parse(noteInfo[0]);
                    if (intChartGameType == 0 && note.type > 0)
                    {
                        note.type = 0;
                    }
                    else if (intChartGameType == 1 && note.type > 1)
                    {
                        note.type %= 2;
                    }
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

                    Vector3 notePos = new Vector3(note.position, (note.time - floatMusicBeat) * floatNoteScrollMultiplier * currentScrollSpeed * note.speed);
                    note.transform.position = notePos;

                    float longNoteLength = float.Parse(noteInfo[4]);
                    if (longNoteLength > 0.01f)
                    {
                        note = SpawnNoteTap();
                        note.health = 1f;
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
                        note.spriteRendererLength.transform.localPosition = Vector3.down * longNoteLength * floatNoteScrollMultiplier * currentScrollSpeed * speed / 2f;
                        note.spriteRendererLength.transform.localScale = new Vector3(
                            note.spriteRendererLength.transform.localScale.x,
                            longNoteLength * floatNoteScrollMultiplier * currentScrollSpeed * speed,
                            1f);
                        note.spriteRendererLength.color = noteColor[note.type];

                        notePos = new Vector3(note.position, (note.time - floatMusicBeat) * floatNoteScrollMultiplier * currentScrollSpeed * note.speed);
                        note.transform.position = notePos;
                    }

                    chartData.listNoteTapInfo.RemoveAt(j);
                }
            }
            // Item
            /*
            if (intChartGameType == 3 &&
                floatMusicBeat > timeItemLastSpawn + floatNoteDodgeItemSpawnFrequency &&
                timeItemLastSpawn < chartData.gameplayLength - (floatNoteDodgeItemSpawnFrequency * 4f + Mathf.Epsilon))
            {
                timeItemLastSpawn += floatNoteDodgeItemSpawnFrequency;

                int iType = Mathf.FloorToInt(floatMusicPosition * 4f) % 4;
                float iPos = floatMusicPosition * 30f;

                Game_NoteItem item = SpawnNoteItem();
                switch (iType)
                {
                    default:
                    case 0:
                        item.transform.position = Vector3.down * 1.1f + Vector3.right * ((iPos % 2f) - 1f);
                        item.transform.rotation = Quaternion.Euler(Vector3.forward * 0f);
                        break;
                    case 1:
                        item.transform.position = Vector3.up * 1.1f + Vector3.left * ((iPos % 2f) - 1f);
                        item.transform.rotation = Quaternion.Euler(Vector3.forward * 180f);
                        break;
                    case 2:
                        item.transform.position = Vector3.left * 1.1f + Vector3.down * ((iPos % 2f) - 1f);
                        item.transform.rotation = Quaternion.Euler(Vector3.forward * 270f);
                        break;
                    case 3:
                        item.transform.position = Vector3.right * 1.1f + Vector3.up * ((iPos % 2f) - 1f);
                        item.transform.rotation = Quaternion.Euler(Vector3.forward * 90f);
                        break;
                }
                item.speed = chartCurrentTempo / chartData.songTempo;
                item.gameObject.SetActive(true);
            }
            */

            // Note positioning
            // Catch note
            foreach (Game_Note x in listNoteCatch)
            {
                if (x.gameObject.activeInHierarchy)
                {
                    Vector3 notePos = new Vector3(x.position, (x.time - floatMusicBeat) * floatNoteScrollMultiplier * currentScrollSpeed * x.speed);
                    if (Vector3.Distance(x.transform.position, notePos) < chartCurrentTempo / 24f)
                    {
                        x.transform.position = Vector3.Lerp(x.transform.position, notePos, Time.deltaTime * NOTE_LERP_RATE_MULTIPLIER * currentScrollSpeed * chartCurrentTempo / 600f);
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

                    // Check game mode
                    if (intChartGameType != 3)
                    {
                        // Note judgment
                        // Normal note or long note end - Go below pos 0 vertically
                        /*
                        if (floatMusicBeat >= x.time ||
                            (x.length > 0.01f &&
                            floatMusicBeat >= x.time - x.length &&
                            Mathf.Abs(x.transform.position.x - objectCatcher[x.type].transform.position.x) > floatDistAccuracyCatchFine[chartJudgeDifficulty])
                            )
                        */
                        if (floatMusicBeat >= x.time)
                        {
                            JudgeNote(x, objectCatcher[x.type], false);
                        }
                        // Long note length - Sway too far from the note's center long enough to get a "Miss"
                        else if (x.length > 0.01f && floatMusicBeat >= x.time - x.length)
                        {
                            if (Mathf.Abs(x.transform.position.x - objectCatcher[x.type].transform.position.x) > floatDistAccuracyCatchFine[chartJudgeDifficulty])
                            {
                                x.health -= 1f / floatNoteCatchLongHealthDropDuration * Time.deltaTime;
                                if (x.health < 0f)
                                {
                                    JudgeNote(x, objectCatcher[x.type], false, true);
                                }

                                Color y = noteColor[x.type];
                                y.r *= 0.4f;
                                y.g *= 0.4f;
                                y.b *= 0.4f;
                                x.spriteRendererLength.color = y;
                            }
                            else
                            {
                                x.health = 1f;
                                x.spriteRendererLength.color = noteColor[x.type];
                            }
                        }
                    }
                    else
                    {
                        // Bullet and item spawning for Note Dodge
                        if (floatMusicBeat >= x.time)
                        {
                            Vector3 bulletPos = Vector3.zero;
                            switch (x.type)
                            {
                                default: case 0:
                                    bulletPos = Vector3.down * 1.1f + Vector3.right * x.position;
                                    break;
                                case 1:
                                    bulletPos = Vector3.up * 1.1f + Vector3.left * x.position;
                                    break;
                                case 2:
                                    bulletPos = Vector3.left * 1.1f + Vector3.down * x.position;
                                    break;
                                case 3:
                                    bulletPos = Vector3.right * 1.1f + Vector3.up * x.position;
                                    break;
                            }

                            Game_NoteBullet bullet;
                            for (int i = 0; i < intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty]; i++)
                            {
                                bullet = SpawnNoteBullet();
                                bullet.transform.position = bulletPos;
                                bullet.transform.rotation = Quaternion.Euler(Vector3.forward * ((((360f * i / intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty])) + (floatMusicPosition * 90f)) % 360f));
                                bullet.speed = x.speed;
                                bullet.gameObject.SetActive(true);
                            }

                            /*
                            Game_NoteItem item = SpawnNoteItem();
                            item.transform.position = bulletPos;
                            switch (x.type)
                            {
                                default:
                                case 0:
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 0f);
                                    break;
                                case 1:
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 180f);
                                    break;
                                case 2:
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 270f);
                                    break;
                                case 3:
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 90f);
                                    break;
                            }
                            item.speed = x.speed;
                            item.gameObject.SetActive(true);
                            */

                            DespawnNote(x);
                        }
                        else if (floatMusicBeat >= x.time - x.length + (1f / floatNoteDodgeBulletLongNoteSpawnFrequency[chartJudgeDifficulty]))
                        {
                            x.length -= 1f / floatNoteDodgeBulletLongNoteSpawnFrequency[chartJudgeDifficulty];

                            Vector3 bulletPos = Vector3.zero;
                            switch (x.type)
                            {
                                default:
                                case 0:
                                    bulletPos = Vector3.down * 1.1f + Vector3.right * x.position;
                                    break;
                                case 1:
                                    bulletPos = Vector3.up * 1.1f + Vector3.left * x.position;
                                    break;
                                case 2:
                                    bulletPos = Vector3.left * 1.1f + Vector3.down * x.position;
                                    break;
                                case 3:
                                    bulletPos = Vector3.right * 1.1f + Vector3.up * x.position;
                                    break;
                            }
                            Quaternion bRot = Quaternion.LookRotation(Vector3.forward, objectMouseCursorDodger.transform.position - bulletPos);

                            Game_NoteBullet bullet;
                            bullet = SpawnNoteBullet();
                            bullet.transform.position = bulletPos;
                            bullet.transform.rotation = bRot;
                            bullet.speed = x.speed;
                            bullet.gameObject.SetActive(true);
                        }
                    }
                }
            }
            // Tap note
            foreach (Game_Note x in listNoteTap)
            {
                if (x.gameObject.activeInHierarchy)
                {
                    Vector3 notePos = new Vector3(x.position, (x.time - floatMusicBeat) * floatNoteScrollMultiplier * currentScrollSpeed * x.speed);
                    if (Vector3.Distance(x.transform.position, notePos) < chartCurrentTempo / 240f)
                    {
                        x.transform.position = Vector3.Lerp(x.transform.position, notePos, Time.deltaTime * NOTE_LERP_RATE_MULTIPLIER * currentScrollSpeed * chartCurrentTempo / 600f);
                    }
                    else
                    {
                        x.transform.position = notePos;
                    }

                    float dist = Mathf.Abs(floatMusicBeat - x.time) / chartCurrentTempo * 60f;

                    // Note flash
                    x.spriteRendererNoteHighlight.color = colorBeatFlash;
                    if (x.length > 0.01f)
                    {
                        x.spriteRendererLengthHighlight.color = colorBeatFlash;
                    }

                    // Check game mode
                    if (intChartGameType != 3)
                    {
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
                    else
                    {
                        // Bullet spawning for Note Dodge
                        if (floatMusicBeat >= x.time)
                        {
                            Game_NoteBullet bullet;
                            for (int i = 0; i < intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty]; i++)
                            {
                                bullet = SpawnNoteBullet();
                                switch (x.type)
                                {
                                    default:
                                    case 0:
                                        bullet.transform.position = Vector3.down * 1.1f + Vector3.right * ((2f * (1f * i / intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty])) - 1f) * 3f;
                                        bullet.transform.rotation = Quaternion.Euler(Vector3.forward * 0f);
                                        break;
                                    case 1:
                                        bullet.transform.position = Vector3.up * 1.1f + Vector3.left * ((2f * (1f * i / intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty])) - 1f) * 3f;
                                        bullet.transform.rotation = Quaternion.Euler(Vector3.forward * 180f);
                                        break;
                                    case 2:
                                        bullet.transform.position = Vector3.left * 1.1f + Vector3.down * ((2f * (1f * i / intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty])) - 1f) * 3f;
                                        bullet.transform.rotation = Quaternion.Euler(Vector3.forward * 270f);
                                        break;
                                    case 3:
                                        bullet.transform.position = Vector3.right * 1.1f + Vector3.up * ((2f * (1f * i / intNoteDodgeBulletRingSpawnQuantity[chartJudgeDifficulty])) - 1f) * 3f;
                                        bullet.transform.rotation = Quaternion.Euler(Vector3.forward * 90f);
                                        break;
                                }
                                bullet.speed = x.speed;

                                bullet.gameObject.SetActive(true);
                            }

                            /*
                            Game_NoteItem item = SpawnNoteItem();
                            switch (x.type)
                            {
                                default:
                                case 0:
                                    item.transform.position = Vector3.down * 1.1f + Vector3.right * ((floatMusicPosition % 2f) - 1f);
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 0f);
                                    break;
                                case 1:
                                    item.transform.position = Vector3.up * 1.1f + Vector3.left * ((floatMusicPosition % 2f) - 1f);
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 180f);
                                    break;
                                case 2:
                                    item.transform.position = Vector3.left * 1.1f + Vector3.down * ((floatMusicPosition % 2f) - 1f);
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 270f);
                                    break;
                                case 3:
                                    item.transform.position = Vector3.right * 1.1f + Vector3.up * ((floatMusicPosition % 2f) - 1f);
                                    item.transform.rotation = Quaternion.Euler(Vector3.forward * 90f);
                                    break;
                            }
                            item.speed = x.speed;
                            item.gameObject.SetActive(true);
                            */

                            DespawnNote(x);
                        }
                    }
                }
            }
            // Note Dodge-exclusive objects
            if (intChartGameType == 3)
            {
                // Bullets
                foreach (Game_NoteBullet x in listNoteBullet)
                {
                    if (x.gameObject.activeInHierarchy)
                    {
                        // Move them
                        x.transform.position += x.transform.up * x.speed * chartCurrentTempo / 180f * Time.deltaTime;

                        // Check if its too close to the dodger
                        if (Vector3.Distance(objectMouseCursorDodger.transform.position, x.transform.position) < floatNoteDodgeBulletHitboxRadius)
                        {
                            if (floatNoteDodgeInvincibilityPeriodCurrent < 0f)
                            {
                                floatNoteDodgeInvincibilityPeriodCurrent = floatNoteDodgeInvincibilityPeriod;
                                PlaySoundEffect(clipGameBulletHit);
                                intNoteDodgeHit++;
                                
                                // Remove nearby bullets
                                foreach (Game_NoteBullet y in listNoteBullet)
                                {
                                    if (Vector3.Distance(objectMouseCursorDodger.transform.position, y.transform.position) < 0.5f)
                                    {
                                        DespawnNote(y);
                                    }
                                }

                                if (PlayerSetting.setting.enableDisplayNoteJudgment)
                                {
                                    Game_AnimationJudgment anim = SpawnJudgeAnimation();
                                    anim.gameObject.SetActive(true);
                                    anim.transform.position = x.transform.position + Vector3.up;
                                    anim.gameObject.layer = 9;
                                    anim.spriteRendererJudgment.sprite = spriteJudgment[3];
                                    anim.animatorJudgment.Play("anim");

                                    ParticleSystem.MainModule animParticleModule = anim.particleSystemJudgment.main;
                                    animParticleModule.startColor = colorJudgmentParticle[3];
                                    anim.particleSystemJudgment.gameObject.layer = anim.gameObject.layer;
                                    anim.particleSystemJudgment.Play();
                                }
                            }
                            DespawnNote(x);
                        }

                        // Despawn it if it's out of bounds
                        if (x.transform.position.x > 1.5f ||
                            x.transform.position.x < -1.5f ||
                            x.transform.position.y > 1.5f ||
                            x.transform.position.y < -1.5f)
                        {
                            DespawnNote(x);
                        }
                    }
                }
                // Point Items
                foreach (Game_NoteItem x in listNoteItem)
                {
                    if (x.gameObject.activeInHierarchy)
                    {
                        // Move them
                        x.transform.position += x.transform.up * x.speed * chartCurrentTempo / 280f * Time.deltaTime;

                        // Check if its close to the dodger
                        if (Vector3.Distance(objectMouseCursorDodger.transform.position, x.transform.position) < floatNoteDodgeItemHitboxRadius)
                        {
                            PlaySoundEffect(clipGameNoteAssistTick);
                            playerAccuracyBest++;
                            DespawnNote(x);

                            // Item collect animation
                            if (PlayerSetting.setting.enableDisplayNoteJudgment)
                            {
                                Game_AnimationJudgment anim = SpawnJudgeAnimation();
                                anim.gameObject.SetActive(true);
                                anim.transform.position = x.transform.position + Vector3.up;
                                anim.gameObject.layer = 9;
                                anim.spriteRendererJudgment.sprite = spriteJudgment[0];
                                //anim.spriteRendererJudgment.sortingLayerID = animSortingLayerID;
                                anim.animatorJudgment.Play("anim");

                                ParticleSystem.MainModule animParticleModule = anim.particleSystemJudgment.main;
                                animParticleModule.startColor = colorJudgmentParticle[0];
                                anim.particleSystemJudgment.gameObject.layer = anim.gameObject.layer;
                                anim.particleSystemJudgment.Play();
                            }

                            // Animate full combo (combo = number of notes)
                            if (playerAccuracyBest == chartTotalNotes && animatorFullCombo != null)
                            {
                                // Perfect accuracy
                                if (intNoteDodgeHit == 0)
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
                        }

                        // Despawn it if it's out of bounds
                        if (x.transform.position.x > 1.5f ||
                            x.transform.position.x < -1.5f ||
                            x.transform.position.y > 1.5f ||
                            x.transform.position.y < -1.5f)
                        {
                            playerAccuracyMiss++;
                            DespawnNote(x);
                        }
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
                    float dist = Mathf.Abs(floatMusicBeat - lowestNote.time) / chartCurrentTempo * 60f;
                    if (dist < floatDistAccuracyTapMiss[chartJudgeDifficulty])
                    {
                        JudgeNote(lowestNote, objectCatcher[lowestNote.type % 4], true);
                    }
                }
            }

            /*
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
            */

            // Press [Escape] to pause the game
            //if (!boolAutoplay && Input.GetKeyDown(KeyCode.Escape))
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                // Check if the last pause is at least 3 seconds before
                if (boolAutoplay || floatMusicPosition > floatLastPaused + 3f)
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

            // Force end condition: Current negative accuracy is below player-set threshold
            if (intChartGameType != 3)
            {
                if (marathonItem == null)
                {
                    if (1f * currentAccuracyNegative / (4f * chartTotalNotes) >
                        1f - (0.01f * PlayerSetting.setting.intAccuracyThreshold))
                    {
                        isForcedEnd = true;
                    }
                }
                else
                {
                    if (1f * currentAccuracyNegative / (4f * chartTotalNotes) >
                        1f - (0.01f * marathonItem.itemAccuracyThreshold))
                    {
                        isForcedEnd = true;
                    }
                }
            }
            else
            {
                if (marathonItem == null)
                {
                    if (1f * currentAccuracyNegative / 1000000 >
                        1000000f - (10000f * PlayerSetting.setting.intAccuracyThreshold))
                    {
                        isForcedEnd = true;
                    }
                }
                else
                {
                    if (1f * currentAccuracyNegative / 1000000 >
                        1000000f - (10000f * marathonItem.itemAccuracyThreshold))
                    {
                        isForcedEnd = true;
                    }
                }
            }

            // Force end condition: Miss enough notes in a marathon
            if (marathonItem != null && marathonItem.itemNoteMissThreshold > 0)
            {
                if (playerAccuracyMiss + marathonAccuracyMiss + intNoteDodgeHit + marathonNoteDodgeHit >= marathonItem.itemNoteMissThreshold)
                {
                    isForcedEnd = true;
                }
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
                        if (intChartGameType != 3) playerAccuracyMiss++;
                        DespawnNote(x);
                    }
                }
                foreach (Game_Note x in listNoteTap)
                {
                    if (x.gameObject.activeSelf)
                    {
                        isForcedEnd = true;
                        if (intChartGameType != 3) playerAccuracyMiss++;
                        DespawnNote(x);
                    }
                }
                if (intChartGameType == 3)
                {
                    foreach (Game_NoteItem x in listNoteItem)
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

        textMeshRecordGhost.gameObject.SetActive(false);
        textMeshComboCurrent.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        yield return null;

        float finalAccuracy = 0f;
        if (intChartGameType != 3)
        {
            finalAccuracy = 1f * ((playerAccuracyBest * 4) + (playerAccuracyGreat * 2) + playerAccuracyFine) / (chartTotalNotes * 4) * Mathf.Pow(0.95f, Mathf.Clamp(intNoteDodgeHit, 0, intNoteDodgeHitMaximum));
        }
        else
        {
            //finalAccuracy = (1f * playerAccuracyBest / itemQuantityTotal) * Mathf.Pow(0.95f, 1f * Mathf.Clamp(intNoteDodgeHit, 0, intNoteDodgeHitMaximum));
            finalAccuracy = Mathf.Pow(0.95f, 1f * intNoteDodgeHit);
        }
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

        // Traditional game mode - single song
        if (marathonItem == null)
        {
            // If the game was being autoplayed, skip result screen.
            if (boolAutoplay)
            {
                ExitGameScene();
                yield break;
            }
            else if (intChartGameType == 3 && isForcedEnd)
            {
                /*
                int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", 0);
                songPlayCount++;
                PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", songPlayCount);
                */
                PlayerSetting.setting.intPlayerTotalPlayCountND++;
                PlayerPrefs.Save();

                ExitGameScene();
                yield break;
            }
            else if (!isForcedEnd)
            {
                switch (intChartGameType)
                {
                    case 0: PlayerSetting.setting.intPlayerTotalPlayCountLN++; break;
                    case 1: PlayerSetting.setting.intPlayerTotalPlayCountDB++; break;
                    case 2: PlayerSetting.setting.intPlayerTotalPlayCountQD++; break;
                    case 3: PlayerSetting.setting.intPlayerTotalPlayCountND++; break;
                }
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
                if (boolCustomSong)
                {
                    // Accuracy record
                    oldRecordAccuracy = PlayerPrefs.GetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy", 0f);
                    if (finalAccuracy > oldRecordAccuracy)
                    {
                        PlayerPrefs.SetFloat(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordaccuracy", finalAccuracy);
                    }

                    /*
                    // Mapchart play count increase
                    int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", 0);
                    songPlayCount++;
                    PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount", songPlayCount);
                    */

                    // Clear status
                    int clearStatus = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-status", 0);
                    if (!isForcedEnd)
                    {
                        if (playerAccuracyBest >= chartTotalNotes)
                        {
                            if (clearStatus < 3) clearStatus = 3;
                        }
                        else if (playerAccuracyMiss > 0)
                        {
                            if (clearStatus < 2) clearStatus = 2;
                        }
                        else
                        {
                            if (clearStatus < 1) clearStatus = 1;
                        }
                    }
                    PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-status", clearStatus);
                }
                else
                {
                    // Score based on accuracy, best combo, chart level, and game mode.
                    finalScore = Mathf.FloorToInt(
                        //(1f * playerComboBest / chartTotalNotes) * finalAccuracy *  // Base accuracy (with best combo)
                        finalAccuracy *                                             // Base accuracy
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

                    // Score record
                    int oldRecordScore = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordscore_official", 0);
                    if (finalScore > oldRecordScore)
                    {
                        PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-recordscore_official", oldRecordScore);
                    }

                    /*
                    // Mapchart play count increase
                    int songPlayCount = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount_official", 0);
                    songPlayCount++;
                    PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-playcount_official", songPlayCount);
                    */

                    // Clear status
                    int clearStatus = PlayerPrefs.GetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-status_official", 0);
                    if (!isForcedEnd)
                    {
                        if (playerAccuracyBest >= chartTotalNotes)
                        {
                            if (clearStatus < 3) clearStatus = 3;
                        }
                        else if (playerAccuracyMiss > 0)
                        {
                            if (clearStatus < 2) clearStatus = 2;
                        }
                        else
                        {
                            if (clearStatus < 1) clearStatus = 1;
                        }
                    }
                    PlayerPrefs.SetInt(stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString() + "-status_official", clearStatus);
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
            if (!isForcedEnd &&
                ((intChartGameType != 3 && playerAccuracyFine == 0 && playerAccuracyGreat == 0 && playerAccuracyMiss == 0) ||
                (intChartGameType == 3 && intNoteDodgeHit == 0)))
            {
                textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYPERFECT", "PERFECT RHYTHM");
                textResultHeader.color = colorResultHeaderPerfect;
                PlaySoundEffect(clipGameEndPerfect);
                imageSongProgressGauge.fillAmount = 1f;
            }
            else if (!isForcedEnd && finalAccuracy >= 0.01f * PlayerSetting.setting.intAccuracyThreshold)
            {
                textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYCLEAR", "COMPLETE RHYTHM");
                textResultHeader.color = colorResultHeaderPass;
                PlaySoundEffect(clipGameEndPass);
                imageSongProgressGauge.fillAmount = 1f;
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
            if (intChartGameType != 3)
            {
                StartCoroutine(TextIntGradualIncrease(textResultBestCombo, playerComboBest, 0.8f));
                for (float f = 0; f < 0.5f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeBest, playerAccuracyBest, 0.6f));
                for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeGreat, playerAccuracyGreat, 0.5f));
                for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeFine, playerAccuracyFine, 0.4f));
                for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeMiss, playerAccuracyMiss, 0.3f));
            }
            else
            {
                textResultBestCombo.text = "0";
                StartCoroutine(TextIntGradualIncrease(textResultJudgeBest, intNoteDodgeHit, 1.0f));
                for (float f = 0; f < 1.4f; f += Time.deltaTime * animatorResults.speed) yield return null;
            }

            for (float f = 0; f < 1.2f; f += Time.deltaTime * animatorResults.speed) yield return null;
            if (!isScoringDisabled)
            {
                if (finalAccuracy > oldRecordAccuracy)
                {
                    animatorNewRecord.gameObject.SetActive(true);
                    animatorNewRecord.Play("clip");
                }

                for (float f = 0; f < 1f; f += Time.deltaTime * 1.5f)
                {
                    textResultOther.text = Translator.GetStringTranslation("GAME_RESULTSCOREADD", "Score:") + " " + Mathf.FloorToInt(f * finalScore).ToString();
                    yield return null;
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
        // Marathon mode - play charts consecutively
        else
        {
            intMarathonItem++;

            marathonAccuracyBest += playerAccuracyBest;
            marathonAccuracyGreat += playerAccuracyGreat;
            marathonAccuracyFine += playerAccuracyFine;
            marathonAccuracyMiss += playerAccuracyMiss;
            marathonNoteDodgeHit += intNoteDodgeHit;
            marathonComboCurrent = playerComboCurrent;
            marathonComboBest = playerComboBest;
            marathonLength += chartData.gameplayLength;

            // Next chart in list
            if (intMarathonItem < marathonItem.itemChartList.Length && !isForcedEnd)
            {
                // Reload scene to play next song
                LoadScene("Game");
            }
            // End of marathon
            else
            {
                // If it was a tutorial, go to title scene immediately
                if (boolIsTutorial)
                {
                    ExitGameScene();
                    yield break;
                }

                // If forced end with incomplete songs
                if (intMarathonItem < marathonItem.itemChartList.Length)
                {
                    // Add each incomplete chart's notes to the MISS counter (for all modes except Note Dodge)
                    for (int i = intMarathonItem; i < marathonItem.itemChartList.Length; i++)
                    {
                        string[] x = marathonItem.itemChartList[intMarathonItem].Split('|');
                        stringSongFileName = x[0];
                        intChartGameType = int.Parse(x[1]);
                        intChartGameChart = int.Parse(x[2]);

                        if (intChartGameType != 3)
                        {
                            string chartFileName = "Songs/" + stringSongFileName + "/" + stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString();
                            TextAsset info = Resources.Load(chartFileName) as TextAsset;
                            string input = info.text;

                            chartData = ScriptableObject.CreateInstance<ChartData>();
                            JsonUtility.FromJsonOverwrite(input, chartData);

                            foreach (string s in chartData.listNoteCatchInfo)
                            {
                                marathonAccuracyMiss++;

                                // If it is a long note, it counts as two notes.
                                x = s.Split('|');
                                if (float.Parse(x[4]) > 0.01f)
                                {
                                    marathonAccuracyMiss++;
                                }
                            }
                            marathonAccuracyMiss += chartData.listNoteTapInfo.Count;
                        }
                    }
                }

                textScoreDisabled.gameObject.SetActive(false);

                int marathonTotalNotes = 4 * (marathonAccuracyBest + marathonAccuracyGreat + marathonAccuracyFine + marathonAccuracyMiss);
                float marathonAccuracy = 1f * ((marathonAccuracyBest * 4) + (marathonAccuracyGreat * 2) + marathonAccuracyFine) / marathonTotalNotes;
                if (marathonTotalNotes == 0) marathonAccuracy = 1f;
                marathonAccuracy *= Mathf.Pow(0.95f, 1f * marathonNoteDodgeHit / marathonItem.itemChartList.Length);
                int finalScore = Mathf.FloorToInt(
                    marathonAccuracy *                                          // Base accuracy
                    Mathf.Pow(4 + marathonItem.itemLevel, 2f) *                 // Marathon level
                    10 * marathonItem.itemScoreMultiplier *                     // Multiplier
                    marathonLength / 60f                                        // Chart gameplay length
                    );
                float oldRecordAccuracy = PlayerPrefs.GetFloat("marathon-" + marathonItemID.ToString() + "-accuracy", 0f);

                if (!isForcedEnd)
                {
                    if (marathonAccuracy > oldRecordAccuracy)
                    {
                        PlayerPrefs.SetFloat("marathon-" + marathonItemID.ToString() + "-accuracy", marathonAccuracy);
                        PlayerPrefs.Save();
                    }

                    textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYMARATHONCOMPLETE", "MARATHON COMPLETE");
                    textResultHeader.color = colorResultHeaderPass;
                    PlaySoundEffect(clipGameEndPass);
                    imageSongProgressGauge.fillAmount = 1f;
                    PlayerSetting.setting.ScoreAdd(finalScore);
                }
                else
                {
                    textResultHeader.text = Translator.GetStringTranslation("GAME_RESULTPLAYMARATHONFAILURE", "MARATHON INCOMPLETE");
                    textResultHeader.color = colorResultHeaderFail;
                    PlaySoundEffect(clipGameEndFail);
                    finalScore = 0;
                }
                PlayerSetting.setting.Save();

                // Show result screen
                animatorResults.gameObject.SetActive(true);
                animatorResults.Play("clip");

                // Click a mouse button or hold space to speed up animation
                StartCoroutine(AnimatorResultsSpeedUp());

                for (float f = 0; f < 0.5f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextFloatGradualIncrease(textResultAccuracy, marathonAccuracy * 100f, 0.8f));

                StartCoroutine(TextIntGradualIncrease(textResultBestCombo, marathonComboBest, 0.8f));
                for (float f = 0; f < 0.5f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeBest, marathonAccuracyBest, 0.6f));
                for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeGreat, marathonAccuracyGreat, 0.5f));
                for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeFine, marathonAccuracyFine, 0.4f));
                for (float f = 0; f < 0.3f; f += Time.deltaTime * animatorResults.speed) yield return null;
                StartCoroutine(TextIntGradualIncrease(textResultJudgeMiss, marathonAccuracyMiss + marathonNoteDodgeHit, 0.3f));

                for (float f = 0; f < 1.2f; f += Time.deltaTime * animatorResults.speed) yield return null;

                if (!isForcedEnd && marathonAccuracy > oldRecordAccuracy)
                {
                    animatorNewRecord.gameObject.SetActive(true);
                    animatorNewRecord.Play("clip");
                }

                for (float f = 0; f < 1f; f += Time.deltaTime * 1.5f)
                {
                    textResultOther.text = Translator.GetStringTranslation("GAME_RESULTSCOREADD", "Score:") + " " + Mathf.FloorToInt(f * finalScore).ToString();
                    yield return null;
                }
                textResultOther.text = Translator.GetStringTranslation("GAME_RESULTSCOREADD", "Score:") + " " + finalScore.ToString();
            }
        }
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
