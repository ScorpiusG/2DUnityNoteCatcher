using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundEffect : MonoBehaviour
{
    private AudioSource mAudioSource;

    private float volumeSound = 0f;

    private void Awake()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        StartCoroutine("_Start");
    }
    private IEnumerator _Start()
    {
        yield return new WaitForSeconds(0.2f);
        volumeSound = 1f;
        while (true)
        {
            volumeSound += Time.deltaTime * 2f;
            volumeSound = Mathf.Clamp(volumeSound, 0.1f, 1f);
            yield return null;
        }
    }

    public void PlaySound(AudioClip clip)
    {
        if (mAudioSource != null && clip != null)
        {
            mAudioSource.PlayOneShot(clip, volumeSound * PlayerSetting.setting.floatVolumeEffect);
            volumeSound -= 0.08f;
        }
    }
}
