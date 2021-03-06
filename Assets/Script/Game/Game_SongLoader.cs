﻿using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_SongLoader : MonoBehaviour
{
    public static Game_SongLoader loader;

    public AudioSource audioSourceMusic;
    public AudioSource audioSourceEffect;

    public float floatSongLoadDelay = 2f;

    public AudioClip clipSong = null;
    public List<AudioClip> listClipEffect = new List<AudioClip>();

    public FFTWindow audioSpectrumType = FFTWindow.Blackman;
    public Image[] imageAudioVisualLeft;
    public Image[] imageAudioVisualRight;
    private float[] floatAudioVisualMusic = new float[64];
    private float[] floatAudioVisualEffect = new float[64];

    private void Awake()
    {
        loader = this;
    }

    private void Start()
    {
        audioSourceMusic.volume = PlayerSetting.setting.floatVolumeMusic;

        foreach (Image x in imageAudioVisualLeft)
        {
            x.gameObject.SetActive(PlayerSetting.setting.enableAudioVisualizer);
            x.fillAmount = 0f;
        }
        foreach (Image x in imageAudioVisualRight)
        {
            x.gameObject.SetActive(PlayerSetting.setting.enableAudioVisualizer);
            x.fillAmount = 0f;
        }

        StartCoroutine("LoadClipMusic", true);
        StartCoroutine("LoadClipEffect", true);
    }

    public void ManualLoadClipMusic()
    {
        StartCoroutine("LoadClipMusic");
    }

    public void ManualLoadClipEffect()
    {
        StartCoroutine("LoadClipEffect");
    }

    private void FixedUpdate()
    {
        if (!PlayerSetting.setting.enableAudioVisualizer)
        {
            return;
        }

        if (!Game_Control.control.isForcedEnd && !Game_Control.control.boolIsPaused && audioSourceMusic.isPlaying)
        {
            audioSourceMusic.GetSpectrumData(floatAudioVisualMusic, 0, audioSpectrumType);
            audioSourceEffect.GetSpectrumData(floatAudioVisualEffect, 0, audioSpectrumType);
        }
        else
        {
            for (int i = 0; i < floatAudioVisualMusic.Length; i++) floatAudioVisualMusic[i] = 0f;
            for (int i = 0; i < floatAudioVisualEffect.Length; i++) floatAudioVisualEffect[i] = 0f;
        }

        for (int i = 0; i < imageAudioVisualLeft.Length; i++)
        {
            float fill = (floatAudioVisualMusic[Mathf.CeilToInt(1f * i * floatAudioVisualMusic.Length / imageAudioVisualLeft.Length) + 1]
                + floatAudioVisualEffect[Mathf.CeilToInt(1f * i * floatAudioVisualEffect.Length / imageAudioVisualLeft.Length) + 1])
                * Mathf.Pow(i + 1, 2) * 3f;
            if (fill > imageAudioVisualLeft[i].fillAmount)
            {
                imageAudioVisualLeft[i].fillAmount = fill;
                imageAudioVisualRight[i].fillAmount = fill;
            }
            else
            {
                imageAudioVisualLeft[i].fillAmount = Mathf.Lerp(imageAudioVisualLeft[i].fillAmount, 0f, Time.fixedDeltaTime * 8f);
                imageAudioVisualRight[i].fillAmount = Mathf.Lerp(imageAudioVisualRight[i].fillAmount, 0f, Time.fixedDeltaTime * 8f);
            }
        }
    }

    private IEnumerator LoadClipMusic(bool isDelayed = false)
    {
        if (isDelayed)
        {
            yield return new WaitForSecondsRealtime(floatSongLoadDelay);
        }

        // Load audio music file
        if (Game_Control.boolCustomSong)
        {
            string url = "Songs/" + Game_Control.stringSongFileName + "/" + Game_Control.stringSongFileName + ".ogg";
            if (!File.Exists(url))
            {
                Debug.LogError("ERROR: " + url + " does not exist.");
                //yield break;
            }

            WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + url);
#if UNITY_EDITOR
            Debug.Log(www.url);
            if (www.error != "")
            {
                Debug.Log("Error message from reading audio file: " + www.error);
            }
#endif
            while (!www.isDone)
            {
                yield return null;
            }
            clipSong = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
            while (clipSong == null)
            {
                yield return null;
            }
        }
        else
        {
            string path = "Songs/" + Game_Control.stringSongFileName + "/" + Game_Control.stringSongFileName;
            clipSong = Resources.Load(path) as AudioClip;
        }

        yield return null;

        audioSourceMusic.clip = clipSong;
    }
    
    private IEnumerator LoadClipEffect(bool isDelayed = false)
    {
        yield return null;
        if (isDelayed)
        {
            yield return new WaitForSecondsRealtime(floatSongLoadDelay);
        }

        // Load hitsound files
        for (int i = 0; true; i++)
        {
            if (Game_Control.boolCustomSong)
            {
                string url = "Songs/" + Game_Control.stringSongFileName + "/sound" + i.ToString() + ".ogg";
                if (!File.Exists(url))
                {
                    Debug.Log("WARNING: " + url + " does not exist.");
                    break;
                }

                WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + "/" + url);
                while (!www.isDone)
                {
                    yield return null;
                }
                if (www.error != "")
                {
#if UNITY_EDITOR
                    Debug.Log("Error message from reading audio file: " + www.error);
                    break;
#endif
                }

                AudioClip newSound = www.GetAudioClip(false, false, AudioType.OGGVORBIS);
                while (!www.isDone && newSound == null)
                {
                    yield return null;
                }
                if (newSound != null)
                {
                    listClipEffect.Add(newSound);
                }
                else
                {
                    break;
                }
            }
            else
            {
                string path = "Songs/" + Game_Control.stringSongFileName + "/sound" + i.ToString() + ".ogg";
                AudioClip newSound = Resources.Load(path) as AudioClip;
                if (newSound != null)
                {
                    listClipEffect.Add(newSound);
                }
                else
                {
                    break;
                }
            }
            yield return null;
        }

        yield return null;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        if (audioSourceEffect != null && clip != null)
        {
            audioSourceEffect.PlayOneShot(clip, PlayerSetting.setting.floatVolumeEffect);
        }
    }
}
