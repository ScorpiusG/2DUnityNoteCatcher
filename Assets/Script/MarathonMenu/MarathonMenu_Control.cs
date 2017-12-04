using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarathonMenu_Control : MonoBehaviour
{
    private static int intMarathonItemLast = -1;
    public MarathonMenu_Button buttonTemplate;
    public Transform transformButtonParent;

    public Image imagePlayerLevelMax;
    public Text textPlayerLevel;
    public Text textPlayerScore;
    public Text textPlayerScoreNextLevel;
    public Image imageScoreGauge;

    public GameObject objectGroupDetails;
    public Vector3 positionGroupDetailsInit;
    public Text textItemName;
    public RawImage imageItemSprite;
    public Text textItemLevel;
    public Text textItemDescription;
    public Text textItemWinCondition;
    public Text textItemSongList;
    public Text textItemModList;
    public Text textItemRecord;

    public Slider sliderScrollSpeed;
    public Text textDisplayScrollSpeed;

    public MarathonMenu_Item[] arrayMarathonItem;
    public List<MarathonMenu_Item> listMarathonItem = new List<MarathonMenu_Item>();

    private void Start ()
    {
        // Variable initialization
        sliderScrollSpeed.value = PlayerSetting.setting.intScrollSpeed;
        textDisplayScrollSpeed.text = Translator.GetStringTranslation("SONGMENU_SCROLLSPEED", "Note Scroll Speed:") + " x" + (0.1f * PlayerSetting.setting.intScrollSpeed).ToString("f1");
        positionGroupDetailsInit = objectGroupDetails.transform.position;

        // Object initialization
        textItemName.text = "";
        imageItemSprite.gameObject.SetActive(false);
        textItemLevel.text = "";
        textItemDescription.text = "";
        textItemWinCondition.text = "";
        textItemSongList.text = "";
        textItemModList.text = "";
        textItemRecord.text = "";

        // Player level and score display
        textPlayerScore.text = PlayerSetting.setting.GetScore();
        // Has already reached maximum level (100), display score only.
        if (PlayerSetting.setting.boolPlayerLevelMax)
        {
            imagePlayerLevelMax.gameObject.SetActive(true);
            textPlayerLevel.gameObject.SetActive(false);
            textPlayerScoreNextLevel.gameObject.SetActive(false);
            imageScoreGauge.fillAmount = 1f;
        }
        // Otherwise; show level, score, and score required to next level.
        else
        {
            int currentLevel = PlayerSetting.setting.GetPlayerLevel();
            int prevLevelScore = PlayerSetting.setting.GetLevelScoreRequirement(currentLevel - 1);
            int currentScore = PlayerSetting.setting.ConvertListIntToInt(PlayerSetting.setting.intPlayerTotalScore);
            int nextLevelScore = PlayerSetting.setting.GetLevelScoreRequirement(currentLevel);

            imagePlayerLevelMax.gameObject.SetActive(false);
            textPlayerLevel.gameObject.SetActive(true);
            textPlayerLevel.text = currentLevel.ToString();
            textPlayerScoreNextLevel.gameObject.SetActive(true);
            textPlayerScoreNextLevel.text = "(" + Translator.GetStringTranslation("SONGMENU_PLAYERNEXTLEVELSCORE", "Next Level:") + " " + (nextLevelScore - currentScore).ToString("n0") + ")";
            imageScoreGauge.fillAmount = 1f * (currentScore - prevLevelScore) / (nextLevelScore - prevLevelScore);
        }

        // Get marathon items, sort them, and create button list
        arrayMarathonItem = Resources.FindObjectsOfTypeAll<MarathonMenu_Item>();
        foreach (MarathonMenu_Item x in arrayMarathonItem)
        {
            listMarathonItem.Add(x);
        }
        listMarathonItem.Sort(
            delegate (MarathonMenu_Item a, MarathonMenu_Item b)
            {
                return (a.name.CompareTo(b.name));
            }
            );
        
        buttonTemplate.gameObject.SetActive(false);
        MarathonMenu_Button autoselectButton = null;
        for (int i = 0; i < listMarathonItem.Count; i++)
        {
            MarathonMenu_Button button = Instantiate(buttonTemplate);
            button.name = listMarathonItem[i].itemName;
            button.itemID = i;
            if (listMarathonItem[i].itemSpriteIcon != null)
            {
                button.imageButton.sprite = listMarathonItem[i].itemSpriteIcon;
            }
            else
            {
                button.imageButton.gameObject.SetActive(false);
            }
            button.textItem.text = listMarathonItem[i].itemName + " (*" + listMarathonItem[i].itemLevel.ToString() + ")";
            button.marathonItem = listMarathonItem[i];

            button.transform.SetParent(transformButtonParent);
            button.transform.localScale = Vector3.one;

            button.gameObject.SetActive(true);

            if (i == intMarathonItemLast || (intMarathonItemLast == -1 && autoselectButton == null))
            {
                autoselectButton = button;
            }
        }
        if (autoselectButton != null)
        {
            intMarathonItemLast = -1;
            ViewItemDetails(autoselectButton);
        }
        Destroy(buttonTemplate.gameObject);
    }

    public void ViewItemDetails (MarathonMenu_Button button)
    {
        // Do nothing if current button is the same
        if (button.itemID == intMarathonItemLast)
        {
            return;
        }

        // Reset all button colors and darken selected one
        MarathonMenu_Button[] buttonAll = FindObjectsOfType<MarathonMenu_Button>();
        foreach (MarathonMenu_Button x in buttonAll)
        {
            x.GetComponent<Image>().color = Color.white;
        }
        button.GetComponent<Image>().color = Color.grey;

        // Get the Scriptable Object
        MarathonMenu_Item item = button.marathonItem;
        Game_Control.marathonItemID = intMarathonItemLast = button.itemID;

        // General
        textItemName.text = item.itemName;
        if (item.itemSpriteIcon != null)
        {
            imageItemSprite.gameObject.SetActive(true);
            imageItemSprite.texture = item.itemSpriteIcon.texture;
        }
        else
        {
            imageItemSprite.gameObject.SetActive(false);
        }
        textItemLevel.text = "*" + item.itemLevel.ToString();
        textItemDescription.text = item.itemDescription;
        textItemWinCondition.text = "";
        textItemSongList.text = "Chart List: ";
        textItemModList.text = "Mods: ";
        textItemRecord.text = "Best Accuracy: " + (PlayerPrefs.GetFloat("marathon-" + button.itemID.ToString() + "-accuracy", 0f) * 100f).ToString("f2") + "% | Attempts: " + PlayerPrefs.GetInt("marathon-" + button.itemID.ToString() + "-attempts", 0).ToString();

        // Win condition
        if (item.itemAccuracyThreshold == 0 && item.itemNoteMissThreshold == 0)
        {
            textItemWinCondition.text = "No win condition.";
        }
        else
        {
            if (item.itemAccuracyThreshold > 0)
            {
                textItemWinCondition.text = "Accuracy Threshold: " + item.itemAccuracyThreshold.ToString() + "%";
            }
            if (item.itemNoteMissThreshold > 0)
            {
                if (textItemWinCondition.text.Length > 0)
                {
                    textItemWinCondition.text += "\n";
                }
                textItemWinCondition.text += "Note Miss Threshold: " + item.itemNoteMissThreshold.ToString();
            }
        }

        // Song list
        if (item.itemChartList.Length == 0)
        {
            textItemSongList.text += "\n   None.";
        }
        else if (item.itemChartList.Length > 5)
        {
            textItemSongList.text += "\n   A lot!";
        }
        else
        {
            ChartData tempChart = ScriptableObject.CreateInstance(typeof(ChartData)) as ChartData;
            foreach (string x in item.itemChartList)
            {
                string[] y = x.Split('|');
                /*
                 *  0 - song folder & file name
                 *  1 - game mode
                 *  2 - chart ID
                 */

                string path = "Songs/" + y[0] + "/" + y[0] + "-" + y[1] + "-" + y[2];
                TextAsset text = (TextAsset)Resources.Load(path, typeof(TextAsset));
                string input = text.text;
                JsonUtility.FromJsonOverwrite(input, tempChart);

                string stringMode = "";
                switch (tempChart.chartGameType)
                {
                    case 0: stringMode = "Linear"; break;
                    case 1: stringMode = "Double"; break;
                    case 2: stringMode = "Quad"; break;
                    case 3: stringMode = "Note Dodge"; break;
                }
                stringMode = Translator.GetStringTranslation("SONGMENU_CHARTGAMETYPEBODY" + tempChart.chartGameType, stringMode);

                textItemSongList.text += "\n   " + tempChart.songName + " / " + stringMode + " #" + (int.Parse(y[2]) + 1).ToString() + " (*" + tempChart.chartLevel.ToString() + ")";
            }
        }

        // Modifier list
        if (item.itemModList.Length == 0)
        {
            textItemModList.text += "None";
        }
        else
        {
            foreach (string x in item.itemModList)
            {
                switch (x)
                {
                    default: textItemModList.text += x; break;
                }

                textItemModList.text += "\n";
            }
        }

        // iTween window tween effect
        float tweenDuration = 0.31f;
        objectGroupDetails.SetActive(true);
        objectGroupDetails.transform.position = positionGroupDetailsInit + Vector3.right * 1000f;
        iTween.MoveTo(objectGroupDetails, positionGroupDetailsInit, tweenDuration);

        // For use in game
        Game_Control.marathonItem = item;
    }

    public void PlayMarathon ()
    {
        //Game_Control.marathonItem = arrayMarathonItem[intMarathonItemLast];
        Game_Control.intMarathonItem = 0;
        Game_Control.boolAutoplay = false;
        Game_Control.boolCustomSong = false;

        SceneTransition.LoadScene("Game");
    }

    public void AdjustScrollSpeed()
    {
        PlayerSetting.setting.intScrollSpeed = Mathf.RoundToInt(sliderScrollSpeed.value);
        textDisplayScrollSpeed.text = Translator.GetStringTranslation("SONGMENU_SCROLLSPEED", "Note Scroll Speed:") + " x" + (0.1f * PlayerSetting.setting.intScrollSpeed).ToString("f1");
    }

    public void ExitMenu ()
    {
        Game_Control.marathonItem = null;

        SceneTransition.LoadScene("Title");
    }
}
