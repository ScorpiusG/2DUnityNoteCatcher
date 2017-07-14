using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    public static PlayerSetting setting;

    public List<int> intPlayerTotalScore;

    public float floatMouseSensitivity = 0.06f;
    public float intScrollSpeed = 40;
    public int intAccuracyTolerance = 30;
    public bool enableVSync = true;

    /// <summary>
    /// Adds score to the total amount. The score limit implemented is super high and may take centuries to reach.
    /// </summary>
    /// <param name="score"></param>
    public void ScoreAdd(int score)
    {
        if (intPlayerTotalScore.Count < 1) intPlayerTotalScore.Add(0);

        intPlayerTotalScore[0] += score;
        
        for (int i = 0;; i++)
        {
            if (intPlayerTotalScore[i] < 1000)
            {
                break;
            }
            else
            {
                if (intPlayerTotalScore.Count >= i + 1)
                {
                    intPlayerTotalScore.Add(0);
                }
                intPlayerTotalScore[i + 1] += intPlayerTotalScore[i] / 1000;
                intPlayerTotalScore[i] %= 1000;
            }
        }
    }

    /// <summary>
    /// Get score in string format. Don't bother trying to parse this into a number-based data type, as the number can get incredibly large.
    /// </summary>
    /// <returns></returns>
    public string GetScore()
    {
        if (intPlayerTotalScore.Count == 0)
        {
            return "0";
        }

        string s = "";
        for (int i = intPlayerTotalScore.Count - 1; i >= 0; i++)
        {
            s += intPlayerTotalScore[i];
            if (i > 0) s += ",";
        }
        return s;
    }
    
	void Start ()
    {
        intPlayerTotalScore = new List<int>();

        if (setting == null)
        {
            setting = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
	}

    /// <summary>
    /// Save settings.
    /// </summary>
    public void Save()
    {
        string data = JsonUtility.ToJson(setting);
        PlayerPrefs.SetString("PlayerSetting", data);
        PlayerPrefs.Save();

#if UNITY_EDITOR
        Debug.Log("Recorded data. " + data);
#endif
    }

    /// <summary>
    /// Load settings. To be used only on startup.
    /// </summary>
    public void Load()
    {
        if (PlayerPrefs.HasKey("PlayerSetting"))
        {
            string data = PlayerPrefs.GetString("PlayerSetting");
            JsonUtility.FromJsonOverwrite(data, setting);
#if UNITY_EDITOR
            Debug.Log("Loaded save data. " + data);
#endif
        }
        else
        {
#if UNITY_EDITOR
            Debug.Log("No save data detected.");
#endif
        }
    }
}
