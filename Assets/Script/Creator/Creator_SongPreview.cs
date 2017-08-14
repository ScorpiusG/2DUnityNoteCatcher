using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Creator_SongPreview : MonoBehaviour
{
    public static Creator_SongPreview mSongPreview;
    private AudioSource mAudioSource;
    private Coroutine coroutinePlaying;

    void Awake()
    {
        mSongPreview = this;
        mAudioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Previews a specific part of the song on the .ogg if it exists. If it doesn't exist, it will use preview.ogg instead.
    /// </summary>
    public void Preview()
    {
        if (mAudioSource.clip == null)
        {
            StartCoroutine("LoadClip");
        }
        else
        {
            StopCoroutine(coroutinePlaying);
            coroutinePlaying = StartCoroutine("PlaySection");
        }
    }

    private IEnumerator PlaySection()
    {
        // Get current song positions depending on where the cursor is at
        float currentSecond = float.Parse(Creator_Control.control.textSongTempo.text) / 60f * Creator_Control.control.intCursorPosition;
        float endSecond = float.Parse(Creator_Control.control.textSongTempo.text) / 60f * (Creator_Control.control.sliderSongPreviewLength.value + Creator_Control.control.intCursorPosition);
        float fadeDuration = Creator_Control.control.sliderSongPreviewFade.value;

        yield return null;

        // Play the (hopefully) loaded clip
        mAudioSource.Play();
        mAudioSource.time = currentSecond;
        mAudioSource.volume = PlayerSetting.setting.floatVolumeMusic;

        // Preview area
        while (mAudioSource.isPlaying && mAudioSource.time < endSecond)
        {
            yield return null;
        }

        // Fade out
        while (mAudioSource.isPlaying && mAudioSource.volume > Mathf.Epsilon)
        {
            mAudioSource.volume -= Time.deltaTime / 3f;
            yield return null;
        }
        mAudioSource.Stop();
    }
    private IEnumerator LoadClip()
    {
        string url = "file://" + Directory.GetCurrentDirectory() + "/MyCharts/" + Creator_Control.control.textFileName.text + ".ogg";
        if (!File.Exists(url))
        {
            url = "file://" + Directory.GetCurrentDirectory() + "/MyCharts/preview.ogg";
            if (!File.Exists(url))
            {
                Debug.LogError("ERROR: Neither " + Creator_Control.control.textFileName.text + ".ogg nor preview.ogg exists in the MyCharts folder.");
                yield break;
            }
        }

        WWW www = new WWW(url);
        while (!www.isDone)
        {
            yield return null;
        }
        mAudioSource.clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
        Preview();
    }
    /// <summary>
    /// Nullifies this audio source's clip.
    /// </summary>
    public void ClearClip()
    {
        mAudioSource.clip = null;
    }
}
