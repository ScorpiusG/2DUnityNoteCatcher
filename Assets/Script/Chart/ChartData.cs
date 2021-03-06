﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ChartData : ScriptableObject
{
    // Main chart variables
    public string songName = "";
    public string songArtist = "";
    public string chartDeveloper = "";
    public string chartDescription = "";
    public int chartVersion = 0;
    public int chartLevel = 0;
    public int chartGameType = 0;
    public float songTempo = 60f;
    public float songLength = 10f;
    public float gameplayLength = 0f;
    public int chartOffset = 0;
    public int chartJudge = 0;
    public bool isHighScoreAllowed = true;
    public bool isModifierAllowed = true;

    public List<string> listNoteCatchInfo;
    public List<string> listNoteTapInfo;
    public List<string> listSpecialEffectInfo;
    
    public List<Vector3> listTempoChanges;

    // Chart editor variables
    public int beatsPerMeasure = 4;

    // General subclasses
    public class NoteInfo : ScriptableObject
    {
        public int type = 0;
        public int size = 0;
        public float time = 0f;
        public float position = 0f;
        public float length = 0f;
        public float speed = 0f;
        public List<string> other;
    }
    
    public class SpecialEffectInfo : ScriptableObject
    {
        public int type = 0;
        public int intensity = 0;
        public float time = 0f;
        public float duration = 0f;
    }
}
