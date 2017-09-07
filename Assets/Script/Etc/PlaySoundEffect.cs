using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlaySoundEffect : MonoBehaviour
{
    private AudioSource mAudioSource;

    private void Awake()
    {
        mAudioSource = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (mAudioSource != null && clip != null)
        {
            mAudioSource.PlayOneShot(clip, PlayerSetting.setting.floatVolumeEffect);
        }
    }
}
