﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Creator_Control : MonoBehaviour
{
    public static Creator_Control control;
    private const int INT_CHART_VERSION = 0;

    public GameObject objectCentralPivot;
    public Camera cameraMain;
    public Canvas canvasCreatorSetting;

    public float holdDelay = 0.3f;
    private float holdDelayCurrent = 0f;

    public Slider sliderZoom;
    public float cameraSizeMin = 3f;
    public float cameraSizeMax = 10f;

    public InputField textFileName;
    public Text textFileGameType;
    public InputField textFileChart;
    public InputField textSongName;
    public InputField textSongArtist;
    public InputField textChartDeveloper;
    public InputField textChartDescription;
    public InputField textChartOffset;
    public InputField textSongTempo;
    public InputField textSongLength;
    public Text textChartJudge;
    private int intChartJudge = 0;

    public Transform transformItemFolderParent;
    public Creator_ItemFolder itemFolderTemplate;
    private List<Creator_ItemFolder> listItemFolder = new List<Creator_ItemFolder>();

    public InputField inputTempoChangeAdd;
    public InputField[] inputTempoChangeEdit;
    public int intTempoChangeEdit = 0;
    public List<Vector3> listTempoChange = new List<Vector3>();
    public Transform transformItemTempoChangeParent;
    public Creator_ItemTempoChange itemTempoChangeTemplate;
    private List<Creator_ItemTempoChange> listItemTempoChange = new List<Creator_ItemTempoChange>();

    public Text textChartLevel;
    public Text textTimeCurrentMeasure;
    public Text textTimeCurrentLength;
    public Text textBeatSnapDivisor;
    public Text textHoriPosSnapDivisor;
    public Text textBeatsPerMeasure;

    public Text textChartGameType;
    public Text textNotePlacementType;
    public Image[] imageNotePlacementHighlight;
    public Image[] imageNotePlacementInvalidDarken;
    public InputField textNoteLength;
    public Toggle toggleNoteLengthEnable;
    public InputField textNoteSpeed;
    public InputField textNoteHitsound;
    public Toggle toggleNoteHitsoundEnable;
    public Text textNoteOther;
    private List<string> listStringNoteOther = new List<string>();

    public Text textMouseScrollSetting;
    public Text textSongPreviewLength;
    public Slider sliderSongPreviewLength;
    public Text textSongPreviewFade;
    public Slider sliderSongPreviewFade;

    public ChartData chartData;

    public GameObject objectSelectedNoteHighlight;
    private List<Creator_Note> listNoteCatchPool = new List<Creator_Note>();
    public Creator_Note objectNoteCatchPrefab;
    private List<Creator_Note> listNoteTapPool = new List<Creator_Note>();
    public Creator_Note objectNoteTapPrefab;
    public Color[] colorNote = { Color.blue, Color.red, Color.green, Color.yellow };

    private List<Creator_SpecialEffect> listSpecialEffectPool = new List<Creator_SpecialEffect>();
    public Creator_SpecialEffect objectSpecialEffectPrefab;
    public Sprite[] spriteSpecialEffect;

    private List<GameObject> listObjectBeatSnapDivisorGuide = new List<GameObject>();
    public GameObject objectBeatSnapDivisorGuidePrefab; 
    private List<GameObject> listObjectHoriPosSnapDivisorGuide = new List<GameObject>();
    public GameObject objectHoriPosSnapDivisorGuidePrefab;

    public AudioSource mAudioSource;
    public AudioClip clipSelect;
    public AudioClip clipTick;
    public AudioClip clipCancel;
    public AudioClip clipNoteCreate;
    public AudioClip clipNoteDelete;

    public string stringSceneNameTitle = "Title";

    private int intMouseScrollSetting = 0;
    public string[] stringMouseScrollSetting = { "Move Chart", "Zoom Chart", "Change Note Type"};
    private int intChartGameType = 0;
    public string[] stringChartGameType = { "Linear", "Double", "Quad", "Powerful" };
    private int intNotePlacementType = 0;
    public string[] stringNotePlacementType = { "Blue (Btm)", "Red (Top)", "Green (Lt)", "Yellw (Dn)" };
    private int intBeatSnapDivisor = 0;
    public int[] intBeatSnapDivisorValue = { 2, 3, 4, 6, 8 };
    private int intHoriPosSnapDivisor = 0;
    public int[] intHoriPosSnapDivisorValue = { 2, 3, 4, 6, 8 };
    [HideInInspector] public float floatCursorPosition = 0;
    private int intBeatsPerMeasure = 4;
    private int intChartLevel = 0;
    private float floatGameplayLength = 0f;
    private Creator_Note objectNoteSelected = null;
    private bool boolEditNoteLength = false;
    [HideInInspector] public bool boolFullPreviewOngoing = false;

    private bool fixedUpdateCheckOtherFrame = false;
    private bool isShortcutKeyDisabled = false;
    private bool isNotificationShownGameTypeChangeDisable = false;

    void Awake()
    {
        control = this;
    }

    void Start()
    {
        chartData = ScriptableObject.CreateInstance(typeof(ChartData)) as ChartData;
        canvasCreatorSetting.gameObject.SetActive(false);
        objectSelectedNoteHighlight.SetActive(false);

        textFileName.text = PlayerPrefs.GetString("creator_textFileName", "");
        intChartGameType = PlayerPrefs.GetInt("creator_intChartGameType", 0);
        textFileChart.text = PlayerPrefs.GetString("creator_textFileChart", "0");
        intMouseScrollSetting = PlayerPrefs.GetInt("creator_intMouseScrollSetting", 0);
        intBeatSnapDivisor = PlayerPrefs.GetInt("creator_intBeatSnapDivisor", 0);
        intHoriPosSnapDivisor = PlayerPrefs.GetInt("creator_intHoriPosSnapDivisor", 2);
        sliderZoom.value = PlayerPrefs.GetFloat("creator_sliderZoom", 0.5f);
        sliderSongPreviewLength.value = PlayerPrefs.GetFloat("creator_floatSongPreviewLength", 8f);
        sliderSongPreviewFade.value = PlayerPrefs.GetFloat("creator_floatSongPreviewFade", 3f);

        textChartJudge.text = "0";

        RefreshGridSnapDivisorGuide();
        CalculateChartLevel();
        TempoChangeRefreshList();
    }

    /// <summary>
    /// Write chart information into a text file.
    /// </summary>
    public void SaveChart()
    {
        PlaySound(clipSelect);
        CalculateChartLevel();
        SaveCreatorSettings();

        Creator_Note[] listNoteC = FindObjectsOfType<Creator_Note>();
        int noteCount = 0;
        foreach (Creator_Note x in listNoteC)
        {
            if (x.gameObject.activeInHierarchy)
            {
                noteCount++;
            }
            if (noteCount > 1)
            {
                break;
            }
        }
        if (noteCount < 1)
        {
            Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORSAVECHARTEMPTY", "Save cancelled, the chart does not have at least one note."), Color.white);
            return;
        }
        if (textFileChart.text.Length < 1)
        {
            Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORLOADCHARTNONEXISTENT", "Cancelled. The file name field is empty.\nPlease input the name to proceed."), Color.white);
            return;
        }

        chartData.listNoteCatchInfo = new List<string>();
        chartData.listNoteTapInfo = new List<string>();
        chartData.listSpecialEffectInfo = new List<string>();

        chartData.listTempoChanges = listTempoChange;

        chartData.chartVersion = INT_CHART_VERSION;
        chartData.songName = textSongName.text;
        chartData.songArtist = textSongArtist.text;
        chartData.chartDeveloper = textChartDeveloper.text;
        chartData.chartDescription = textChartDescription.text;
        chartData.chartLevel = intChartLevel;
        chartData.songLength = 0;
        chartData.gameplayLength = floatGameplayLength;
        float.TryParse(textSongLength.text, out chartData.songLength);
        chartData.songTempo = 60f;
        float.TryParse(textSongTempo.text, out chartData.songTempo);
        chartData.chartGameType = intChartGameType;
        chartData.chartJudge = intChartJudge;
        //chartData.chartJudge = int.Parse(textChartJudge.text);
        //int.TryParse(textChartJudge.text, out chartData.chartJudge);
        chartData.chartOffset = 0;
        int.TryParse(textChartOffset.text, out chartData.chartOffset);

        chartData.gameplayLength *= chartData.songTempo / 60f;

        //ChartData.NoteInfo newNote = new ChartData.NoteInfo();
        ChartData.NoteInfo newNote = ScriptableObject.CreateInstance(typeof(ChartData.NoteInfo)) as ChartData.NoteInfo;
        foreach (Creator_Note x in listNoteCatchPool)
        {
            if (!x.gameObject.activeInHierarchy)
            {
                continue;
            }

            newNote.time = x.transform.position.y;
            newNote.position = x.transform.position.x;
            newNote.type = x.type;
            newNote.size = x.size;
            if (x.length > 0.01f)
            {
                newNote.length = x.length;
            }
            else
            {
                newNote.length = 0;
            }
            newNote.speed = x.speed;
            newNote.other = x.other;

            // Format: <type> | <size> | <time> | <hori-position> | <length (long note)> | <speed multiplier>
            string stringNote = newNote.type.ToString() + "|" +
                newNote.size.ToString() + "|" +
                newNote.time.ToString() + "|" +
                newNote.position.ToString() + "|" +
                newNote.length.ToString() + "|" +
                newNote.speed.ToString();
            foreach (string s in newNote.other)
            {
                stringNote += "|" + s;
            }
            chartData.listNoteCatchInfo.Add(stringNote);

            //if (newNote.time + x.length > chartData.songLength + 4f)
            //{
            //    chartData.songLength = newNote.time + 4f + x.length;
            //    textSongLength.text = (newNote.time + 4f + x.length).ToString();
            //}
        }
        foreach (Creator_Note x in listNoteTapPool)
        {
            if (!x.gameObject.activeInHierarchy)
            {
                continue;
            }

            newNote.time = x.transform.position.y;
            newNote.position = x.transform.position.x;
            newNote.type = x.type;
            newNote.size = x.size;
            if (x.length > 0.01f)
            {
                newNote.length = x.length;
            }
            else
            {
                newNote.length = 0;
            }
            newNote.speed = x.speed;
            newNote.other = x.other;

            // Format: <type> | <size> | <time> | <hori-position> | <length (long note)> | <speed>
            string stringNote = newNote.type.ToString() + "|" +
                newNote.size.ToString() + "|" +
                newNote.time.ToString() + "|" +
                newNote.position.ToString() + "|" +
                newNote.length.ToString() + "|" +
                newNote.speed.ToString();
            foreach (string s in newNote.other)
            {
                stringNote += "|" + s;
            }
            chartData.listNoteTapInfo.Add(stringNote);

            //if (newNote.time + x.length > chartData.songLength + 4f)
            //{
            //    chartData.songLength = newNote.time + 4f + x.length;
            //    textSongLength.text = (newNote.time + 4f + x.length).ToString();
            //}
        }

        chartData.beatsPerMeasure = intBeatsPerMeasure;

        string output = JsonUtility.ToJson(chartData);
#if UNITY_EDITOR
        Debug.Log(output);
#endif

        // Output to file
        if (!Directory.Exists("Songs"))
        {
            Directory.CreateDirectory("Songs");
        }
        if (!Directory.Exists("Songs/" + textFileName.text))
        {
            Directory.CreateDirectory("Songs/" + textFileName.text);
        }
        string path = "Songs/" + textFileName.text + "/" + textFileName.text + "-" + intChartGameType.ToString() + "-" + textFileChart.text + ".txt";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(output);
        writer.Close();
        Notification.Display(Translator.GetStringTranslation("CREATOR_SAVESUCCESS", "Save successful. Your chart is saved here:") + "\n" + path, Color.white);

        ShortcutKeysEnable();
    }

    /// <summary>
    /// Load chart information from an external text file.
    /// </summary>
    public void LoadChart()
    {
        PlaySound(clipSelect);
        string input = textFileName.text + "-" + intChartGameType.ToString() + "-" + textFileChart.text;

        if (textFileChart.text.Length < 1)
        {
            Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORLOADCHARTNONEXISTENT", "Cancelled. The file name field is empty.\nPlease input the name to proceed."), Color.white);
            return;
        }

        ClearChart();

        // Load file
        string path = "Songs/" + textFileName.text + "/" + input + ".txt";
        if (!File.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: The chart file does not exist! Path: " + path);
#endif
            Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORLOADCHARTNONEXISTENT", "The following chart file does not exist:") + "\n" + path, Color.white);
            return;
        }
        StreamReader reader = new StreamReader(path);
        input = reader.ReadToEnd();
        reader.Close();
#if UNITY_EDITOR
        Debug.Log(input);
#endif

        // Use file data
        JsonUtility.FromJsonOverwrite(input, chartData);

        if (chartData.chartVersion > INT_CHART_VERSION)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Chart's version is newer than the current supported version. " + chartData.chartVersion.ToString() + " > " + INT_CHART_VERSION.ToString());
#endif
            Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORLOADCHARTNEWVERSION1", "Failed to load the chart. The chart's version is|somehow newer than the currently supported version."), new Color(1f, 0.2f, 0.2f));
            Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORLOADCHARTNEWVERSION2", "Please download the latest version of the game|to be able to load this chart in the editor."), Color.white);
            return;
        }

        textSongName.text = chartData.songName;
        textSongArtist.text = chartData.songArtist;
        textChartDeveloper.text = chartData.chartDeveloper;
        textChartDescription.text = chartData.chartDescription;
        textSongLength.text = chartData.songLength.ToString();
        textSongTempo.text = chartData.songTempo.ToString("f2");
        listTempoChange = chartData.listTempoChanges;
        intChartJudge = chartData.chartJudge;
        //intChartGameType = chartData.chartGameType;
        textChartOffset.text = chartData.chartOffset.ToString();

        ChartData.NoteInfo newNote = ScriptableObject.CreateInstance(typeof(ChartData.NoteInfo)) as ChartData.NoteInfo;
        foreach (string x in chartData.listNoteCatchInfo)
        {
            newNote.other = new List<string>();
            string[] y = x.Split('|');
            newNote.type = int.Parse(y[0]);
            newNote.size = int.Parse(y[1]);
            newNote.time = float.Parse(y[2]);
            newNote.position = float.Parse(y[3]);
            newNote.length = float.Parse(y[4]);
            newNote.speed = float.Parse(y[5]);
            if (y.Length > 6)
            {
                for (int i = 6; i < y.Length; i++)
                {
                    newNote.other.Add(y[i]);
                }
            }
            CreateNoteCatch(newNote);
        }
        foreach (string x in chartData.listNoteTapInfo)
        {
            newNote.other = new List<string>();
            string[] y = x.Split('|');
            newNote.type = int.Parse(y[0]);
            newNote.size = int.Parse(y[1]);
            newNote.time = float.Parse(y[2]);
            newNote.position = float.Parse(y[3]);
            newNote.length = float.Parse(y[4]);
            newNote.speed = float.Parse(y[5]);
            if (y.Length > 6)
            {
                for (int i = 6; i < y.Length; i++)
                {
                    newNote.other.Add(y[i]);
                }
            }
            CreateNoteTap(newNote);
        }

        intBeatsPerMeasure = chartData.beatsPerMeasure;

        CalculateChartLevel();
        ShortcutKeysEnable();
        TempoChangeRefreshList();

        Notification.Display(Translator.GetStringTranslation("CREATOR_LOADSUCCESS", "Chart loaded successfully."), Color.white);
    }

    /// <summary>
    /// Completely wipes chart to start over from scratch.
    /// </summary>
    public void ClearChart()
    {
        floatCursorPosition = 0;

        //textFileName.text = "";
        textSongName.text = "";
        textSongArtist.text = "";
        textChartDeveloper.text = "";
        textChartDescription.text = "";
        textSongTempo.text = "60.00";
        textSongLength.text = "10";
        textChartJudge.text = "0";
        textChartOffset.text = "0";

        Creator_Note[] listNoteC = FindObjectsOfType<Creator_Note>();
        foreach (Creator_Note x in listNoteC)
        {
            DeleteNote(x);
        }

        listTempoChange.Clear();

        CalculateChartLevel();
        Creator_SongPreview.mSongPreview.ClearClip();
        TempoChangeRefreshList();
    }
    /// <summary>
    /// Wipes chart of note data only.
    /// </summary>
    public void ClearNotesOnly()
    {
        floatCursorPosition = 0;

        Creator_Note[] listNoteC = FindObjectsOfType<Creator_Note>();
        foreach (Creator_Note x in listNoteC)
        {
            DeleteNote(x);
        }

        CalculateChartLevel();
    }
    
    public void FileChartFieldEdit(string x)
    {
        textFileName.text = x;
    }
    /// <summary>
    /// Refreshes folder list for folder selection window.
    /// </summary>
    public void FolderListRefresh()
    {
        // Remove previous list
        foreach (Creator_ItemFolder x in listItemFolder)
        {
            Destroy(x.gameObject);
        }
        listItemFolder.Clear();

        // Get the folder names
        string path = Directory.GetCurrentDirectory() + "/Songs";
        DirectoryInfo main = new DirectoryInfo(path);
        DirectoryInfo[] sub = main.GetDirectories();
        Creator_ItemFolder newFolder = null;

        // Create folder buttons
        itemFolderTemplate.gameObject.SetActive(false);
        foreach (DirectoryInfo x in sub)
        {
            newFolder = Instantiate(itemFolderTemplate);
            newFolder.transform.SetParent(transformItemFolderParent);
            newFolder.transform.localScale = Vector3.one;
            newFolder.name = newFolder.textFolderName.text = x.Name;

            // Quantity detection
            int[] quantity = new int[4];
            // i = game mode
            // j = chart number
            for (int i = 0; i < quantity.Length; i++)
            {
                for (int j = 0; j < 1000; j++)
                {
                    path = Directory.GetCurrentDirectory() + "/Songs/" + newFolder.name + "/" + newFolder.name + "-" + i.ToString() + "-" + j.ToString() + ".txt";
                    if (!File.Exists(path))
                    {
                        quantity[i] = j;
                        break;
                    }
                }
            }
            newFolder.textFolderInfo.text = quantity[0].ToString() + " : " + quantity[1].ToString() + " : " + quantity[2].ToString() + " : " + quantity[3].ToString();
            //newFolder.textFolderInfo.text = "LN" + quantity[0].ToString() + " DB" + quantity[1].ToString() + " QD" + quantity[2].ToString() + " ND" + quantity[3].ToString();

            newFolder.gameObject.SetActive(true);
            listItemFolder.Add(newFolder);
        }
    }

    /// <summary>
    /// Create a note using the chart's note data.
    /// </summary>
    /// <param name="note"></param>
    public void CreateNoteCatch(ChartData.NoteInfo note)
    {
        // Object pooling - try to get an unused object from the pool, and make a new object if there are no unused ones
        Creator_Note newNote = null;
        foreach (Creator_Note x in listNoteCatchPool)
        {
            if (!x.gameObject.activeSelf)
            {
                newNote = x;
                break;
            }
        }
        if (newNote == null)
        {
            newNote = Instantiate(objectNoteCatchPrefab);
            listNoteCatchPool.Add(newNote);
        }

        // Modify note to use information given
        newNote.isNoteTap = false;
        newNote.transform.position = new Vector3(note.position, note.time);
        newNote.type = note.type % 4;
        newNote.size = note.size;
        newNote.speed = note.speed;
        newNote.gameObject.layer = 9 + newNote.type;
        if (note.speed < 0.01f)
        {
            newNote.speed = 1f;
        }
        // Long note visualization
        if (note.length > 0.01f)
        {
            newNote.length = note.length;
            newNote.spriteRendererLength.gameObject.SetActive(true);
            newNote.spriteRendererLength.gameObject.layer = 13 + newNote.type;
            newNote.spriteRendererLength.transform.localPosition = Vector3.up * note.length * 0.5f;
            newNote.spriteRendererLength.transform.localScale = new Vector3(
                newNote.spriteRendererLength.transform.localScale.x,
                25f * note.length,
                newNote.spriteRendererLength.transform.localScale.z);
            newNote.spriteRendererLengthEndNote.gameObject.SetActive(true);
            newNote.spriteRendererLengthEndNote.gameObject.layer = 13 + newNote.type;
            newNote.spriteRendererLengthEndNote.transform.localPosition = Vector3.up * note.length;
        }
        else
        {
            newNote.length = 0;
            newNote.spriteRendererLength.gameObject.SetActive(false);
            newNote.spriteRendererLengthEndNote.gameObject.SetActive(false);
        }
        newNote.other = note.other;

        // Colorize note based on type
        newNote.GetComponent<SpriteRenderer>().color = colorNote[note.type % 4];
        if (colorNote.Length > 0.01f)
        {
            Color dim = colorNote[note.type];
            dim.r *= 0.7f;
            dim.g *= 0.7f;
            dim.b *= 0.7f;
            newNote.spriteRendererLength.color = dim;
            newNote.spriteRendererLengthEndNote.color = dim;
        }

        // Update note's text mesh
        switch (newNote.type)
        {
            default:
                newNote.textMeshNoteType.gameObject.SetActive(false);
                break;
            case 0:
            case 4:
                newNote.textMeshNoteType.text = "1";
                newNote.textMeshNoteType.anchor = TextAnchor.LowerRight;
                break;
            case 1:
            case 5:
                newNote.textMeshNoteType.text = "2";
                newNote.textMeshNoteType.anchor = TextAnchor.LowerLeft;
                break;
            case 2:
            case 6:
                newNote.textMeshNoteType.text = "3";
                newNote.textMeshNoteType.anchor = TextAnchor.UpperRight;
                break;
            case 3:
            case 7:
                newNote.textMeshNoteType.text = "4";
                newNote.textMeshNoteType.anchor = TextAnchor.UpperLeft;
                break;
        }
        newNote.textMeshNoteOther.text = "";
        foreach (string x in newNote.other)
        {
            newNote.textMeshNoteOther.text = " " + x + " ";
        }

        // Make note active
        newNote.gameObject.SetActive(true);

        ShortcutKeysEnable();
    }
    /// <summary>
    /// Create a new custom note from scratch.
    /// </summary>
    /// <param name="position">Note's horizontal position (right is positive). If this note would be used for the vertical catchers, this would be used for the vertical position instead (up is positive).</param>
    /// <param name="time">Note's time. Determines when the note will pass the catcher's position.</param>
    /// <param name="type">Note's type. Determines which catcher it will appear for.</param>
    /// <param name="size">Note's size. Larger sizes make note judgment easier. Also affects note's appearance. The default value is 0 and cannot be lower than that.</param>
    /// <param name="length">Note's length. Values above 0 make the note become a long note. Leave it at 0 to make it a normal note instead.</param>
    /// <param name="speed">Note's scroll speed multiplier. Values above 1 make the note move faster and values below 1 make the note move slower. The default value is 1 and cannot be lower 0.</param>
    /// <param name="other">Note's additional attributes.</param>
    public void CreateNoteCatch(float position, float time, int type = 0, int size = 0, float length = 0f, float speed = 1f, List<string> other = null)
    {
        //ChartData.NoteInfo newNote = new ChartData.NoteInfo();
        ChartData.NoteInfo newNote = ScriptableObject.CreateInstance(typeof(ChartData.NoteInfo)) as ChartData.NoteInfo;
        newNote.position = position;
        newNote.time = time;
        newNote.type = type;
        newNote.size = size;
        newNote.length = length;
        newNote.speed = speed;
        if (other == null)
        {
            newNote.other = new List<string>();
        }
        else
        {
            newNote.other = other;
        }

        CreateNoteCatch(newNote);
    }

    /// <summary>
    /// Create a note using the chart's note data.
    /// </summary>
    /// <param name="note"></param>
    public void CreateNoteTap(ChartData.NoteInfo note)
    {
        bool createNote = true;
        Creator_Note newNote = null;
        foreach (Creator_Note x in listNoteTapPool)
        {
            if (!x.gameObject.activeSelf)
            {
                newNote = x;
                createNote = false;
                break;
            }
        }
        if (createNote)
        {
            newNote = Instantiate(objectNoteTapPrefab);
            listNoteTapPool.Add(newNote);
        }

        newNote.isNoteTap = true;
        newNote.transform.position = new Vector3(0f, note.time);
        newNote.type = note.type % 4;
        newNote.size = note.size;
        newNote.speed = note.speed;
        newNote.gameObject.layer = 9 + newNote.type;
        if (note.speed < 0.01f)
        {
            newNote.speed = 1f;
        }
        if (note.length > 0.01f)
        {
            newNote.length = note.length;
            newNote.spriteRendererLength.gameObject.SetActive(true);
            newNote.spriteRendererLength.gameObject.layer = 13 + newNote.type;
            newNote.spriteRendererLength.transform.localPosition = Vector3.up * note.length * 0.5f;
            newNote.spriteRendererLength.transform.localScale = new Vector3(
                newNote.spriteRendererLength.transform.localScale.x,
                25f * note.length,
                newNote.spriteRendererLength.transform.localScale.z);
            newNote.spriteRendererLengthEndNote.gameObject.SetActive(true);
            newNote.spriteRendererLengthEndNote.gameObject.layer = 13 + newNote.type;
            newNote.spriteRendererLengthEndNote.transform.localPosition = Vector3.up * note.length;
        }
        else
        {
            newNote.length = 0;
            newNote.spriteRendererLength.gameObject.SetActive(false);
            newNote.spriteRendererLengthEndNote.gameObject.SetActive(false);
        }
        newNote.other = note.other;
        
        newNote.GetComponent<SpriteRenderer>().color = colorNote[note.type % 4];
        if (colorNote.Length > 0.01f)
        {
            Color dim = colorNote[note.type % 4];
            dim.r *= 0.7f;
            dim.g *= 0.7f;
            dim.b *= 0.7f;
            newNote.spriteRendererLength.color = dim;
            newNote.spriteRendererLengthEndNote.color = dim;
        }
        
        switch (newNote.type)
        {
            default:
                newNote.textMeshNoteType.gameObject.SetActive(false);
                break;
            case 0:
            case 4:
                newNote.textMeshNoteType.text = "1";
                newNote.textMeshNoteType.anchor = TextAnchor.LowerRight;
                break;
            case 1:
            case 5:
                newNote.textMeshNoteType.text = "2";
                newNote.textMeshNoteType.anchor = TextAnchor.LowerLeft;
                break;
            case 2:
            case 6:
                newNote.textMeshNoteType.text = "3";
                newNote.textMeshNoteType.anchor = TextAnchor.UpperRight;
                break;
            case 3:
            case 7:
                newNote.textMeshNoteType.text = "4";
                newNote.textMeshNoteType.anchor = TextAnchor.UpperLeft;
                break;
        }
        newNote.textMeshNoteOther.text = "";
        foreach (string x in newNote.other)
        {
            newNote.textMeshNoteOther.text = " " + x + " ";
        }

        // Make note active
        newNote.gameObject.SetActive(true);
    }
    /// <summary>
    /// Create a new custom note from scratch.
    /// </summary>
    /// <param name="time">Note's time. Determines when the note will pass the catcher's position.</param>
    /// <param name="type">Note's type. Determines which catcher it will appear for.</param>
    /// <param name="size">Note's size. Larger sizes make note judgment easier. Also affects note's appearance. The default value is 0 and cannot be lower than that.</param>
    /// <param name="length">Note's length. Values above 0 make the note become a long note. Leave it at 0 to make it a normal note instead.</param>
    /// <param name="speed">Note's scroll speed multiplier. Values above 1 make the note move faster and values below 1 make the note move slower. The default value is 1 and cannot be lower 0.</param>
    /// <param name="other">Note's additional attributes.</param>
    public void CreateNoteTap(float time, int type = 0, int size = 0, float length = 0f, float speed = 1f, List<string> other = null)
    {
        //ChartData.NoteInfo newNote = new ChartData.NoteInfo();
        ChartData.NoteInfo newNote = ScriptableObject.CreateInstance(typeof(ChartData.NoteInfo)) as ChartData.NoteInfo;
        newNote.time = time;
        newNote.type = type;
        newNote.size = size;
        newNote.speed = speed;
        newNote.length = length;
        if (other == null)
        {
            newNote.other = new List<string>();
        }
        else
        {
            newNote.other = other;
        }

        CreateNoteTap(newNote);
    }

    /// <summary>
    /// Create a special effect using the chart's note data.
    /// </summary>
    /// <param name="effect"></param>
    public void CreateSpecialEffect(ChartData.SpecialEffectInfo effect)
    {
        // Object pooling - try to get an unused object from the pool, and make a new object if there are no unused ones
        bool createNote = true;
        Creator_SpecialEffect newEffect = null;
        foreach (Creator_SpecialEffect x in listSpecialEffectPool)
        {
            if (!x.gameObject.activeSelf)
            {
                newEffect = x;
                createNote = false;
                break;
            }
        }
        if (createNote)
        {
            newEffect = Instantiate(objectSpecialEffectPrefab);
            listSpecialEffectPool.Add(newEffect);
        }

        // Modify effect to use information given
        newEffect.transform.position = new Vector3(0f, effect.time);
        newEffect.type = effect.type;
        newEffect.intensity = effect.intensity;
        newEffect.duration = effect.duration;

        // Change sprite depending on its type
        if (newEffect.type < spriteSpecialEffect.Length)
        {
            newEffect.spriteRendererType.sprite = spriteSpecialEffect[newEffect.type];
        }

        // Make note active
        newEffect.gameObject.SetActive(true);
    }
    /// <summary>
    /// Create a new custom effect from scratch.
    /// </summary>
    /// <param name="time">"When" the effect will occur in the song.</param>
    /// <param name="type">The type of effect to use.</param>
    /// <param name="duration">The duration of the effect, if applicable.</param>
    /// <param name="intensity">The "strength" of the effect, if applicable.</param>
    public void CreateSpecialEffect(float time, int type = 0, float duration = 4, int intensity = 1)
    {
        //ChartData.SpecialEffectInfo effect = new ChartData.SpecialEffectInfo();
        ChartData.SpecialEffectInfo effect = ScriptableObject.CreateInstance(typeof(ChartData.SpecialEffectInfo)) as ChartData.SpecialEffectInfo;
        effect.time = time;
        effect.type = type;
        effect.duration = duration;
        effect.intensity = intensity;

        CreateSpecialEffect(effect);
    }

    /// <summary>
    /// Change the game type ID for this chart.
    /// </summary>
    /// <param name="modifier">The additive value on the game type ID.</param>
    public void ChartGameTypeChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);

        bool boolChangeTypeValidity = true;
        foreach (Creator_Note x in listNoteCatchPool)
        {
            if (x.gameObject.activeSelf)
            {
                boolChangeTypeValidity = false;
                break;
            }
        }
        if (boolChangeTypeValidity)
        {
            foreach (Creator_Note x in listNoteTapPool)
            {
                if (x.gameObject.activeSelf)
                {
                    boolChangeTypeValidity = false;
                    break;
                }
            }
        }
        if (!boolChangeTypeValidity)
        {
            if (!isNotificationShownGameTypeChangeDisable)
            {
                isNotificationShownGameTypeChangeDisable = true;
                Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORCHANGEGAMETYPE1", "You must clear ALL notes from the chart\nif you want to change game modes."), Color.white);
                Notification.Display(Translator.GetStringTranslation("CREATOR_ERRORCHANGEGAMETYPE2", "To clear all notes, click the following buttons:\nEditor Settings > Clear Chart > Clear Notes Only"), Color.white);
            }
            return;
        }

        intChartGameType += modifier;
        if (intChartGameType < 0)
        {
            intChartGameType = stringChartGameType.Length - 1;
        }
        if (intChartGameType >= stringChartGameType.Length)
        {
            intChartGameType = 0;
        }

        intNotePlacementType = 0;
    }

    public void AddNoteEffect(Text text)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        text.text.Trim(' ');
        text.text.TrimStart(' ');
        text.text.TrimEnd(' ');
        if (text.text.Length <= 0)
        {
            return;
        }

        PlaySound(clipTick);
        if (listStringNoteOther.Count >= 8)
        {
            return;
        }
        listStringNoteOther.Add(text.text);
    }
    public void ClearOneNoteEffect(Text text)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);
        listStringNoteOther.Remove(text.text);
    }
    public void ClearAllNoteEffect()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);
        listStringNoteOther.Clear();
    }

    public float GetCurrentPos()
    {
        float songTempo = 0f;
        float.TryParse(textSongTempo.text, out songTempo);
        float pos = 60f / songTempo * floatCursorPosition;
        Vector3 lastChange = new Vector3(0f, songTempo, 0f);

        if (listTempoChange.Count > 0)
        {
            foreach (Vector3 x in listTempoChange)
            {
                if (floatCursorPosition >= x.x - Mathf.Epsilon)
                {
                    lastChange = x;
                }
            }

            /*
             *  x = Beat Position
             *  y = BPM
             *  z = Offset
             */
            pos = ((floatCursorPosition - lastChange.x) * (60f / lastChange.y)) + lastChange.z;
        }

        return pos;
    }
    public void TempoChangeAdd()
    {
        float floatTempoChange = 0f;
        float.TryParse(inputTempoChangeAdd.text, out floatTempoChange);

        if (floatTempoChange < 0.01f - Mathf.Epsilon)
        {
            return;
        }

        Vector3 newChange = new Vector3()
        {
            x = floatCursorPosition,
            y = Mathf.Abs(floatTempoChange),
            z = GetCurrentPos()
        };
        listTempoChange.Add(newChange);

        TempoChangeRefreshList();
    }
    public void TempoChangeRefreshList()
    {
        foreach(Creator_ItemTempoChange x in listItemTempoChange)
        {
            Destroy(x.gameObject);
        }
        listItemTempoChange.Clear();

        listTempoChange.Sort((a, b) => a.x.CompareTo(b.x));

        itemTempoChangeTemplate.gameObject.SetActive(false);
        for (int i = 0; i < listTempoChange.Count; i++)
        {
            Creator_ItemTempoChange item = Instantiate(itemTempoChangeTemplate);
            item.transform.SetParent(transformItemTempoChangeParent);
            item.transform.localScale = Vector3.one;
            item.name = item.textItem.text = listTempoChange[i].x + " : " + listTempoChange[i].y + " : " + listTempoChange[i].z.ToString("f3");
            item.itemID = i;
            item.gameObject.SetActive(true);
            listItemTempoChange.Add(item);
        }
    }
    public void TempoChangeRemove(Creator_ItemTempoChange item)
    {
        listTempoChange.RemoveAt(item.itemID);
        TempoChangeRefreshList();
    }
    public void TempoChangeEdit(Creator_ItemTempoChange item)
    {
        inputTempoChangeEdit[0].text = listTempoChange[item.itemID].x.ToString();
        inputTempoChangeEdit[1].text = listTempoChange[item.itemID].y.ToString();
        inputTempoChangeEdit[2].text = listTempoChange[item.itemID].z.ToString();
        intTempoChangeEdit = item.itemID;
    }
    public void TempoChangeEditAttempt()
    {
        float floatTempoChangeBeat = 0f;
        float floatTempoChangeBPM = 0f;
        float floatTempoChangeTime = 0f;
        float.TryParse(inputTempoChangeEdit[0].text, out floatTempoChangeBeat);
        float.TryParse(inputTempoChangeEdit[1].text, out floatTempoChangeBPM);
        float.TryParse(inputTempoChangeEdit[2].text, out floatTempoChangeTime);

        if (floatTempoChangeBeat < -Mathf.Epsilon || floatTempoChangeBPM < 0.01f - Mathf.Epsilon || floatTempoChangeTime < -Mathf.Epsilon)
        {
            Notification.Display("Failed to edit the BPM change. At least one of the input fields has an invalid value.", new Color(1f, 0.4f, 0.4f));
            return;
        }

        Vector3 newChange = new Vector3()
        {
            x = floatTempoChangeBeat,
            y = floatTempoChangeBPM,
            z = floatTempoChangeTime
        };
        listTempoChange.RemoveAt(intTempoChangeEdit);
        listTempoChange.Add(newChange);
        TempoChangeRefreshList();
    }

    /// <summary>
    /// Change the current note placement type.
    /// </summary>
    /// <param name="modifier">The additive value on the note type ID.</param>
    public void ChartNotePlacementTypeChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);
        intNotePlacementType += modifier;
        if (intNotePlacementType < 0)
        {
            intNotePlacementType = stringNotePlacementType.Length - 1;
        }
        if (intNotePlacementType >= stringNotePlacementType.Length)
        {
            intNotePlacementType = 0;
        }
    }
    public void ChartNotePlacementTypeChangeTo(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);

        if ((intChartGameType == 0 && modifier % 4 != 0) ||
            (intChartGameType == 1 && modifier % 4 > 1))
        {
            Notification.Display("Unable to place that note in this game mode.", Color.white);
            return;
        }
        intNotePlacementType = modifier;
    }

    public void MouseScrollSettingChange(int modifier)
    {
        PlaySound(clipTick);
        intMouseScrollSetting += modifier;
        if (intMouseScrollSetting < 0)
        {
            intMouseScrollSetting = stringMouseScrollSetting.Length - 1;
        }
        if (intMouseScrollSetting >= stringMouseScrollSetting.Length)
        {
            intMouseScrollSetting = 0;
        }
    }

    /// <summary>
    /// Change beat snap divisor value ID.
    /// </summary>
    /// <param name="modifier">The additive value on the divisor type ID.</param>
    public void BeatSnapDivisorChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);
        intBeatSnapDivisor += modifier;
        if (intBeatSnapDivisor < -1)
        {
            intBeatSnapDivisor = -1;
        }
        if (intBeatSnapDivisor >= intBeatSnapDivisorValue.Length)
        {
            intBeatSnapDivisor = intBeatSnapDivisorValue.Length - 1;
        }
        RefreshGridSnapDivisorGuide();
    }
    public void HoriPosSnapDivisorChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        PlaySound(clipTick);
        intHoriPosSnapDivisor += modifier;
        if (intHoriPosSnapDivisor < 0)
        {
            intHoriPosSnapDivisor = 0;
        }
        if (intHoriPosSnapDivisor >= intHoriPosSnapDivisorValue.Length)
        {
            intHoriPosSnapDivisor = intHoriPosSnapDivisorValue.Length - 1;
        }
        RefreshGridSnapDivisorGuide();
    }

    /// <summary>
    /// Refreshes the beat snap divisor guide lines (the horizontal white lines).
    /// </summary>
    private void RefreshGridSnapDivisorGuide()
    {
        foreach (GameObject x in listObjectBeatSnapDivisorGuide)
        {
            x.SetActive(false);
        }
        foreach (GameObject x in listObjectHoriPosSnapDivisorGuide)
        {
            x.SetActive(false);
        }

        GameObject line = null;
        if (intBeatSnapDivisor >= 0)
        {
            for (int i = 0; i <= 10; i++)
            {
                for (float f = 0; f < 1 - Mathf.Epsilon; f += 1f / intBeatSnapDivisorValue[intBeatSnapDivisor])
                {
                    if (f < Mathf.Epsilon) continue;

                    line = GetObjectBeatSnapDivisorGuide();
                    line.transform.parent = objectCentralPivot.transform;
                    line.transform.localPosition = Vector3.up * (f + i);
                    line.SetActive(true);
                    line = GetObjectBeatSnapDivisorGuide();
                    line.transform.parent = objectCentralPivot.transform;
                    line.transform.localPosition = Vector3.down * (f + i);
                    line.SetActive(true);
                }
            }
        }

        for (float f = 0; f < 1 - Mathf.Epsilon; f += 1f / intHoriPosSnapDivisorValue[intHoriPosSnapDivisor])
        {
            if (f < Mathf.Epsilon) continue;

            line = GetObjectHoriPosSnapDivisorGuide();
            line.transform.parent = objectCentralPivot.transform;
            line.transform.localPosition = Vector3.left * f;
            line.SetActive(true);
            line = GetObjectHoriPosSnapDivisorGuide();
            line.transform.parent = objectCentralPivot.transform;
            line.transform.localPosition = Vector3.right * f;
            line.SetActive(true);
        }
    }
    private GameObject GetObjectBeatSnapDivisorGuide()
    {
        foreach (GameObject x in listObjectBeatSnapDivisorGuide)
        {
            if (!x.activeSelf)
            {
                return x;
            }
        }

        GameObject newLine = Instantiate(objectBeatSnapDivisorGuidePrefab);
        listObjectBeatSnapDivisorGuide.Add(newLine);
        return newLine;
    }
    private GameObject GetObjectHoriPosSnapDivisorGuide()
    {
        foreach (GameObject x in listObjectHoriPosSnapDivisorGuide)
        {
            if (!x.activeSelf)
            {
                return x;
            }
        }

        GameObject newLine = Instantiate(objectHoriPosSnapDivisorGuidePrefab);
        listObjectHoriPosSnapDivisorGuide.Add(newLine);
        return newLine;
    }

    public void AdjustBeatsPerMeasure(int modifier)
    {
        PlaySound(clipTick);
        intBeatsPerMeasure = Mathf.Clamp(intBeatsPerMeasure + modifier, 3, 9);
    }

    public void ChartJudgeLevelChange(int modifier)
    {
        intChartJudge += modifier;

        if (intChartJudge < 0) intChartJudge = 0;
        if (intChartJudge > 9) intChartJudge = 9;

        CalculateChartLevel();
    }

    /// <summary>
    /// Despawns a note.
    /// </summary>
    /// <param name="note">The note to be despawned.</param>
    public void DeleteNote(Creator_Note note)
    {
        note.transform.position = Vector3.down * 100000f + Vector3.right * 10f;
        note.gameObject.SetActive(false);
    }

    /// <summary>
    /// Toggle activity of editor setting window canvas.
    /// </summary>
    public void ToggleCreatorSettingWindow()
    {
        if (canvasCreatorSetting.gameObject.activeSelf)
        {
            SaveCreatorSettings();
        }

        PlaySound(clipSelect);
        canvasCreatorSetting.gameObject.SetActive(!canvasCreatorSetting.gameObject.activeSelf);
    }

    public void SaveCreatorSettings()
    {
        PlayerPrefs.SetString("creator_textFileName", textFileName.text);
        PlayerPrefs.SetInt("creator_intChartGameType", intChartGameType);
        PlayerPrefs.SetString("creator_textFileChart", textFileChart.text);
        PlayerPrefs.SetInt("creator_intMouseScrollSetting", intMouseScrollSetting);
        PlayerPrefs.SetInt("creator_intBeatSnapDivisor", intBeatSnapDivisor);
        PlayerPrefs.SetInt("creator_intHoriPosSnapDivisor", intHoriPosSnapDivisor);
        PlayerPrefs.SetFloat("creator_sliderZoom", sliderZoom.value);
        PlayerPrefs.SetFloat("creator_floatSongPreviewLength", sliderSongPreviewLength.value);
        PlayerPrefs.SetFloat("creator_floatSongPreviewFade", sliderSongPreviewFade.value);

        PlayerPrefs.Save();
    }

    public void ShortcutKeysEnable()
    {
        isShortcutKeyDisabled = false;
    }
    public void ShortcutKeysDisable()
    {
        isShortcutKeyDisabled = true;
    }

    public void ToggleObjectActivity(GameObject x)
    {
        x.SetActive(!x.activeSelf);
    }

    /// <summary>
    /// Updates the chart level value.
    /// </summary>
    public void CalculateChartLevel()
    {
        List<Vector2> listNotePosHori = new List<Vector2>();
        List<Vector2> listNotePosVert = new List<Vector2>();
        List<float> listNoteTap = new List<float>();
        //int intNoteTapCount = 0;

        // Get all the catcher points in the chart
        foreach (Creator_Note x in listNoteCatchPool)
        {
            if (x.gameObject.activeSelf)
            {
                switch (x.type)
                {
                    case 0:
                        listNotePosHori.Add(new Vector2(x.transform.position.x, x.transform.position.y));
                        if (x.length > 0.01f)
                        {
                            listNotePosHori.Add(new Vector2(x.transform.position.x, x.transform.position.y + x.length));
                        }
                        break;
                    case 1:
                        listNotePosHori.Add(new Vector2(-x.transform.position.x, x.transform.position.y));
                        if (x.length > 0.01f)
                        {
                            listNotePosHori.Add(new Vector2(-x.transform.position.x, x.transform.position.y + x.length));
                        }
                        break;
                    case 2:
                        listNotePosVert.Add(new Vector2(x.transform.position.x, x.transform.position.y));
                        if (x.length > 0.01f)
                        {
                            listNotePosVert.Add(new Vector2(x.transform.position.x, x.transform.position.y + x.length));
                        }
                        break;
                    case 3:
                        listNotePosVert.Add(new Vector2(-x.transform.position.x, x.transform.position.y));
                        if (x.length > 0.01f)
                        {
                            listNotePosVert.Add(new Vector2(-x.transform.position.x, x.transform.position.y + x.length));
                        }
                        break;
                }
            }
        }

        // Sort the points
        if (listNotePosHori.Count > 1)
        {
            bool keepSorting = false;
            do
            {
                keepSorting = false;
                for (int i = 0; i < listNotePosHori.Count - 1; i++)
                {
                    if (listNotePosHori[i].y > listNotePosHori[i + 1].y)
                    {
                        keepSorting = true;
                        Vector2 tempV2 = listNotePosHori[i];
                        listNotePosHori[i] = listNotePosHori[i + 1];
                        listNotePosHori[i + 1] = tempV2;
                    }
                }
            } while (keepSorting);
        }
        if (listNotePosVert.Count > 1)
        {
            bool keepSorting = false;
            do
            {
                keepSorting = false;
                for (int i = 0; i < listNotePosVert.Count - 1; i++)
                {
                    if (listNotePosVert[i].y > listNotePosVert[i + 1].y)
                    {
                        keepSorting = true;
                        Vector2 tempV2 = listNotePosVert[i];
                        listNotePosVert[i] = listNotePosVert[i + 1];
                        listNotePosVert[i + 1] = tempV2;
                    }
                }
            } while (keepSorting);
        }

        // Tap notes quantity
        foreach (Creator_Note x in listNoteTapPool)
        {
            if (x.gameObject.activeSelf)
            {
                listNoteTap.Add(x.transform.position.y);
            }
        }
        listNoteTap.Sort();

        // Now calculate distance between points
        float floatTotalMovement = 0f;
        float floatNotePositionFirstNote = 0f;
        if (listNotePosHori.Count > 0) floatNotePositionFirstNote = listNotePosHori[0].y;
        if (listNotePosVert.Count > 0 && listNotePosVert[0].y < floatNotePositionFirstNote) floatNotePositionFirstNote = listNotePosVert[0].y;
        if (listNoteTap.Count > 0 && listNoteTap[0] < floatNotePositionFirstNote) floatNotePositionFirstNote = listNoteTap[0];
        float floatNotePositionLastNote = floatNotePositionFirstNote;
        if (listNotePosHori.Count > 0) floatNotePositionLastNote = listNotePosHori[listNotePosHori.Count - 1].y;
        if (listNotePosVert.Count > 0 && listNotePosVert[listNotePosVert.Count - 1].y > floatNotePositionLastNote) floatNotePositionLastNote = listNotePosVert[listNotePosVert.Count - 1].y;
        if (listNoteTap.Count > 0 && listNoteTap[listNoteTap.Count - 1] > floatNotePositionLastNote) floatNotePositionLastNote = listNoteTap[listNoteTap.Count - 1];

        if (listNotePosHori.Count > 1)
        {
            for (int i = 0; i < listNotePosHori.Count - 1; i++)
            {
                if (Mathf.Abs(listNotePosHori[i].y - listNotePosHori[i + 1].y) > 0.01f)
                {
                    floatTotalMovement += CalculateIntensityOfNotes(listNotePosHori[i], listNotePosHori[i + 1]);
                }
            }
        }
        if (listNotePosVert.Count > 1)
        {
            for (int i = 0; i < listNotePosVert.Count - 1; i++)
            {
                if (Mathf.Abs(listNotePosVert[i].y - listNotePosVert[i + 1].y) > 0.01f)
                {
                    floatTotalMovement += CalculateIntensityOfNotes(listNotePosVert[i], listNotePosVert[i + 1]);
                }
            }
        }

        // Final value calculation
        float notesPerBeat = 1f * (listNotePosHori.Count + listNotePosVert.Count + listNoteTapPool.Count) / (floatNotePositionLastNote - floatNotePositionFirstNote);
        //int intChartJudge = 0;
        //int.TryParse(textChartJudge.text, out intChartJudge);
        float finalChartLevel = Mathf.Sqrt(Mathf.Pow(Mathf.Sqrt(notesPerBeat) + floatTotalMovement - 1f, 1.6f) * (0.09f + (0.01f * intChartJudge)));

        intChartLevel = 1 + Mathf.FloorToInt(finalChartLevel);
        if (intChartLevel < 1) intChartLevel = 1;
        if (intChartLevel > 20) intChartLevel = 20;
        
        floatGameplayLength = floatNotePositionLastNote - floatNotePositionFirstNote;

        float floatTextSongLength = 0f;
        float.TryParse(textSongLength.text, out floatTextSongLength);
        if (floatNotePositionLastNote + 8f > floatTextSongLength)
        {
            floatTextSongLength = floatNotePositionLastNote + 8f;
        }
        textSongLength.text = floatTextSongLength.ToString("f0");

        textChartJudge.text = (intChartJudge + 1).ToString();

#if UNITY_EDITOR
        textChartLevel.text = Translator.GetStringTranslation("CREATOR_CHARTLEVEL", "CHART LEVEL") + " " + intChartLevel.ToString() + " (" + (1 + finalChartLevel).ToString("f3") + ")";
#else
        textChartLevel.text = Translator.GetStringTranslation("CREATOR_CHARTLEVEL", "CHART LEVEL") + " " + intChartLevel.ToString();
#endif
    }
    public float CalculateIntensityOfNotes(Vector2 point1, Vector2 point2)
    {
        float calc = 0f;
        calc = Mathf.Abs(Mathf.Pow(Mathf.Abs(point1.x - point2.x), 2) * (1f / (point2.y - point1.y + 1f)));
        /*
        if (intChartGameType != 3)
        {
            calc = Mathf.Abs(Mathf.Pow(Mathf.Abs(point1.x - point2.x), 2) * (1f / (point2.y - point1.y + 1f)));
        }
        else
        {
            calc = Mathf.Abs(point1.x - point2.x) + Mathf.Abs(point1.y - point2.y);
        }
        */
        return calc;
    }

    public void PlaySound(AudioClip clip)
    {
        if (mAudioSource == null || clip == null)
        {
            return;
        }
        mAudioSource.PlayOneShot(clip, PlayerSetting.setting.floatVolumeEffect);
    }

    public void SceneTransferToTitle()
    {
        SaveCreatorSettings();
        StartCoroutine("_SceneTransferToTitle");
    }
    private IEnumerator _SceneTransferToTitle()
    {
        yield return null;
        //SceneManager.LoadScene(stringSceneNameTitle);
        SceneTransition.LoadScene(stringSceneNameTitle);
    }

    private void FixedUpdate()
    {
        if (fixedUpdateCheckOtherFrame)
        {
            fixedUpdateCheckOtherFrame = false;
            return;
        }
        fixedUpdateCheckOtherFrame = true;

        // Sort note order
        if (listNoteCatchPool.Count >= 2)
        {
            bool keepSorting = false;
            do
            {
                keepSorting = false;
                for (int i = 0; i < listNoteCatchPool.Count - 1; i++)
                {
                    if (listNoteCatchPool[i].transform.position.y > listNoteCatchPool[i + 1].transform.position.y + Mathf.Epsilon)
                    {
                        Creator_Note tempNote = listNoteCatchPool[i];
                        listNoteCatchPool[i] = listNoteCatchPool[i + 1];
                        listNoteCatchPool[i + 1] = tempNote;
                        keepSorting = true;
                    }
                }
            } while (keepSorting);
        }

        // Display cursor position
        float songTempo = 0;
        textTimeCurrentMeasure.text =
            Translator.GetStringTranslation("CREATOR_CURRENTMEASURE", "Measure") + " " + (Mathf.Floor(floatCursorPosition / intBeatsPerMeasure) + 1).ToString() + " " +
            Translator.GetStringTranslation("CREATOR_CURRENTBEAT", "Beat") + " " + (Mathf.Floor(floatCursorPosition % intBeatsPerMeasure) + 1).ToString() + " (" + Mathf.Floor(floatCursorPosition + 1).ToString() + ")";
        //if (float.TryParse(textSongTempo.text, out songTempo))
        //{
        //    textTimeCurrentLength.text = Translator.GetStringTranslation("CREATOR_CURRENTSONGPOSITION", "Song Pos") + " " + (Mathf.FloorToInt(60f / songTempo * floatCursorPosition / 60f)).ToString("0") + ":" + (60f / songTempo * floatCursorPosition % 60f).ToString("00.00");
        //}
        if (float.TryParse(textSongTempo.text, out songTempo))
        {
            textTimeCurrentLength.text = Translator.GetStringTranslation("CREATOR_CURRENTSONGPOSITION", "Song Pos") + " " + (Mathf.FloorToInt(GetCurrentPos() / 60f)).ToString("0") + ":" + (GetCurrentPos() % 60f).ToString("00.00");
        }
        else
        {
            textTimeCurrentLength.text = Translator.GetStringTranslation("CREATOR_CURRENTSONGPOSITIONUNKNOWN", "Song Pos UNKNOWN");
        }
        if (intBeatSnapDivisor >= 0)
        {
            textBeatSnapDivisor.text = "1/" + intBeatSnapDivisorValue[intBeatSnapDivisor].ToString();
        }
        else
        {
            textBeatSnapDivisor.text = "FREE";
        }
        textHoriPosSnapDivisor.text = "1/" + intHoriPosSnapDivisorValue[intHoriPosSnapDivisor].ToString();
        textBeatsPerMeasure.text = intBeatsPerMeasure.ToString();

        // Current selection
        textChartGameType.text = Translator.GetStringTranslation("CREATOR_CHARTGAMETYPEBODY" + intChartGameType.ToString(), stringChartGameType[intChartGameType]);
        textFileGameType.text = "-" + intChartGameType.ToString() + "-";
        textNotePlacementType.text = Translator.GetStringTranslation("CREATOR_NOTEPLACETYPEBODY" + intNotePlacementType.ToString(), stringNotePlacementType[intNotePlacementType]);
        textNoteOther.text = "";
        foreach (string s in listStringNoteOther)
        {
            textNoteOther.text += s + "\n";
        }

        textMouseScrollSetting.text = Translator.GetStringTranslation("CREATOR_OPTIONSMOUSESCROLLFUNCBODY" + intMouseScrollSetting.ToString(), stringMouseScrollSetting[intMouseScrollSetting]);
        textSongPreviewLength.text = sliderSongPreviewLength.value.ToString("f2");
        textSongPreviewFade.text = sliderSongPreviewFade.value.ToString("f2");

        // Note selection sprite highlight
        if (imageNotePlacementHighlight.Length > 0)
        {
            for (int i = 0; i < imageNotePlacementHighlight.Length; i++)
            {
                if (!imageNotePlacementHighlight[i].gameObject.activeSelf && i == intNotePlacementType)
                {
                    imageNotePlacementHighlight[i].gameObject.SetActive(true);
                }
                else if (imageNotePlacementHighlight[i].gameObject.activeSelf && i != intNotePlacementType)
                {
                    imageNotePlacementHighlight[i].gameObject.SetActive(false);
                }
            }
        }
        // Darken disabled note types
        if (imageNotePlacementInvalidDarken.Length > 0)
        {
            for (int i = 0; i < imageNotePlacementInvalidDarken.Length; i++)
            {
                if (!imageNotePlacementInvalidDarken[i].gameObject.activeSelf && intChartGameType == i)
                {
                    imageNotePlacementInvalidDarken[i].gameObject.SetActive(true);
                }
                else if (imageNotePlacementInvalidDarken[i].gameObject.activeSelf && intChartGameType != i)
                {
                    imageNotePlacementInvalidDarken[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void Update()
    {
        if (boolFullPreviewOngoing || isShortcutKeyDisabled)
        {
            return;
        }

        holdDelayCurrent -= Time.deltaTime;

        // Cursor movement by input
        if (Input.GetKey(KeyCode.DownArrow) && holdDelayCurrent < 0f)
        {
            PlaySound(clipTick);
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                floatCursorPosition -= 16;
            }
            else
            {
                floatCursorPosition--;
            }
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.UpArrow) && holdDelayCurrent < 0f)
        {
            PlaySound(clipTick);
            if (Input.GetKey(KeyCode.RightArrow))
            {
                floatCursorPosition += 16;
            }
            else
            {
                floatCursorPosition++;
            }
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && holdDelayCurrent < 0f)
        {
            PlaySound(clipTick);
            floatCursorPosition -= 4;
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.RightArrow) && holdDelayCurrent < 0f)
        {
            PlaySound(clipTick);
            floatCursorPosition += 4;
            holdDelayCurrent = holdDelay;
        }

        // Mouse scroll wheel input
        switch (intMouseScrollSetting)
        {
            default:
                if (Mathf.Abs(Input.GetAxis("MouseScrollWheel")) > Mathf.Epsilon)
                {
                    Debug.LogWarning("WARNING: This mouse scroll setting input type has no function. intMouseScrollSetting = " + intMouseScrollSetting.ToString());
                }
                break;
            case 0:
                if (Input.GetAxis("MouseScrollWheel") >  Mathf.Epsilon)
                {
                    PlaySound(clipTick);
                    floatCursorPosition++;
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    PlaySound(clipTick);
                    floatCursorPosition--;
                }
                break;
            case 1:
                if (Input.GetAxis("MouseScrollWheel") > Mathf.Epsilon)
                {
                    PlaySound(clipTick);
                    floatCursorPosition--;
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    PlaySound(clipTick);
                    floatCursorPosition++;
                }
                break;
            case 2:
                if (Input.GetAxis("MouseScrollWheel") > Mathf.Epsilon)
                {
                    PlaySound(clipTick);
                    sliderZoom.value = Mathf.Clamp01(sliderZoom.value - 0.08f);
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    PlaySound(clipTick);
                    sliderZoom.value = Mathf.Clamp01(sliderZoom.value + 0.08f);
                }
                break;
            case 3:
                if (Input.GetAxis("MouseScrollWheel") > Mathf.Epsilon)
                {
                    ChartNotePlacementTypeChange(-1);
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    ChartNotePlacementTypeChange(1);
                }
                break;
            case 4:
                if (Input.GetAxis("MouseScrollWheel") > Mathf.Epsilon)
                {
                    BeatSnapDivisorChange(1);
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    BeatSnapDivisorChange(-1);
                }
                break;
        }

        // Song full preview
        if (Input.GetKeyDown(KeyCode.Space))
        {
            boolFullPreviewOngoing = true;
            Creator_SongPreview.mSongPreview.PreviewAll();
        }

        // Left-click
//#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
//#else
        //if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetMouseButtonDown(0))
//#endif
        {
            // Do nothing if settings window is on
            if (canvasCreatorSetting.gameObject.activeSelf)
            {
                return;
            }

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 15f))
            {
                // Hold Left Alt - Create note
#if UNITY_EDITOR
                if ((hit.collider.tag == "Creator_ChartBack" || hit.collider.tag == "Creator_Note") &&
                    Input.GetKey(KeyCode.LeftAlt))
#else
                if ((hit.collider.tag == "Creator_ChartBack" || hit.collider.tag == "Creator_Note") &&
                    (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)))
#endif
                {
                    objectNoteSelected = null;

                    // Restrict certain note types from being placed in wrong game modes.
                    if ((intChartGameType == 0 && intNotePlacementType % 4 != 0) ||
                        (intChartGameType == 1 && intNotePlacementType % 4 > 1))
                    {
                        Notification.Display("Unable to place that note in this game mode.", Color.white);
                        return;
                    }

                    // Possible to create note only if note is at or above song start
                    if (hit.point.y > -Mathf.Epsilon)
                    {
                        float pos = Mathf.Clamp(hit.point.x, -1f, 1f);
                        float time = hit.point.y;
                        int type = intNotePlacementType;
                        float length = 0f;
                        float speed = 1f;
                        listStringNoteOther = new List<string>();
                        // Length parse from string & catch note only
                        if (toggleNoteLengthEnable.isOn && type < 4)
                        {
                            float.TryParse(textNoteLength.text, out length);
                        }
                        if (toggleNoteHitsoundEnable.isOn)
                        {
                            int intSoundID = -1;
                            if (int.TryParse(textNoteHitsound.text, out intSoundID))
                            {
                                listStringNoteOther.Add("sound:" + intSoundID.ToString());
                            }
                        }
                        float.TryParse(textNoteSpeed.text, out speed);

                        // Left shift ignores horizontal position grid snap
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            pos = Mathf.Round(intHoriPosSnapDivisorValue[intHoriPosSnapDivisor] * pos) / intHoriPosSnapDivisorValue[intHoriPosSnapDivisor];
                        }
                        // If there is a beat snap divisor, it will be applied
                        if (intBeatSnapDivisor >= 0)
                        {
                            time = Mathf.Round(intBeatSnapDivisorValue[intBeatSnapDivisor] * time) / intBeatSnapDivisorValue[intBeatSnapDivisor];
                            /*
                            float beat = Mathf.Floor(time);
                            float partial = time - beat;
                            for (int divisor = intBeatSnapDivisorValue[intBeatSnapDivisor] - 1; divisor >= 0 ; divisor--)
                            {
                                if (partial > 1f * divisor / intBeatSnapDivisorValue[intBeatSnapDivisor])
                                {
                                    time = beat + 1f * divisor / intBeatSnapDivisorValue[intBeatSnapDivisor];
                                    break;
                                }
                            }
                            */
                        }

                        if (CheckNotePlacementValidity(pos, time, type, length))
                        {
                            // Note repositioning for all game modes except Note Dodge
                            if (intChartGameType != 3)
                            {
                                // If there is a note of a type from the opposite catcher, place on the opposite horizontal position
                                foreach (Creator_Note x in listNoteCatchPool)
                                {
                                    // Note main position
                                    Creator_Note sameNote = null;
                                    Creator_Note oppositeNote = null;
                                    float dist = Mathf.Abs(time - x.transform.position.y);
                                    if (dist < 0.01f)
                                    {
                                        switch (type)
                                        {
                                            case 0: if (x.type == 1) oppositeNote = x; break;
                                            case 1: if (x.type == 0) oppositeNote = x; break;
                                            case 2: if (x.type == 3) oppositeNote = x; break;
                                            case 3: if (x.type == 2) oppositeNote = x; break;
                                        }
                                    }
                                    if (x.length > 0.01f)
                                    {
                                        // Long note end
                                        dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                                        if (dist < 0.01f)
                                        {
                                            switch (type)
                                            {
                                                case 0: if (x.type == 1) oppositeNote = x; break;
                                                case 1: if (x.type == 0) oppositeNote = x; break;
                                                case 2: if (x.type == 3) oppositeNote = x; break;
                                                case 3: if (x.type == 2) oppositeNote = x; break;
                                            }
                                        }

                                        // Long note length
                                        if (time < x.transform.position.y + x.length - Mathf.Epsilon && time > x.transform.position.y + Mathf.Epsilon)
                                        {
                                            if ((type == 0 || type == 1) && (x.type == 0 || x.type == 1))
                                            {
                                                if (type == x.type) sameNote = x;
                                                else oppositeNote = x;
                                            }
                                            if ((type == 2 || type == 3) && (x.type == 2 || x.type == 3))
                                            {
                                                if (type == x.type) sameNote = x;
                                                else oppositeNote = x;
                                            }
                                        }
                                    }

                                    if (sameNote != null)
                                    {
                                        pos = x.transform.position.x;
                                        break;
                                    }
                                    if (oppositeNote != null)
                                    {
                                        pos = -x.transform.position.x;
                                        break;
                                    }
                                }
                            }
#if UNITY_EDITOR
                            Debug.Log("Note creation: pos " + pos.ToString("f3") + ", time " + time.ToString("f3") + ", type " + type.ToString() + ", length " + length.ToString("f3") + ", speed " + speed.ToString("f3"));
#endif
                            if (type < 4)
                            {
                                CreateNoteCatch(pos, time, type, 0, length, speed, listStringNoteOther);
                            }
                            else
                            {
                                CreateNoteTap(time, type, 0, length, speed, listStringNoteOther);
                            }
                            PlaySound(clipNoteCreate);
                            CalculateChartLevel();
                        }
                        else
                        {
                            PlaySound(clipCancel);
                        }
                    }
                }
                // No key held - Move note
                // Hold Left Shift - Alter note's length
                else if (hit.collider.tag == "Creator_Note" && !Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.LeftCommand) && !Input.GetKey(KeyCode.LeftAlt))
                {
                    // Drag start - keep the on-clicked note in memory
                    objectNoteSelected = hit.collider.GetComponent<Creator_Note>();
                    PlaySound(clipTick);

                    objectSelectedNoteHighlight.SetActive(true);
                    objectSelectedNoteHighlight.transform.position = objectNoteSelected.transform.position;

                    boolEditNoteLength = Input.GetKey(KeyCode.LeftShift);
                }
            }
        }

        // Drag end
        if (Input.GetMouseButtonUp(0) && objectNoteSelected != null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            Creator_Note draggedNote = objectNoteSelected;
            objectNoteSelected = null;
            objectSelectedNoteHighlight.SetActive(false);

            if (Physics.Raycast(ray, out hit, 15f))
            {
                // Set note length
                if (boolEditNoteLength)
                {
                    float pos = draggedNote.transform.position.x;
                    float time = draggedNote.transform.position.y;
                    int type = draggedNote.type;
                    float length = hit.point.y - draggedNote.transform.position.y;
                    float speed = draggedNote.speed;

                    float oldPosY = draggedNote.transform.position.y;
                    draggedNote.transform.position += Vector3.down * (oldPosY + 1f);

                    length = Mathf.Round(intBeatSnapDivisorValue[intBeatSnapDivisor] * length) / intBeatSnapDivisorValue[intBeatSnapDivisor];

                    /*
                    // Old method: Edit the note directly
                    if (length < 0.01f || draggedNote.isNoteTap)
                    {
                        length = 0;
                        draggedNote.spriteRendererLength.gameObject.SetActive(false);
                        draggedNote.spriteRendererLengthEndNote.gameObject.SetActive(false);
                    }
                    else
                    {
                        draggedNote.length = length;
                        draggedNote.spriteRendererLength.gameObject.SetActive(true);
                        draggedNote.spriteRendererLength.transform.localPosition = Vector3.up * length * 0.5f;
                        draggedNote.spriteRendererLength.transform.localScale = new Vector3(
                            draggedNote.spriteRendererLength.transform.localScale.x,
                            25f * length,
                            draggedNote.spriteRendererLength.transform.localScale.z);
                        draggedNote.spriteRendererLengthEndNote.gameObject.SetActive(true);
                        draggedNote.spriteRendererLengthEndNote.transform.localPosition = Vector3.up * length;
                    }
                    */

                    if (CheckNotePlacementValidity(pos, time, type, length))
                    {
                        if (intChartGameType != 3)
                        {
                            foreach (Creator_Note x in listNoteCatchPool)
                            {
                                Creator_Note sameNote = null;
                                Creator_Note oppositeNote = null;
                                float dist = Mathf.Abs(time - x.transform.position.y);
                                if (dist < 0.01f)
                                {
                                    switch (type)
                                    {
                                        case 0: if (x.type == 1) oppositeNote = x; break;
                                        case 1: if (x.type == 0) oppositeNote = x; break;
                                        case 2: if (x.type == 3) oppositeNote = x; break;
                                        case 3: if (x.type == 2) oppositeNote = x; break;
                                    }
                                }
                                if (x.length > 0.01f)
                                {
                                    dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                                    if (dist < 0.01f)
                                    {
                                        switch (type)
                                        {
                                            case 0: if (x.type == 1) oppositeNote = x; break;
                                            case 1: if (x.type == 0) oppositeNote = x; break;
                                            case 2: if (x.type == 3) oppositeNote = x; break;
                                            case 3: if (x.type == 2) oppositeNote = x; break;
                                        }
                                    }
                                    if (time < x.transform.position.y + x.length - Mathf.Epsilon && time > x.transform.position.y + Mathf.Epsilon)
                                    {
                                        if ((type == 0 || type == 1) && (x.type == 0 || x.type == 1))
                                        {
                                            if (type == x.type) sameNote = x;
                                            else oppositeNote = x;
                                        }
                                        if ((type == 2 || type == 3) && (x.type == 2 || x.type == 3))
                                        {
                                            if (type == x.type) sameNote = x;
                                            else oppositeNote = x;
                                        }
                                    }
                                }

                                if (sameNote != null)
                                {
                                    pos = x.transform.position.x;
                                    break;
                                }
                                if (oppositeNote != null)
                                {
                                    pos = -x.transform.position.x;
                                    break;
                                }
                            }
                        }

                        if (!draggedNote.isNoteTap)
                        {
                            CreateNoteCatch(pos, time, type, 0, length, speed, draggedNote.other);
                        }
                        else
                        {
                            CreateNoteTap(time, type, 0, length, speed, draggedNote.other);
                        }
                        PlaySound(clipTick);
                        DeleteNote(draggedNote);
                        CalculateChartLevel();
                    }
                    else
                    {
                        draggedNote.transform.position += Vector3.up * (oldPosY + 1f);
                        PlaySound(clipCancel);
                    }
                }
                // Set note position
                else
                {
                    // More or less the same check as creating the note
                    if (hit.point.y > -Mathf.Epsilon)
                    {
                        float pos = hit.point.x;
                        float time = hit.point.y;
                        int type = draggedNote.type;
                        float length = draggedNote.length;
                        float speed = draggedNote.speed;
                        if (draggedNote.isNoteTap) type += 4;

                        float oldPosY = draggedNote.transform.position.y;
                        draggedNote.transform.position += Vector3.down * (oldPosY + 1f);

                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            pos = Mathf.Round(intHoriPosSnapDivisorValue[intHoriPosSnapDivisor] * pos) / intHoriPosSnapDivisorValue[intHoriPosSnapDivisor];
                        }
                        if (intBeatSnapDivisor >= 0)
                        {
                            time = Mathf.Round(intBeatSnapDivisorValue[intBeatSnapDivisor] * time) / intBeatSnapDivisorValue[intBeatSnapDivisor];
                        }

                        if (CheckNotePlacementValidity(pos, time, type, length))
                        {
                            if (intChartGameType != 3)
                            {
                                foreach (Creator_Note x in listNoteCatchPool)
                                {
                                    Creator_Note sameNote = null;
                                    Creator_Note oppositeNote = null;
                                    float dist = Mathf.Abs(time - x.transform.position.y);
                                    if (dist < 0.01f)
                                    {
                                        switch (type)
                                        {
                                            case 0: if (x.type == 1) oppositeNote = x; break;
                                            case 1: if (x.type == 0) oppositeNote = x; break;
                                            case 2: if (x.type == 3) oppositeNote = x; break;
                                            case 3: if (x.type == 2) oppositeNote = x; break;
                                        }
                                    }
                                    if (x.length > 0.01f)
                                    {
                                        dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                                        if (dist < 0.01f)
                                        {
                                            switch (type)
                                            {
                                                case 0: if (x.type == 1) oppositeNote = x; break;
                                                case 1: if (x.type == 0) oppositeNote = x; break;
                                                case 2: if (x.type == 3) oppositeNote = x; break;
                                                case 3: if (x.type == 2) oppositeNote = x; break;
                                            }
                                        }
                                        if (time < x.transform.position.y + x.length - Mathf.Epsilon && time > x.transform.position.y + Mathf.Epsilon)
                                        {
                                            if ((type == 0 || type == 1) && (x.type == 0 || x.type == 1))
                                            {
                                                if (type == x.type) sameNote = x;
                                                else oppositeNote = x;
                                            }
                                            if ((type == 2 || type == 3) && (x.type == 2 || x.type == 3))
                                            {
                                                if (type == x.type) sameNote = x;
                                                else oppositeNote = x;
                                            }
                                        }
                                    }

                                    if (sameNote != null)
                                    {
                                        pos = x.transform.position.x;
                                        break;
                                    }
                                    if (oppositeNote != null)
                                    {
                                        pos = -x.transform.position.x;
                                        break;
                                    }
                                }
                            }
#if UNITY_EDITOR
                            //Debug.Log("Note move: pos " + pos.ToString("f3") + ", time " + time.ToString("f3") + ", type " + type.ToString() + ", length " + length.ToString("f3"));
#endif
                            if (!draggedNote.isNoteTap)
                            {
                                CreateNoteCatch(pos, time, type, 0, length, speed, draggedNote.other);
                            }
                            else
                            {
                                CreateNoteTap(time, type, 0, length, speed, draggedNote.other);
                            }
                            PlaySound(clipTick);
                            DeleteNote(draggedNote);
                            CalculateChartLevel();
                        }
                        else
                        {
                            draggedNote.transform.position += Vector3.up * (oldPosY + 1f);
                            PlaySound(clipCancel);
                        }
                    }
                }
            }
        }

        // Right-click to delete a note
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(1))
#else
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetMouseButtonDown(1))
#endif
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 15f))
            {
                if (hit.collider.tag == "Creator_Note")
                {
                    DeleteNote(hit.collider.GetComponent<Creator_Note>());
                    PlaySound(clipNoteDelete);
                    CalculateChartLevel();
                }
            }
        }
        
        // Open online manual
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Notification.Display(Translator.GetStringTranslation("CREATOR_OPTIONSMANUALNOTIFICATION", "Atempting to open the online chart editor manual with your default browser..."), Color.white);
            Application.OpenURL("https://docs.google.com/document/d/17A4IUbQejGyEl5mMLZwad12J05k_JhxREVYLD0zOjpk/edit?usp=sharing");
        }
    }

    void LateUpdate()
    {
        // Cursor cannot be positioned before the start of the chart
        if (floatCursorPosition < 0) floatCursorPosition = 0;

        // Camera manipulation
        objectCentralPivot.transform.position = Vector3.Lerp(objectCentralPivot.transform.position, Vector3.up * floatCursorPosition, Time.deltaTime * 16f);
        cameraMain.orthographicSize = Mathf.Lerp(cameraMain.orthographicSize, cameraSizeMin + ((cameraSizeMax - cameraSizeMin) * sliderZoom.value), Time.deltaTime * 8f);
    }

    private bool CheckNotePlacementValidity(float pos, float time, int type, float length)
    {
        // Any game mode that isn't Note Dodge
        if (intChartGameType != 3)
        {
            // If the new note is too close to another note of the same type, don't create
            if (type < 4)
            {
                // Catch note
                foreach (Creator_Note x in listNoteCatchPool)
                {
                    if (x.gameObject.activeSelf && type == x.type)
                    {
                        float dist = Mathf.Abs(time - x.transform.position.y);
                        if (dist < 0.01f)
                        {
                            return false;
                        }
                        if (x.length > 0.01f)
                        {
                            dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                            if (dist < 0.01f)
                            {
                                return false;
                            }
                        }

                        // Same for long note ends
                        if (length > 0.01f)
                        {
                            dist = Mathf.Abs(time + length - x.transform.position.y);
                            if (dist < 0.01f)
                            {
                                return false;
                            }
                            if (x.length > 0.01f)
                            {
                                dist = Mathf.Abs(time + length - (x.transform.position.y + x.length));
                                if (dist < 0.01f)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Tap note
                foreach (Creator_Note x in listNoteTapPool)
                {
                    if (x.gameObject.activeSelf)
                    {
                        float dist = Mathf.Abs(time - x.transform.position.y);
                        if (dist < 0.01f)
                        {
                            return false;
                        }
                        if (x.length > 0.01f)
                        {
                            dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                            if (dist < 0.01f)
                            {
                                return false;
                            }
                        }

                        if (length > 0.01f)
                        {
                            dist = Mathf.Abs(time + length - x.transform.position.y);
                            if (dist < 0.01f)
                            {
                                return false;
                            }
                            if (x.length > 0.01f)
                            {
                                dist = Mathf.Abs(time + length - (x.transform.position.y + x.length));
                                if (dist < 0.01f)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }
        // Note Dodge game mode only
        else
        {
            // If the new note is too close to another note of the same type, don't create
            if (type < 4)
            {
                // Catch note
                foreach (Creator_Note x in listNoteCatchPool)
                {
                    if (x.gameObject.activeSelf && type == x.type)
                    {
                        float dist = Mathf.Abs(time - x.transform.position.y);
                        float distAlt = Mathf.Abs(pos - x.transform.position.x);
                        if (dist < 0.01f && distAlt < 0.01f)
                        {
                            return false;
                        }
                        
                        if (length > 0.01f)
                        {
                            dist = Mathf.Abs(time + length - x.transform.position.y);
                            if (dist < 0.01f && distAlt < 0.01f)
                            {
                                return false;
                            }
                            if (x.length > 0.01f)
                            {
                                dist = Mathf.Abs(time + length - (x.transform.position.y + x.length));
                                if (dist < 0.01f && distAlt < 0.01f)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Tap note
                foreach (Creator_Note x in listNoteTapPool)
                {
                    if (x.gameObject.activeSelf)
                    {
                        float dist = Mathf.Abs(time - x.transform.position.y);
                        if (dist < 0.01f)
                        {
                            return false;
                        }
                        if (x.length > 0.01f)
                        {
                            dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                            if (dist < 0.01f)
                            {
                                return false;
                            }
                        }

                        if (length > 0.01f)
                        {
                            dist = Mathf.Abs(time + length - x.transform.position.y);
                            if (dist < 0.01f)
                            {
                                return false;
                            }
                            if (x.length > 0.01f)
                            {
                                dist = Mathf.Abs(time + length - (x.transform.position.y + x.length));
                                if (dist < 0.01f)
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }
        }

        return true;
    }
}
