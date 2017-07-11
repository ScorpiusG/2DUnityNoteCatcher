using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class NoteInfo : MonoBehaviour
{
    public int type = 0;
    public int size = 0;
    public float time = 0f;
    public float position = 0f;
    public float length = 0f;
    public List<string> special = new List<string>();
}
