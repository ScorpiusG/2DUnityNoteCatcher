using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Game_Control : MonoBehaviour
{
    public static string stringSongFileName = "";
    public static int intChartGameType = 0;
    public static int intChartGameChart = 0;

    public static bool boolAutoplay = false;

    public GameObject[] objectGroupType;
    public GameObject objectMouseCrosshair;
    public GameObject[] objectCatcher;

    public Game_Note notePrefab;
    private List<Game_Note> listNote;
    public Color[] noteColor = { Color.blue, Color.red, Color.green, Color.yellow };
    public float floatNoteDistanceSpawn = 3f;

    public AudioSource audioSourceMusic;

    public Text textSongAndArtistName;
    public Text textSongProgressTime;
    public Image imageSongProgressGauge;
    public Image imageAccuracyGauge;
    public Image imageAccuracyNegativeGauge;
    public Text textAccuracy;
    public Image imageAccuracyTolerance;

    private ChartData chartData;
    private int chartTotalNotes = 0;
    private int chartJudgeDifficulty = 0;
    private float floatMusicPosition = 0f;
    private float floatMusicBeat = 0f;
    private float movementHoriAlpha = 0f;
    private float movementVertAlpha = 0f;

    public float[] floatDistAccuracyBest = { 0.16f, 0.14f, 0.125f, 0.11f, 0.1f };
    public float[] floatDistAccuracyGreat = { 0.19f, 0.165f, 0.15f, 0.125f, 0.113f };
    public float[] floatDistAccuracyFine = { 0.21f, 0.183f, 0.164f, 0.138f, 0.122f };

    private bool isForcedFailure = false;
    private float floatTimeEscapeHeld = 0f;
    private int playerAccuracyBest = 0;
    private int playerAccuracyGreat = 0;
    private int playerAccuracyFine = 0;
    private int playerAccuracyMiss = 0;

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
                // BEST
                if (dist < floatDistAccuracyBest[chartJudgeDifficulty] || boolAutoplay)
                {
                    playerAccuracyBest++;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (BEST)");
#endif
                }
                // GREAT
                else if (dist < floatDistAccuracyBest[chartJudgeDifficulty])
                {
                    playerAccuracyGreat++;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (GREAT)");
#endif
                }
                // FINE
                else if (dist < floatDistAccuracyBest[chartJudgeDifficulty])
                {
                    playerAccuracyFine++;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (FINE)");
#endif
                }
                // MISS
                else
                {
                    playerAccuracyMiss++;
#if UNITY_EDITOR
                    Debug.Log("Note judgment - distance: " + dist + " (MISS)");
#endif
                }
                break;
        }

        DespawnNote(note);
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
        imageSongProgressGauge.fillAmount = 0f;
        imageAccuracyTolerance.transform.localPosition = Vector3.right * 928f * 0.01f * PlayerSetting.setting.intAccuracyTolerance;
        foreach (GameObject x in objectCatcher)
        {
            x.transform.localPosition = Vector3.zero;
        }

        // Read chart from the text file
        string input = "";
        string chartFileName = stringSongFileName + "-" + intChartGameType.ToString() + "-" + intChartGameChart.ToString();
        string path = Directory.GetCurrentDirectory() + "/Songs/" + stringSongFileName + "/" + chartFileName + ".txt";
        if (!File.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: The chart file does not exist! Path: " + path);
#endif
            isForcedFailure = true;
        }
        StreamReader reader = new StreamReader(path);
        input = reader.ReadToEnd();
        reader.Close();
#if UNITY_EDITOR
        Debug.Log(input);
#endif
        chartData = ScriptableObject.CreateInstance<ChartData>();
        JsonUtility.FromJsonOverwrite(input, chartData);

        //FileStream fileMusic = File.Open(Directory.GetCurrentDirectory() + "Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg", FileMode.Open);
        //audioSourceMusic.clip = (AudioClip)fileMusic as AudioClip;

        textSongAndArtistName.text = chartData.songArtist + " - " + chartData.songName;
        chartTotalNotes = chartData.listNoteInfo.Count;
        chartJudgeDifficulty = chartData.chartJudge;

        StartCoroutine("GameLoop");
    }
	
	private IEnumerator GameLoop ()
    {
        // Load audio file
        WWW www = new WWW("file://" + Directory.GetCurrentDirectory() + "/Songs/" + stringSongFileName + "/" + stringSongFileName + ".ogg");
        while (!www.isDone)
        {
            yield return null;
        }
        AudioClip myClip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);

        yield return new WaitForSeconds(0.5f);
        // Play music and begin the game
        audioSourceMusic.Play();
#if UNITY_EDITOR
        Debug.Log("Playing song. The game begins.");
