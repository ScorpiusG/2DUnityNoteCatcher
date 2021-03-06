﻿using System;
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

    private bool boolLoadSongError = false;

    void Awake()
    {
        mSongPreview = this;
        mAudioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        //StartCoroutine("LoadClip");
    }

    /// <summary>
    /// Previews a specific part of the song on the .ogg if it exists. If it doesn't exist, it will use preview.ogg instead.
    /// </summary>
    public void Preview()
    {
#if UNITY_EDITOR
        Debug.Log("Song short preview.");
#endif
        boolLoadSongError = false;
        if (coroutinePlaying != null)
        {
            StopCoroutine(coroutinePlaying);
            coroutinePlaying = null;
        }
        coroutinePlaying = StartCoroutine(PlaySong());
    }
    /// <summary>
    /// Previews the whole song on the .ogg from where the cursor is.
    /// </summary>
    public void PreviewAll()
    {
#if UNITY_EDITOR
        Debug.Log("Whole song preview.");
#endif
        boolLoadSongError = false;
        if (coroutinePlaying != null)
        {
            StopCoroutine(coroutinePlaying);
            coroutinePlaying = null;
        }
        coroutinePlaying = StartCoroutine(PlaySongWhole());
    }

    private IEnumerator PlaySong()
    {
        if (mAudioSource.clip == null)
        {
            StartCoroutine("LoadClip");
            yield return new WaitWhile(() => mAudioSource.clip == null && !boolLoadSongError);

            if (boolLoadSongError)
            {
                Creator_Control.control.boolFullPreviewOngoing = false;
                yield break;
            }
        }

        // Get current song positions depending on where the cursor is at
        int chartOffset = 0;
        int.TryParse(Creator_Control.control.textChartOffset.text, out chartOffset);
        float cursorPos = Creator_Control.control.floatCursorPosition;
        float currentSecond = (float.Parse(Creator_Control.control.textSongTempo.text) / 60f * cursorPos * 0.25f) + (0.001f * chartOffset);
        float endSecond = currentSecond + Creator_Control.control.sliderSongPreviewLength.value;
        float fadeDuration = Creator_Control.control.sliderSongPreviewFade.value;

        yield return null;

        // Play the (hopefully) loaded clip
        mAudioSource.Play();
        mAudioSource.time = currentSecond;
        mAudioSource.volume = PlayerSetting.setting.floatVolumeMusic;

        yield return null;
        
        // Preview area
        while (mAudioSource.isPlaying && mAudioSource.time < endSecond)
        {
            yield return null;
        }

        // Fade out
        while (mAudioSource.isPlaying && mAudioSource.volume > Mathf.Epsilon)
        {
            mAudioSource.volume -= Time.deltaTime / fadeDuration;
            yield return null;
        }

        Creator_Control.control.boolFullPreviewOngoing = false;
        mAudioSource.Stop();
    }
    private IEnumerator PlaySongWhole()
    {
        if (mAudioSource.clip == null)
        {
            StartCoroutine("LoadClip");
            yield return new WaitWhile(() => mAudioSource.clip == null && !boolLoadSongError);

            if (boolLoadSongError)
            {
                Creator_Control.control.boolFullPreviewOngoing = false;
                yield break;
            }
        }

#if UNITY_EDITOR
        Debug.Log("PlaySongWhole");
#endif
        // Get current song positions depending on where the cursor is at
        int chartOffset = 0;
        int.TryParse(Creator_Control.control.textChartOffset.text, out chartOffset);
        //float cursorPos = Creator_Control.control.floatCursorPosition;
        float songBPM = float.Parse(Creator_Control.control.textSongTempo.text);
        float currentSecond = Creator_Control.control.GetCurrentPos() + (0.001f * chartOffset);
        //float currentSecond = ((60f / songBPM) * cursorPos) + (0.001f * chartOffset);

        Vector3 lastChange = new Vector3(0f, songBPM, 0f);

        yield return null;

        // Play the (hopefully) loaded clip
        mAudioSource.Play();
        mAudioSource.time = currentSecond;
        mAudioSource.volume = PlayerSetting.setting.floatVolumeMusic;

        yield return null;
        
        // Preview area
        while (mAudioSource.isPlaying)
        {
            if (Creator_Control.control.listTempoChange.Count > 0)
            {
                foreach (Vector3 x in Creator_Control.control.listTempoChange)
                {
                    if (Creator_Control.control.floatCursorPosition >= x.x - Mathf.Epsilon)
                    {
                        lastChange = x;
                    }
                }
            }

            if (lastChange.y > 0.01f)
            {
                songBPM = lastChange.y;
            }

            Creator_Control.control.floatCursorPosition = ((mAudioSource.time - (0.001f * chartOffset) - lastChange.z) * songBPM / 60f) + lastChange.x;
            if (Input.GetKeyDown(KeyCode.Space)) break;
            yield return null;
        }
#if UNITY_EDITOR
        Debug.Log("PlaySongWhole");
#endif

        Creator_Control.control.floatCursorPosition = Mathf.Floor(Creator_Control.control.floatCursorPosition);
        Creator_Control.control.boolFullPreviewOngoing = false;
        mAudioSource.Stop();
    }
    private IEnumerator LoadClip()
    {
        yield return null;

        string url = "Songs/" + Creator_Control.control.textFileName.text + "/" + Creator_Control.control.textFileName.text + ".ogg";
        if (!File.Exists(url))
        {
            Debug.LogError("ERROR: " + url + " does not exist.");
            Notification.Display("ERROR: The following song file does not exist:\n" + url, Color.red);
            boolLoadSongError = true;
            yield break;
        }
        
        WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + url);
#if UNITY_EDITOR
        Debug.Log("Load audio file path: " + www.url);
#endif
        while (!www.isDone)
        {
            yield return null;
        }
        mAudioSource.clip = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
        //Preview();
    }
    /// <summary>
    /// Nullifies this audio source's clip.
    /// </summary>
    public void ClearClip()
    {
        mAudioSource.clip = null;
    }
}
