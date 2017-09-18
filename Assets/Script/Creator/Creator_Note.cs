using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creator_Note : MonoBehaviour
{
    public SpriteRenderer spriteRendererLength;
    public SpriteRenderer spriteRendererLengthEndNote;
    public TextMesh textMeshNoteType;
    public TextMesh textMeshNoteOther;

    public int type = 0;
    public int size = 0;
    public float length = 0f;
    public float speed = 0f;
    public List<string> other = new List<string>();

    public bool isNoteTap = false;
}