#endif
        
        while (floatMusicPosition < chartData.songLength && !isForcedFailure)
        {
            // Time update
            floatMusicPosition = audioSourceMusic.time;
            floatMusicBeat = 60f * floatMusicPosition / chartData.songTempo;

            // Normal play
            if (!boolAutoplay)
            {
                // Mouse movement
                movementHoriAlpha = Input.GetAxisRaw("MouseX");
                movementVertAlpha = Input.GetAxisRaw("MouseY");

                // Crosshair position
                Vector3 mouseCursorPos = objectMouseCrosshair.transform.position;
                mouseCursorPos.x = Mathf.Clamp(mouseCursorPos.x + movementHoriAlpha * PlayerSetting.setting.floatMouseSensitivity, -1f, 1f);
                mouseCursorPos.y = Mathf.Clamp(mouseCursorPos.y + movementVertAlpha * PlayerSetting.setting.floatMouseSensitivity, -1f, 1f);
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
            }
            // Automatic play (always perfect)
            else
            {

            }

            // Text and gauge update
            textSongProgressTime.text =
                Mathf.Floor(floatMusicPosition / 60f).ToString() + ":" + Mathf.Floor(floatMusicPosition % 60f).ToString("00") + " / " +
                Mathf.Floor(chartData.songLength / 60f).ToString() + ":" + Mathf.Floor(chartData.songLength % 60f).ToString("00");
            imageSongProgressGauge.fillAmount = floatMusicPosition / chartData.songLength;
            int currentAccuracy = (playerAccuracyBest * 4) + (playerAccuracyGreat * 3) + (playerAccuracyFine * 2);
            int currentAccuracyNegative = playerAccuracyGreat + (playerAccuracyFine * 2) + (playerAccuracyMiss * 4);
            imageAccuracyGauge.fillAmount = 1f * currentAccuracy / chartTotalNotes;
            imageAccuracyNegativeGauge.fillAmount = 1f * currentAccuracyNegative / chartTotalNotes;
            textAccuracy.text =
                "B" + playerAccuracyBest.ToString() + "\n" +
                "G" + playerAccuracyGreat.ToString() + "\n" +
                "F" + playerAccuracyFine.ToString() + "\n" +
                "M" + playerAccuracyMiss.ToString();

            // Note spawning
            foreach (string s in chartData.listNoteInfo)
            {
                string[] noteInfo = s.Split('|');
                float time = float.Parse(noteInfo[2]);
                // Spawn note if position of note < current beat pos / FOV (scroll speed)
                if (time < floatMusicBeat + (floatNoteDistanceSpawn / (0.01f * PlayerSetting.setting.intScrollSpeed)))
                {
                    Game_Note note = SpawnNote();
                    note.type = int.Parse(noteInfo[0]);
                    note.size = int.Parse(noteInfo[1]);
                    note.time = time;
                    note.position = float.Parse(noteInfo[3]);
                    note.length = float.Parse(noteInfo[4]);
                    note.other = new List<string>();
                    if (noteInfo.Length > 5)
                    {
                        for (int i = 5; i < noteInfo.Length; i++)
                        {
                            note.other.Add(noteInfo[i]);
                        }
                    }
                    note.gameObject.SetActive(true);
                }
            }

            // Note positioning
            foreach (Game_Note x in listNote)
            {
                if (x.gameObject.activeSelf)
                {
                    x.transform.position = new Vector3(x.position, x.time - floatMusicBeat);

                    // Note judgment
                    // Normal note or long note end - Go below pos 0 vertically
                    // Long note length - Sway too far from the note's center enough to get a "Miss"
                    if (x.transform.position.y <= 0f || 
                        (x.transform.position.y - (x.length * 0.01f * PlayerSetting.setting.intScrollSpeed) < 0f &&
                        Mathf.Abs(x.transform.position.x - objectCatcher[x.type].transform.position.x) > floatDistAccuracyFine[chartJudgeDifficulty])
                        )
                    {
                        JudgeNote(x, objectCatcher[x.type]);
                    }
                }
            }

            // Player force fail
            if (Input.GetKey(KeyCode.Escape))
            {
                floatTimeEscapeHeld += Time.deltaTime;
            }
            else
            {
                floatTimeEscapeHeld = 0f;
            }

            // Fail conditions - at least one is true:
            // - Current negative accuracy is below tolerance
            // - [Escape] key was held for over two seconds
            if (1f * currentAccuracyNegative / chartTotalNotes > 0.01f * PlayerSetting.setting.intAccuracyTolerance ||
                floatTimeEscapeHeld > 2f)
            {
                isForcedFailure = true;
            }

            yield return null;
        }
        imageSongProgressGauge.fillAmount = 1f;

#if UNITY_EDITOR
        Debug.Log("Song finished. Display results.");
#endif
        // Revert settings (cursor, v-sync, etc.)
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;
        yield return null;

        // TODO: Show result screen
    }
}
