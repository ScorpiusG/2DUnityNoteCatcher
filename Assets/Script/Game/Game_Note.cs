using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Note : MonoBehaviour
{
    public int type = 0;
    public int size = 0;
    public float position = 0f;
    public float time = 0f;
    public float length = 0f;
    public List<string> other;

    public SpriteRenderer spriteRendererNote;
    public SpriteRenderer spriteRendererLength;
}
