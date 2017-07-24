using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    public static PlayerSetting setting;

    // Player accumulated score.
    public List<int> intPlayerTotalScore = new List<int>();

    // General game options.
    // Affects in-game song offset.
    public int intGameOffset = 0;
    // Affects mouse movement.
    public float floatMouseSensitivity = 0.06f;
    // Use raw mouse input than smooth input.
    public bool boolUseMouseRawInput = true;
    // Affects note scroll speed.
    public int intScrollSpeed = 40;
    // Affects song force end on being unable to achieve a certain accuracy percentage.
    public int intAccuracyTolerance = 30;

    // Interface and frame settings.
    // Vertical synchronization.
    public bool enableVSync = true;
    // Display song details - artist, name, chart developer, chart ID, chart level, time.
    public bool enableInterfaceSongDetails = true;
    // Display player's current accuracy.
    public bool enableInterfaceAccuracy = true;
    // Display current combo.
    public bool enableDisplayCombo = true;
    // Display note hit judgment.
    public bool enableDisplayNoteJudgment = true;
    // Display small counters for each note judgment received.
    public bool enableDisplayNoteHitCounterSmall = false;
    // Have all catchers and notes pulse on every beat.
    public bool enableNoteAndCatcherHighlightBeatPulse = true;

    // Other fun(?) stuff. DISABLE SCORING if any of these are used.
    // Does nothing while not affecting high score
    public bool modDisableScore = false;
    // Flips the whole screen 180 degrees.
    public bool modScreenFlip = false;
    // Mirrors the screen horizontally.
    public bool modScreenMirror = false;
    // All note types are inverted: 0 <> 1, 2 <> 3.
    public bool modChartFlip = false;
    // All notes' horizontal positions are inverted.
    public bool modChartMirror = false;
    // Random fake(!) notes appear around the real note.
    public bool modChartCluster = false;
    // A thermometer will appear on screen. With little to no movement, the temperature rises. Moving the cursor will lower the temperature. Force end on maxed temperature.
    public bool modChartHell = false;
    // The cursor will become a ship. Notes will spawn rings of bullets (higher judgment = more bullets!). Lose a life if cursor hits a bullet. Accuracy = Chart progress. If 5 lives are lost, force end.
    public bool modChartRain = false;

    /// <summary>
    /// Adds score to the total amount. The score limit implemented is super high and may take centuries to reach. (Or in this case, a few years?)
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

    /// <summary>
    /// Calculates player possible level using the value given.
    /// </summary>
    /// <param name="value">Score value.</param>
    /// <returns></returns>
    public int CalculateLevel(int value)
    {
        List<int> v = ConvertIntToListInt(value);
        return CalculateLevel(v);
    }
    /// <summary>
    /// Calculates player possible level using the value given in the list.
    /// </summary>
    /// <param name="value">Score value in every three units per item in the list.</param>
    /// <returns></returns>
    public int CalculateLevel(List<int> value)
    {
        int level = 1;
        while (true)
        {
            int req = GetLevelScoreRequirement(level);
            bool isAboveRequirement = false;
            List<int> reqList = ConvertIntToListInt(req);

            // Requirement has more digits than value.
            if (reqList.Count > value.Count)
            {
                isAboveRequirement = false;
            }
            // Requirement has less digits than value.
            else if (reqList.Count < value.Count)
            {
                isAboveRequirement = true;
            }
            // Requirement has similar digits as value.
            else
            {
                for (int i = value.Count - 1; i >= 0; i--)
                {
                    if (reqList[i] > value[i])
                    {
                        isAboveRequirement = false;
                        break;
                    }
                    else if (reqList[i] < value[i])
                    {
                        isAboveRequirement = true;
                        break;
                    }
                }
            }

            if (isAboveRequirement)
            {
                level++;
            }
            else
            {
                break;
            }
        }
        return level;
    }

    /// <summary>
    /// Uses a formula to calculate requirement to next level.
    /// </summary>
    /// <param name="level">The value used for calculation.</param>
    /// <returns></returns>
    public int GetLevelScoreRequirement(int level)
    {
        return (((level + 1) * level) / 2) * ((level * 6) + 94);
    }

    /// <summary>
    /// Converts an integer to a list of 3-digit integers.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public List<int> ConvertIntToListInt(int value)
    {
        List<int> list = new List<int>();
        for (int i = 0; ; i++)
        {
            list.Add(value % 1000);
            value /= 1000;
            if (value <= 0) break;
        }
        return list;
    }
    
	void Awake ()
    {
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
