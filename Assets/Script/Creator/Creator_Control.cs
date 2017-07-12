﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_Control : MonoBehaviour
{
    public Camera cameraMain;
    public Canvas canvasCreatorSetting;

    public float holdDelay = 0.3f;
    private float holdDelayCurrent = 0f;

    public Slider sliderZoom;
    public float cameraSizeMin = 3f;
    public float cameraSizeMax = 10f;

    public Text textFileName;
    public Text textSongName;
    public Text textSongArtist;
    public Text textChartDeveloper;
    public Text textChartDescription;
    public Text textSongTempo;
    public Text textSongLength;
    public Text textChartJudge;

    public Text textTimeCurrentMeasure;
    public Text textTimeCurrentLength;
    public Text textBeatSnapDivisor;

    public Text textChartGameType;
    public Text textNotePlacementType;
    public Text textNoteLength;
    public Text textNoteOther;
    private List<string> listStringNoteOther = new List<string>();

    public Text textMouseScrollSetting;

    public ChartData chartData;

    private List<Creator_Note> listNotePool = new List<Creator_Note>();
    public Creator_Note objectNotePrefab;
    public Color[] colorNote = { Color.blue, Color.red, Color.green, Color.yellow };

    private List<Creator_SpecialEffect> listSpecialEffectPool = new List<Creator_SpecialEffect>();
    public Creator_SpecialEffect objectSpecialEffectPrefab;

    private List<GameObject> listObjectBeatSnapDivisorGuide = new List<GameObject>();
    public GameObject objectBeatSnapDivisorGuidePrefab;

    private int intMouseScrollSetting = 0;
    public string[] stringMouseScrollSetting = { "Move Chart", "Zoom Chart", "Change Note Type"};
    private int intChartGameType = 0;
    public string[] stringChartGameType = { "Linear", "Double", "Quad", "Powerful" };
    private int intNotePlacementType = 0;
    public string[] stringNotePlacementType = { "Blue (Btm)", "Red (Top)", "Green (Lt)", "Yellw (Dn)" };
    private int intBeatSnapDivisor = 0;
    public int[] intBeatSnapDivisorValue = { 2, 3, 4, 6, 8 };
    private int intCursorPosition = 0;

    void Start()
    {
        canvasCreatorSetting.gameObject.SetActive(false);

        textFileName.text = PlayerPrefs.GetString("creator_textFileName", "");
        intMouseScrollSetting = PlayerPrefs.GetInt("creator_intMouseScrollSetting", 0);
        intBeatSnapDivisor = PlayerPrefs.GetInt("creator_intBeatSnapDivisor", 0);
        sliderZoom.value = PlayerPrefs.GetFloat("creator_sliderZoom", 0.5f);

        RefreshBeatSnapDivisorGuide();
    }

    /// <summary>
    /// Write chart information into a text file.
    /// </summary>
    /// <param name="location"></param>
    public void SaveChart(string location)
    {
        Creator_Note[] listNoteC = FindObjectsOfType<Creator_Note>();
        if (listNoteC.Length < 1)
        {
            return;
        }

        chartData.listNoteInfo.Clear();
        chartData.listSpecialEffectInfo.Clear();

        chartData.songName = textSongName.text;
        chartData.songArtist = textSongArtist.text;
        chartData.chartDeveloper = textChartDeveloper.text;
        chartData.chartDescription = textChartDescription.text;
        chartData.songLength = 0;
        float.TryParse(textSongLength.text, out chartData.songLength);
        chartData.songTempo = 60f;
        float.TryParse(textSongTempo.text, out chartData.songTempo);
        chartData.chartGameType = intChartGameType;
        chartData.chartJudge = int.Parse(textChartJudge.text);
        int.TryParse(textChartJudge.text, out chartData.chartJudge);
        
        NoteInfo newNote = new NoteInfo();
        foreach (Creator_Note x in listNoteC)
        {
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
            newNote.other = x.other;
            chartData.listNoteInfo.Add(newNote);

            if (newNote.position > chartData.songLength - 2.5f)
            {
                chartData.songLength = newNote.position + 2.5f;
                textSongLength.text = (newNote.position + 2.5f).ToString();
            }
        }

        string output = JsonUtility.ToJson(chartData);

        // TODO: output to file
    }

    /// <summary>
    /// Load chart information from an external text file.
    /// </summary>
    /// <param name="location"></param>
    public void LoadChart(string location)
    {
        ClearChart();

        string input = "";
        // TODO: load file

        JsonUtility.FromJsonOverwrite(input, chartData);

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
            DeleteNote(x);
        }
    }

    /// <summary>
    /// Create a note using the chart's note data.
    /// </summary>
    /// <param name="note"></param>
    public void CreateNote(NoteInfo note)
    {
        // Object pooling - try to get an unused object from the pool, and make a new object if there are no unused ones
        bool createNote = true;
        Creator_Note newNote = null;
        foreach (Creator_Note x in listNotePool)
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
            newNote = Instantiate(objectNotePrefab);
            listNotePool.Add(newNote);
        }

        // Modify note to use information given
        newNote.transform.position = new Vector3(note.position, note.time);
        newNote.type = note.type;
        newNote.size = note.size;
        // Long note visualization
        if (note.length > 0.01f)
        {
            newNote.length = note.length;
            newNote.spriteRendererLength.gameObject.SetActive(true);
            newNote.spriteRendererLength.transform.localPosition = Vector3.up * note.length * 0.5f;
            newNote.spriteRendererLength.transform.localScale = new Vector3(
                newNote.spriteRendererLength.transform.localScale.x,
                25f * note.length,
                newNote.spriteRendererLength.transform.localScale.z);
        }
        else
        {
            newNote.length = 0;
            newNote.spriteRendererLength.gameObject.SetActive(false);
        }
        newNote.other = note.other;

        // Colorize note based on type
        if (colorNote.Length > note.type)
        {
            newNote.GetComponent<SpriteRenderer>().color = colorNote[note.type];

            Color dim = colorNote[note.type];
            dim.r *= 0.7f;
            dim.g *= 0.7f;
            dim.b *= 0.7f;
            newNote.spriteRendererLength.color = dim;
        }

        // Update note's text mesh
        switch (newNote.type)
        {
            default:
                newNote.textMeshNoteType.gameObject.SetActive(false);
                break;
            case 0:
                newNote.textMeshNoteType.text = "1";
                newNote.textMeshNoteType.anchor = TextAnchor.LowerRight;
                break;
            case 1:
                newNote.textMeshNoteType.text = "2";
                newNote.textMeshNoteType.anchor = TextAnchor.LowerLeft;
                break;
            case 2:
                newNote.textMeshNoteType.text = "3";
                newNote.textMeshNoteType.anchor = TextAnchor.UpperRight;
                break;
            case 3:
                newNote.textMeshNoteType.text = "4";
                newNote.textMeshNoteType.anchor = TextAnchor.UpperLeft;
                break;
            case 4:
                newNote.textMeshNoteType.text = "SHAKE";
                newNote.textMeshNoteType.anchor = TextAnchor.MiddleCenter;
                break;
        }

        // Make note active
        newNote.gameObject.SetActive(true);
    }
    /// <summary>
    /// Create a new custom note from scratch.
    /// </summary>
    /// <param name="position">Note's horizontal position (right is positive). If this note would be used for the vertical catchers, this would be used for the vertical position instead (up is positive).</param>
    /// <param name="time">Note's time. Determines when the note will pass the catcher's position.</param>
    /// <param name="type">Note's type. Determines which catcher it will appear for.</param>
    /// <param name="size">Note's size. Larger sizes make note judgment easier. Also affects note's appearance. The default value is 0 and cannot be lower than that.</param>
    /// <param name="other">Note's additional attributes.</param>
    public void CreateNote(float position, float time, int type = 0, int size = 0, float length = 0f, List<string> other = null)
    {
        NoteInfo newNote = new NoteInfo();
        newNote.position = position;
        newNote.time = time;
        newNote.type = type;
        newNote.size = size;
        newNote.length = length;
        if (other == null)
        {
            newNote.other = new List<string>();
        }
        else
        {
            newNote.other = other;
        }

        CreateNote(newNote);
    }

    /// <summary>
    /// Change the game type ID for this chart.
    /// </summary>
    /// <param name="modifier">The additive value on the game type ID.</param>
    public void ChartGameTypeChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        intChartGameType += modifier;
        if (intChartGameType < 0)
        {
            intChartGameType = stringChartGameType.Length - 1;
        }
        if (intChartGameType >= stringChartGameType.Length)
        {
            intChartGameType = 0;
        }
    }

    public void AddNoteEffect(Text text)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        if (listStringNoteOther.Count >= 8)
        {
            return;
        }
        listStringNoteOther.Add(text.text);
    }
    public void ClearOneNoteEffect(Text text)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        listStringNoteOther.Remove(text.text);
    }
    public void ClearAllNoteEffect()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        listStringNoteOther.Clear();
    }

    /// <summary>
    /// Change the current note placement type.
    /// </summary>
    /// <param name="modifier">The additive value on the note type ID.</param>
    public void ChartNotePlacementTypeChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

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

    public void MouseScrollSettingChange(int modifier)
    {
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

    public void BeatSnapDivisorChange(int modifier)
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) return;

        intBeatSnapDivisor += modifier;
        if (intBeatSnapDivisor < -1)
        {
            intBeatSnapDivisor = -1;
        }
        if (intBeatSnapDivisor >= intBeatSnapDivisorValue.Length)
        {
            intBeatSnapDivisor = intBeatSnapDivisorValue.Length - 1;
        }
        RefreshBeatSnapDivisorGuide();
    }

    public void RefreshBeatSnapDivisorGuide()
    {
        foreach (GameObject x in listObjectBeatSnapDivisorGuide)
        {
            x.SetActive(false);
        }

        if (intBeatSnapDivisor >= 0)
        {
            GameObject line = null;
            for (int i = 0; i <= 5; i++)
            {
                for (float f = 0; f < 1 - Mathf.Epsilon; f += 1f / intBeatSnapDivisorValue[intBeatSnapDivisor])
                {
                    if (f < Mathf.Epsilon) continue;

                    line = GetObjectBeatSnapDivisorGuide();
                    line.transform.parent = cameraMain.transform;
                    line.transform.localPosition = Vector3.up * (f + i);
                    line.SetActive(true);
                    line = GetObjectBeatSnapDivisorGuide();
                    line.transform.parent = cameraMain.transform;
                    line.transform.localPosition = Vector3.down * (f + i);
                    line.SetActive(true);
                }
            }
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

    /// <summary>
    /// Despawns a note.
    /// </summary>
    /// <param name="note">The note to be despawned.</param>
    public void DeleteNote(Creator_Note note)
    {
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

        canvasCreatorSetting.gameObject.SetActive(!canvasCreatorSetting.gameObject.activeSelf);
    }

    public void SaveCreatorSettings()
    {
        PlayerPrefs.SetString("creator_textFileName", textFileName.text);
        PlayerPrefs.SetInt("creator_intMouseScrollSetting", intMouseScrollSetting);
        PlayerPrefs.SetInt("creator_intBeatSnapDivisor", intBeatSnapDivisor);
        PlayerPrefs.SetFloat("creator_sliderZoom", sliderZoom.value);

        PlayerPrefs.Save();
    }

    private void FixedUpdate()
    {
        // Display cursor position
        float songTempo = 0;
        textTimeCurrentMeasure.text = "Measure " + ((intCursorPosition / 4) + 1).ToString() + ", Beat " + ((intCursorPosition % 4) + 1).ToString();
        if (float.TryParse(textSongTempo.text, out songTempo))
        {
            textTimeCurrentLength.text = "Song Pos " + (Mathf.FloorToInt(60f / songTempo * intCursorPosition / 60f)).ToString("0") + ":" + (60f / songTempo * intCursorPosition % 60f).ToString("00.00");
        }
        else
        {
            textTimeCurrentLength.text = "Song Pos UNKNOWN";
        }
        if (intBeatSnapDivisor >= 0)
        {
            textBeatSnapDivisor.text = "1/" + intBeatSnapDivisorValue[intBeatSnapDivisor].ToString();
        }
        else
        {
            textBeatSnapDivisor.text = "FREE";
        }

        // Current selection
        textChartGameType.text = stringChartGameType[intChartGameType];
        textNotePlacementType.text = stringNotePlacementType[intNotePlacementType];
        textNoteOther.text = "";
        foreach (string s in listStringNoteOther)
        {
            textNoteOther.text += s + "\n";
        }

        textMouseScrollSetting.text = stringMouseScrollSetting[intMouseScrollSetting];
    }

    private void Update()
    {
        holdDelayCurrent -= Time.deltaTime;

        // Cursor movement by input
        if (Input.GetKey(KeyCode.DownArrow) && holdDelayCurrent < 0f)
        {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                intCursorPosition -= 16;
            }
            else
            {
                intCursorPosition--;
            }
            holdDelayCurrent = holdDelay;
        }
        if (Input.GetKey(KeyCode.UpArrow) && holdDelayCurrent < 0f)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                intCursorPosition += 16;
            }
            else
            {
                intCursorPosition++;
            }
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
                    intCursorPosition++;
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    intCursorPosition--;
                }
                break;
            case 1:
                if (Input.GetAxis("MouseScrollWheel") > Mathf.Epsilon)
                {
                    intCursorPosition--;
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
                    intCursorPosition++;
                }
                break;
            case 2:
                if (Input.GetAxis("MouseScrollWheel") > Mathf.Epsilon)
                {
                    sliderZoom.value = Mathf.Clamp01(sliderZoom.value - 0.08f);
                }
                if (Input.GetAxis("MouseScrollWheel") < -Mathf.Epsilon)
                {
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

        // Left-click to place a note
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
#else
        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftCommand)) && Input.GetMouseButtonDown(0))
