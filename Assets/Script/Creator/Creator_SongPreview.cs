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

    void Start()
    {
        //StartCoroutine("LoadClip");
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
            if (coroutinePlaying != null)
            {
                StopCoroutine(coroutinePlaying);
                coroutinePlaying = null;
            }
            coroutinePlaying = StartCoroutine("PlaySection");
        }
    }

    private IEnumerator PlaySection()
    {
        // Get current song positions depending on where the cursor is at
        int chartOffset = 0;
        int.TryParse(Creator_Control.control.textChartOffset.text, out chartOffset);
        float currentSecond = (float.Parse(Creator_Control.control.textSongTempo.text) / 60f * Creator_Control.control.intCursorPosition * 0.25f) + (0.001f * chartOffset);
        float endSecond = currentSecond + Creator_Control.control.sliderSongPreviewLength.value;
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
        yield return null;

        string url = "Songs/" + Creator_Control.control.textFileName.text + "/" + Creator_Control.control.textFileName.text + ".ogg";
        if (!File.Exists(url))
        {
            Debug.LogError("ERROR: " + url + " does not exist.");
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
