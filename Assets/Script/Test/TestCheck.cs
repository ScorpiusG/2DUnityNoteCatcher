using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCheck : MonoBehaviour
{
    public void CheckLevelRequirement()
    {
        for (int i = 1; i <= 100; i++)
        {
            if (i > 1)
            {
                Debug.Log("Score requirement for Level " + i.ToString() + ": " + PlayerSetting.setting.GetLevelScoreRequirement(i).ToString() +
                    " (Difference from last level: " + (PlayerSetting.setting.GetLevelScoreRequirement(i) - PlayerSetting.setting.GetLevelScoreRequirement(i - 1)).ToString() + ")");
            }
            else
            {
                Debug.Log("Score requirement for Level " + i.ToString() + ": " + PlayerSetting.setting.GetLevelScoreRequirement(i).ToString());
            }
        }
    }

    public void CheckLevel(int score)
    {
        Debug.Log("Score " + score.ToString() + " > Level " + PlayerSetting.setting.CalculateLevel(score).ToString());
    }

    public void CheckHighestPossibleScoreGain()
    {
        for (int i = 1; i <= 50; i++)
        {
            int finalScore = Mathf.FloorToInt(Mathf.Pow(4 + i, 2f) * 10);
            Debug.Log("Chart level " + i.ToString() + ", Score " + finalScore.ToString());
        }
    }
}
