using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alpha_FeedbackNotification : MonoBehaviour
{
    public GameObject objectGetFeedbackWindow;

    private const string feedbackURL = "https://docs.google.com/forms/d/18kFQmyNlrVAMX_W7jdhg96zCT4FWmEUvqti5EOibQf8";

    private void OnEnable()
    {
        if (objectGetFeedbackWindow != null)
        {
            int userGaveFeedback = PlayerPrefs.GetInt("Alpha_FeedbackNotification.userGaveFeedback", 0);
            int userLevel = PlayerSetting.setting.GetPlayerLevel();

#if UNITY_EDITOR
            Debug.Log("[Alpha_FeedbackNotification] Player Level: " + userLevel.ToString());
#endif

            if (userGaveFeedback == 0 && userLevel > 4)
            {
                objectGetFeedbackWindow.SetActive(true);
            }
            else
            {
                objectGetFeedbackWindow.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            OpenForm();
        }
    }

    /// <summary>
    /// Closes the window asking for feedback.
    /// </summary>
    public void CloseWindow()
    {
        objectGetFeedbackWindow.SetActive(false);
    }

    /// <summary>
    /// Opens the uniform resource locator leading to the Google feedback form.
    /// </summary>
    public void OpenForm()
    {
        PlayerPrefs.SetInt("Alpha_FeedbackNotification.userGaveFeedback", 1);
        PlayerPrefs.Save();

        Notification.Display("The form is being opened through your browser...", Color.white);
        Application.OpenURL(feedbackURL);
    }
}
