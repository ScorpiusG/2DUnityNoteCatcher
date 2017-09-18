using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour
{
    public static Notification instance = null;

    public Animator mAnimator;
    public Text mText;
    public AudioSource mAudioSource;
    public AudioClip clipSoundNotification;

    private static List<string> listStringNotification = new List<string>();
    private static List<Color> listColorNotification = new List<Color>();
    private bool isAnimationEnd = false;

    /// <summary>
    /// Displays the message to the user.
    /// </summary>
    /// <param name="message"></param>
    public static void Display(string message, Color color)
    {
        listStringNotification.Add(message);
        listColorNotification.Add(color);

        if (instance == null)
        {
            Debug.LogWarning("WARNING: Message may not be displayed because the Notification instanced object is \"null\". Message to display: " + message);
        }
    }

    /// <summary>
    /// DO NOT TOUCH. For use by the Animator to notify when the animation ends.
    /// </summary>
    public void AnimatorMessageClipEnd()
    {
        isAnimationEnd = true;
    }

    public void MessageEndImmediately()
    {
        mAnimator.speed = 16f;
    }

    private IEnumerator RunMe()
    {
        yield return null;

        while (true)
        {
            if (listStringNotification.Count > 0)
            {
                isAnimationEnd = false;
                mText.text = listStringNotification[0];
                mText.color = listColorNotification[0];
                mAnimator.speed = 1f;
                mAnimator.Play("clip");

                if (mAudioSource != null && clipSoundNotification != null)
                {
                    mAudioSource.PlayOneShot(clipSoundNotification, PlayerSetting.setting.floatVolumeEffect);
                }

                yield return new WaitUntil(() => isAnimationEnd);

                listStringNotification.RemoveAt(0);
                listColorNotification.RemoveAt(0);

                yield return null;
            }

            yield return null;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
            StartCoroutine("RunMe");
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
