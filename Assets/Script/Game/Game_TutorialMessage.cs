using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_TutorialMessage : MonoBehaviour
{
    private Game_SongLoader player;

    public Vector2 timeShowMessage = new Vector2(0f, 8f);

    private Image[] arrayImage;
    private Text[] arrayText;

    private float floatAlpha = 0f;

    private void Start()
    {
        player = Game_SongLoader.loader;

        arrayImage = GetComponentsInChildren<Image>();
        arrayText = GetComponentsInChildren<Text>();

        foreach (Image x in arrayImage)
        {
            x.gameObject.SetActive(false);
        }
        foreach (Text x in arrayText)
        {
            x.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (player.audioSourceMusic.time > timeShowMessage.x && player.audioSourceMusic.time < timeShowMessage.y)
        {
            floatAlpha += Time.deltaTime / 0.8f;

            foreach (Image x in arrayImage)
            {
                if (!x.gameObject.activeSelf) x.gameObject.SetActive(true);
            }
            foreach (Text x in arrayText)
            {
                if (!x.gameObject.activeSelf) x.gameObject.SetActive(true);
            }
        }
        else
        {
            floatAlpha -= Time.deltaTime / 0.8f;
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
            y.a = floatAlpha * 0.7f;
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
