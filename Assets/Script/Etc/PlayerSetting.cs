using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSetting : MonoBehaviour
{
    public static PlayerSetting setting;

    // Player in-game level maxed out. If true, prevent calculations.
    public bool boolPlayerLevelMax = false;
    // Player total play count for each respective game mode
    public int intPlayerTotalPlayCountLN = 0;
    public int intPlayerTotalPlayCountDB = 0;
    public int intPlayerTotalPlayCountQD = 0;
    public int intPlayerTotalPlayCountND = 0;
    // Player accumulated score.
    public List<int> intPlayerTotalScore = new List<int>();

    // General game options.
    // Affects in-game song offset.
    public int intGameOffset = 0;
    // Affects mouse movement.
    public float floatMouseSensitivity = 0.06f;
    // Affects brightness of background.
    public float floatBackgroundBrightness = 0.7f;
    // Use raw mouse input than smooth input.
    public bool boolUseMouseRawInput = true;
    // Affects note scroll speed.
    public int intScrollSpeed = 10;
    // Affects song force end on being unable to achieve a certain accuracy percentage.
    public int intAccuracyTolerance = 0;
    // Music volume
    public float floatVolumeMusic = 1f;
    // Sound effect volume
    public float floatVolumeEffect = 1f;

    // Interface and frame settings.
    // Vertical synchronization.
    public bool enableVSync = true;
    // Display song details - artist, name, chart developer, chart ID, chart level, time.
    public bool enableInterfaceSongDetails = true;
    // Display player's current accuracy.
    public bool enableInterfaceAccuracy = true;
    // Display current combo.
    public bool enableDisplayCombo = true;
    // Display player record's "ghost" accuracy. It will be displayed below the combo value.
    public bool enableDisplayRecordGhost = false;
    // Display note hit judgment.
    public bool enableDisplayNoteJudgment = true;
    // Display small counters for each note judgment received.
    public bool enableDisplayNoteHitCounterSmall = false;
    // Have all catchers and notes pulse on every beat.
    public bool enableNoteAndCatcherHighlightBeatPulse = true;
    // Play a tick sound effect upon hitting a note.
    public bool enableAssistTickSound = false;

    // Game modifiers. Enabling certain ones will disable scoring (record for highest accuracy and total score). As of now, these are not implemented yet.
    // Does nothing while disabling high score.
    public bool modDisableScore = false;
    // Flips the whole screen 180 degrees. Disables scoring.
    public bool modScreenFlip = false;
    // Mirrors the screen horizontally. Disables scoring.
    public bool modScreenMirror = false;
    // All note types are inverted: 0 <> 1, 2 <> 3. Disables scoring.
    public bool modChartFlip = false;
    // All notes' horizontal positions are inverted. Disables scoring.
    public bool modChartMirror = false;
    // All notes' horizontal positions are randomly assigned with fixed distances between each other. (If the note is somehow positioned outside the field, position it back in the center.) Disables scoring.
    public bool modChartRandom = false;
    // All notes' horizontal positions are randomly assigned. Unlike ChartRandom mod above, this one has no fixed distances between each note and is completely random. Disables scoring.
    public bool modChartBerserk = false;
    // Random fake(!) notes appear around the real note. Disables scoring.
    public bool modChartCluster = false;
    // A thermometer will appear on screen. With little to no movement, the temperature rises. Moving the cursor will lower the temperature. Force end on maxed temperature. Disables scoring.
    public bool modChartHell = false;
    // Notes will reduce opacity as it is being reached.
    public bool modNoteFadeOut = false;
    // Notes will start transparent and increase opacity as it is being reached.
    public bool modNoteFadeIn = false;
    // Notes' opacity will blink on beat to the song.
    public bool modNoteBlink = false;
    // Note color will be randomized upon being spawned.
    public bool modNoteRandomColor = false;
    // Bullets will reduce opacity over time.
    public bool modDodgeBulletFadeOut = false;
    // Bullets will start transparent and increase opacity over time.
    public bool modDodgeBulletFadeIn = false;
    // Bullets' opacity will blink on beat to the song.
    public bool modDodgeBulletBlink = false;
    // A small area is only visible around the dodger.
    public bool modDodgeDarkRoom = false;

    // Et cetera
    // Screenshot quantity number.
    public int intScreenshotValue = 0;

    /// <summary>
    /// Adds score to the total amount. The score limit implemented is super high and may take centuries to reach. (Or in this case, a few years?)
    /// </summary>
    /// <param name="score"></param>
    public void ScoreAdd(int score)
    {
        // Add a number into the score list if it's empty
        if (intPlayerTotalScore.Count < 1) intPlayerTotalScore.Add(0);

        // Add the value into the first item in the list
        intPlayerTotalScore[0] += score;
        
        // Check if the value is over 1,000
        for (int i = 0;; i++)
        {
            // False = Stop
            if (intPlayerTotalScore[i] < 1000)
            {
                break;
            }
            // True = Transfer 1,000s to the next item
            else
            {
                // If the next item doesn't exist, add it
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
        // How it works: Score starts from last value in the list and ends at the first.
        // Example list: { 789, 456, 123, 1 }   Will appear as: "1,123,456,789"

        string s = "";
        if (intPlayerTotalScore.Count > 0)
        {
            for (int i = intPlayerTotalScore.Count - 1; i >= 0; i--)
            {
                if (i == intPlayerTotalScore.Count - 1)
                {
                    // Remove any zero values at the end of the list.
                    if (intPlayerTotalScore[i] == 0)
                    {
                        intPlayerTotalScore.RemoveAt(i);
                        continue;
                    }

                    // The highest triple-digits are displayed normally.
                    s += intPlayerTotalScore[i].ToString("0");
                }
                else
                {
                    // The non-highest triple-digits must have three numbers.
                    s += intPlayerTotalScore[i].ToString("000");
                }
                if (i > 0) s += ",";
            }
        }
        else
        {
            s = "0";
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
        if (level >= 100) boolPlayerLevelMax = true;
        return level;
    }

    /// <summary>
    /// Get player level with its internally recorded total score. Returns 100 at maximum.
    /// </summary>
    /// <returns></returns>
    public int GetPlayerLevel()
    {
        if (boolPlayerLevelMax)
        {
            return 100;
        }

        int level = CalculateLevel(intPlayerTotalScore);
        if (level > 100)
        {
            level = 100;
            boolPlayerLevelMax = true;
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
        return (((level + 1) * level) / 2) * ((level * 6) + 94) * 10;
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
    /// <summary>
    /// Converts a list of 3-digit integers to an integer
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public int ConvertListIntToInt(List<int> list)
    {
        int value = 0;
        for (int i = 0; i < list.Count; i++)
        {
            value += list[i] * (1000 * i);
        }
        return value;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
#if UNITY_EDITOR
            Debug.Log("Screen captured: screenshot" + intScreenshotValue.ToString() + ".png");
#endif
            Application.CaptureScreenshot("screenshot" + intScreenshotValue.ToString() + ".png");
            intScreenshotValue++;
        }
    }
}
