using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class ChartData : MonoBehaviour
{
    public string songName = "";
    public string songArtist = "";
    public string chartDeveloper = "";
    public string chartDescription = "";
    public int chartGameType = 0;
    public float songTempo = 60f;
    public float songLength = 10f;
    public int chartJudge = 0;
    public bool isHighScoreAllowed = true;
    public bool isModifierAllowed = true;
    public List<NoteInfo> listNoteInfo = new List<NoteInfo>();
}
