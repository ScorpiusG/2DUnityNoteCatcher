using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Note : MonoBehaviour
{
    public float health = 1f;

    public int type = 0;
    public int size = 0;
    public float position = 0f;
    public float time = 0f;
    public float length = 0f;
    public float speed = 0f;
    public List<string> other;

    public SpriteRenderer spriteRendererNote;
    public SpriteRenderer spriteRendererNoteHighlight;
    public SpriteRenderer spriteRendererLength;
    public SpriteRenderer spriteRendererLengthHighlight;

    private void LateUpdate()
    {
        spriteRendererNote.gameObject.layer = gameObject.layer;
        spriteRendererNoteHighlight.gameObject.layer = gameObject.layer;
        spriteRendererLengthHighlight.gameObject.layer = gameObject.layer;
    }
}