#endif
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 15f))
            {
                // Possible to create note only if note is at or above song start
                if (hit.point.y > -Mathf.Epsilon)
                {
                    bool createNote = true;
                    float pos = hit.point.x;
                    float time = hit.point.y;
                    int type = intNotePlacementType;
                    float length = 0f;
                    float.TryParse(textNoteLength.text, out length);

                    // Left shift snaps horizontal position to nearest quarter from the center
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        pos = Mathf.Round(4f * pos) / 4f;
                    }
                    // If there is a beat snap divisor, it will be applied
                    if (intBeatSnapDivisor >= 0)
                    {
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
                    }

                    // If the new note is too close to another note of the same type, don't create
                    foreach (Creator_Note x in listNotePool)
                    {
                        if (x.gameObject.activeSelf && type == x.type)
                        {
                            //float dist = Vector3.Distance(new Vector3(hit.point.x, hit.point.y), x.transform.position);
                            float dist = Mathf.Abs(time - x.transform.position.y);
                            if (dist < 0.01f)
                            {
                                createNote = false;
                                break;
                            }
                            if (x.length > 0.01f)
                            {
                                dist = Mathf.Abs(time - (x.transform.position.y + x.length));
                                if (dist < 0.01f)
                                {
                                    createNote = false;
                                    break;
                                }
                            }
                        }
                    }

                    if (createNote)
                    {
#if UNITY_EDITOR
                        Debug.Log("Note creation: pos " + pos.ToString("f3") + ", time " + time.ToString("f3") + ", type " + type.ToString() + ", length " + length.ToString("f3"));
#endif
                        CreateNote(pos, time, type, 0, length, listStringNoteOther);
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

            if (Physics.Raycast(ray, out hit, 15f) && hit.collider.tag == "Creator_Note")
            {
                DeleteNote(hit.collider.GetComponent<Creator_Note>());
            }
        }

        // Cursor cannot be positioned before the start of the chart
        if (intCursorPosition < 0) intCursorPosition = 0;

        // Camera manipulation
        cameraMain.transform.position = Vector3.Lerp(cameraMain.transform.position, Vector3.up * intCursorPosition, Time.deltaTime * 16f);
        cameraMain.orthographicSize = Mathf.Lerp(cameraMain.orthographicSize, cameraSizeMin + ((cameraSizeMax - cameraSizeMin) * sliderZoom.value), Time.deltaTime * 8f);
    }
}