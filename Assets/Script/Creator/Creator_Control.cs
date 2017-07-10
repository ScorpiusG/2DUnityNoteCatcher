using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_Control : MonoBehaviour
{
    public Camera cameraMain;

    public float holdDelay = 0.3f;
    private float holdDelayCurrent = 0f;

    public Slider sliderZoom;
    public float cameraSizeMin = 3f;
    public float cameraSizeMax = 10f;

    public Text textSongName;
    public Text textSongArtist;
    public Text textChartDeveloper;
    public Text textChartDescription;
    public Text textSongTempo;
    public Text textSongLength;
    public Text textChartJudge;

    public Text textTimeCurrentMeasure;
    public Text textTimeCurrentLength;

    public Text textChartGameType;

    public ChartData chartData;

    public Creator_Note objectNotePrefab;
    public Color[] colorNote = { Color.blue, Color.red, Color.green, Color.yellow };

    private int intMouseScrollSetting = 0;
    private int intChartGameType = 0;
    public string[] stringChartGameType = { "Linear", "Double", "Quad", "Powerful" };
    private int intNotePlacementType = 0;
    private int intCursorPosition = 0;

    void Start()
    {
    }

    /// <summary>
    /// Write chart information into a text file.
    /// </summary>
    /// <param name="location"></param>
    public void SaveChart(string location)
    {
        chartData.songName = textSongName.text;
        chartData.songArtist = textSongArtist.text;
        chartData.chartDeveloper = textChartDeveloper.text;
        chartData.chartDescription = textChartDescription.text;
        
        Creator_Note[] listNoteC = FindObjectsOfType<Creator_Note>();
        NoteInfo newNote = new NoteInfo();
        foreach (Creator_Note x in listNoteC)
        {
            newNote.time = x.transform.position.y;
            newNote.position = x.transform.position.x;
            newNote.type = x.type;
            newNote.size = x.size;
            newNote.special = x.special;
            chartData.listNoteInfo.Add(newNote);
        }

        // TODO: output to file
    }

    /// <summary>
    /// Load chart information from an external text file.
    /// </summary>
    /// <param name="location"></param>
    public void LoadChart(string location)
    {
        ClearChart();

        // TODO: load file

        foreach (NoteInfo x in chartData.listNoteInfo)
        {
            CreateNote(x);
        }
    }

    /// <summary>
    /// Completely wipes chart to start over from scratch.
    /// </summary>
    public void ClearChart()
    {
        textSongName.text = "";
        textSongArtist.text = "";
        textChartDeveloper.text = "";
        textChartDescription.text = "";
        textSongTempo.text = "60.00";
        textSongLength.text = "10";
        textChartJudge.text = "";

        Creator_Note[] listNoteC = FindObjectsOfType<Creator_Note>();
        foreach (Creator_Note x in listNoteC)
        {
            Destroy(x.gameObject);
        }
    }

    /// <summary>
    /// Create a note using the chart's note data.
    /// </summary>
    /// <param name="note"></param>
    public void CreateNote(NoteInfo note)
    {
        Creator_Note newNote = Instantiate(objectNotePrefab);
        newNote.transform.position = new Vector3(note.position, note.time);
        newNote.type = note.type;
        newNote.size = note.size;
        newNote.special = note.special;

        if (colorNote.Length > note.type)
        {
            newNote.GetComponent<SpriteRenderer>().color = colorNote[note.type];
        }
    }
    /// <summary>
    /// Create a new custom note.
    /// </summary>
    /// <param name="position">Note's horizontal position (right is positive). If this note would be used for the vertical catchers, this would be used for the vertical position instead (up is positive).</param>
    /// <param name="time">Note's time. Determines when the note will pass the catcher's position.</param>
    /// <param name="type">Note's type. Determines which catcher it will appear for.</param>
    /// <param name="size">Note's size. Larger sizes make note judgment easier. Also affects note's appearance. The default value is 0 and cannot be lower than that.</param>
    /// <param name="special">Note's additional attributes.</param>
    public void CreateNote(float position, float time, int type = 0, int size = 0, List<string> special = null)
    {
        NoteInfo newNote = new NoteInfo();
        newNote.position = position;
        newNote.time = time;
        newNote.type = type;
        newNote.size = size;
        newNote.special = special;
    }

    /// <summary>
    /// Change the game type ID for this chart.
    /// </summary>
    /// <param name="modifier">The additive value on the game type ID.</param>
    public void ChartGameTypeChange(int modifier)
    {
        intChartGameType += modifier;
        while (intChartGameType < 0)
        {
            intChartGameType += stringChartGameType.Length;
        }
        while (intChartGameType >= stringChartGameType.Length)
        {
            intChartGameType -= stringChartGameType.Length;
        }
    }

    private void Update()
    {
        holdDelayCurrent -= Time.deltaTime;

        // Cursor movement
        if (Input.GetKey(KeyCode.DownArrow) && holdDelayCurrent < 0f)
        {
            intCursorPosition--;
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.UpArrow) && holdDelayCurrent < 0f)
        {
            intCursorPosition++;
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.LeftArrow) && holdDelayCurrent < 0f)
        {
            intCursorPosition -= 4;
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.RightArrow) && holdDelayCurrent < 0f)
        {
            intCursorPosition += 4;
            holdDelayCurrent = holdDelay;
        }

        if (intCursorPosition < 0) intCursorPosition = 0;

        // Camera manipulation
        cameraMain.transform.position = Vector3.Lerp(cameraMain.transform.position, Vector3.up * intCursorPosition, Time.deltaTime * 16f);
        cameraMain.orthographicSize = Mathf.Lerp(cameraMain.orthographicSize, cameraSizeMin + ((cameraSizeMax - cameraSizeMin) * sliderZoom.value), Time.deltaTime * 8f);

        // Display cursor position
        float songTempo = 0;
        textTimeCurrentMeasure.text = "Measure " + intCursorPosition.ToString();
        if (float.TryParse(textSongTempo.text, out songTempo))
        {
            textTimeCurrentLength.text = "Length " + (60f / songTempo * intCursorPosition).ToString("f2");
        }
        else
        {
            textTimeCurrentLength.text = "Length Error";
        }

        // Other text manipluation
        textChartGameType.text = stringChartGameType[intChartGameType];
    }
}
