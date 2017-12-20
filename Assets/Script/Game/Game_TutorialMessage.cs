using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_TutorialMessage : MonoBehaviour
{
    public Game_SongLoader player;

    public Vector2 timeShowMessage = new Vector2(0f, 8f);

    public Image[] arrayImage;
    public Text[] arrayText;

    private float floatAlpha = 0f;

    private void Start()
    {
        player = Game_SongLoader.loader;
    }

    private void Update()
    {
        if (player.audioSourceMusic.time > timeShowMessage.x && player.audioSourceMusic.time < timeShowMessage.y)
        {
            floatAlpha += Time.deltaTime / 1.2f;
        }
        else
        {
            floatAlpha -= Time.deltaTime / 1.2f;
        }
        floatAlpha = Mathf.Clamp01(floatAlpha);

        if (floatAlpha < Mathf.Epsilon && player.audioSourceMusic.time > timeShowMessage.y)
        {
            Destroy(gameObject);
            return;
        }

        foreach (Image x in arrayImage)
        {
            Color y = x.color;
            y.a = floatAlpha;
            x.color = y;
        }
        foreach (Text x in arrayText)
        {
            Color y = x.color;
            y.a = floatAlpha;
            x.color = y;
        }
    }
}
