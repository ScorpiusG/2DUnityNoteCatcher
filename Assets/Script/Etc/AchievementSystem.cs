using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementSystem : MonoBehaviour
{
    /*
     *  == HOW TO USE ==
     * 
     * Creating new achievements:
     * 
     *  There is a new tab you can use to create one. Follow this path in your Unity Editor.
     *      Assets > Create > Achievement Item
     *  
     *  A new scriptable object will be created in the folder you have currently selected.
     *  You can rename it to whatever you like, but you MUST place it in your own "Resource" folder - preferrably in the "Resources/Achievements" folder.
     *  Edit its details (Name, Description, etc.) as necessary.
     *  
     *      (It doeesn't matter where the file is located in the Resources folder (like Resources/Mike/Game/Achievements),
     *      but the most important rule is that it has to be within the Resources folder for it to be detected by this script.)
     *  
     *  
     * Scripting functions (can be copied for convenience):
     * 
     *  AchievementSystem.instance.AchievementAccomplish("name");
     *      Used when the player has fulfilled conditions to get the achievement.
     *  
     *  AchievementSystem.instance.AchievementCheck("name");
     *      Returns a boolean. TRUE if achievement has been accomplished. FALSE if otherwise.
     *      e.g.
     *          bool gotThisAchievement = AchievementSystem.instance.AchievementCheck("MyAchievement");
     *          string text = "I got this achievement = " + gotThisAchievement.ToString();
     *      
     *  AchievementSystem.instance.GetAchievementItem("name");
     *      Returns an AchievementItem. If you need to get its details such as its name, description, etc. use this.
     *      e.g.
     *          AchievementItem item = AchievementSystem.instance.GetAchievementDetails("MyAchievement");
     *          string itemName = item.achievementName;
     *          
     *  AchievementSystem.instance.GetAchievementItemAll();
     *      Returns an AchievementItem array. Useful for making a list for ALL existing achievements.
     *      e.g.
     *          AchievementItem[] list = AchievementSystem.instance.GetAchievementItemAll();
     *          foreach (AchievementItem item in list)
     *          {
     *              yourAchievementList.Add(item);
     *          }
     *          
     *  AchievementSystem.instance.ResetAchievements();
     *      Deletes ALL accomplished achievements. Used internally for debug purpose.
     *      I will not be responsible for removing your 100+ achievements you worked hard over 1,000 hours after using this.
     */

    private static AchievementSystem _instance;
    public static AchievementSystem instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = Instantiate(Resources.Load("AchievementSystem")) as GameObject;
                _instance = go.GetComponent<AchievementSystem>();
            }
            return _instance;
        }
    }

    private AchievementItem[] listItem;
    public List<string> listItemGet = new List<string>();

    public Animator animatorAchievementGet;
    public Text textAchievementGetName;
    private List<string> listAchievementDisplay = new List<string>();
    private bool boolAnimatorAchievementGetFinish = false;

    private void Awake()
    {
        // Singleton.
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // Get all achievements from resources folder.
        listItem = Resources.FindObjectsOfTypeAll<AchievementItem>();

        // Load any existing achievements acquired.
        Load();

        // Check for acquired achievements during its runtime and show an animation for each new one.
        StartCoroutine("AnimateAchievementDisplay");
    }

    private void Save()
    {
        string info = "";
        info = JsonUtility.ToJson(_instance);
        PlayerPrefs.SetString("_achievements", info);
        PlayerPrefs.Save();
    }
    private void Load()
    {
        if (!PlayerPrefs.HasKey("_achievements")) return;

        string info = "";
        info = PlayerPrefs.GetString("_achievements");
        JsonUtility.FromJsonOverwrite(info, _instance);
    }

    /// <summary>
    /// Have the player "accomplish" an achievement.
    /// </summary>
    /// <param name="achievementName">The achievement code in the scriptable object, NOT the file's name itself.</param>
    public void AchievementAccomplish(string achievementCode)
    {
        // If the player has this achievement, stop immediately.
        foreach (string s in listItemGet)
        {
            if (achievementCode == s)
            {
                return;
            }
        }

        // Otherwise, find it in the main list.
        bool isItemExisting = false;
        foreach (AchievementItem i in listItem)
        {
            //Debug.Log(achievementCode + " = " + i.achievementCode + " ?");
            if (achievementCode == i.achievementCode)
            {
                isItemExisting = true;
                break;
            }
        }
        // If the achievement code links to a non-existent item, nothing happens.
        if (!isItemExisting)
        {
#if UNITY_EDITOR
            Debug.LogWarning("Achievement item for code \"" + achievementCode + "\" does not exist.");
#endif
            return;
        }

        // Add item's code to the acquired achievements list.
        listItemGet.Add(achievementCode);
        Save();

        // Animate this.
        listAchievementDisplay.Add(achievementCode);
    }

    /// <summary>
    /// Checks if the specified achievement has already been accomplished by the player.
    /// </summary>
    /// <param name="achievementCode">The achievement code in the scriptable object, NOT the file's name itself.</param>
    /// <returns></returns>
    public bool AchievementCheck(string achievementCode)
    {
        foreach (string s in listItemGet)
        {
            if (achievementCode == s)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Returns an achievement item to get and use its variables.
    /// </summary>
    /// <param name="achievementCode">The achievement code in the scriptable object, NOT the file's name itself.</param>
    /// <returns></returns>
    public AchievementItem GetAchievementItem(string achievementCode)
    {
        foreach (AchievementItem i in listItem)
        {
            if (achievementCode == i.achievementCode)
            {
                return i;
            }
        }

        return null;
    }

    /// <summary>
    /// Returns ALL available achievement items into an array.
    /// </summary>
    /// <returns></returns>
    public AchievementItem[] GetAchievementItemAll()
    {
        return listItem;
    }

    /// <summary>
    /// Removes ALL achievements. DO NOT USE THIS outside experiments/debugging.
    /// </summary>
    public void ResetAchievements()
    {
        PlayerPrefs.DeleteKey("_achievements");
        listItemGet.Clear();
    }

    private IEnumerator AnimateAchievementDisplay()
    {
        yield return null;

        while (true)
        {
            if (listAchievementDisplay.Count > 0 && animatorAchievementGet != null && textAchievementGetName != null)
            {
                boolAnimatorAchievementGetFinish = false;
                string aCode = listAchievementDisplay[0];
                listAchievementDisplay.RemoveAt(0);

                animatorAchievementGet.Play("clip");
                textAchievementGetName.text = instance.GetAchievementItem(aCode).achievementName;
                yield return new WaitUntil(() => boolAnimatorAchievementGetFinish);
            }

            yield return null;
        }
    }

    /// <summary>
    /// DON'T TOUCH THIS. If you're curious, this is an animation trigger to end a coroutine loop.
    /// </summary>
    public void AnimationFinished()
    {
        boolAnimatorAchievementGetFinish = true;
    }
}
