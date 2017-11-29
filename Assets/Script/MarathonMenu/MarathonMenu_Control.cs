using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarathonMenu_Control : MonoBehaviour
{
    private static int intMarathonItemLast = 0;
    public MarathonMenu_Button buttonTemplate;
    public Transform transformButtonParent;

    public Image imagePlayerLevelMax;
    public Text textPlayerLevel;
    public Text textPlayerScore;
    public Text textPlayerScoreNextLevel;
    public Image imageScoreGauge;

    public Text textItemName;
    public Text textItemLevel;
    public Text textItemDescription;
    public Text textItemWinCondition;
    public Text textItemSongList;
    public Text textItemModList;

    public MarathonMenu_Item[] arrayMarathonItem;
    public List<MarathonMenu_Item> listMarathonItem = new List<MarathonMenu_Item>();

    private void Start ()
    {
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

        // Get marathon items and create button list
        arrayMarathonItem = Resources.LoadAll("MarathonItem", typeof(MarathonMenu_Item)) as MarathonMenu_Item[];
        foreach (MarathonMenu_Item x in arrayMarathonItem)
        {
            listMarathonItem.Add(x);
        }
        listMarathonItem.Sort();
        
        buttonTemplate.gameObject.SetActive(false);
        int itemValue = 0;
        foreach (MarathonMenu_Item x in listMarathonItem)
        {
            MarathonMenu_Button button = Instantiate(buttonTemplate);
            button.name = itemValue.ToString() + " " + x.itemName;
            button.itemID = itemValue;
            button.imageButton.sprite = x.itemSpriteIcon;
            button.textItem.text = x.itemName + " (" + x.itemLevel.ToString() + ")";

            button.transform.SetParent(transformButtonParent);
            button.transform.localScale = Vector3.one;

            button.gameObject.SetActive(true);

            itemValue++;
        }
    }

    public void ViewItemDetails (MarathonMenu_Button button)
    {
        MarathonMenu_Item item = arrayMarathonItem[button.itemID];

        // General
        textItemName.text = item.itemName;
        textItemLevel.text = item.itemLevel.ToString();
        textItemDescription.text = item.itemDescription;
        textItemWinCondition.text = "";
        textItemSongList.text = "";
        textItemModList.text = "";

        // Win condition
        if (item.itemAccuracyThreshold > 0)
        {
            textItemWinCondition.text += "Accuracy Threshold: " + item.itemAccuracyThreshold.ToString() + "%\n";
        }
        if (item.itemNoteMissThreshold > 0)
        {
            textItemWinCondition.text += "Note Miss Threshold: " + item.itemNoteMissThreshold.ToString() + "\n";
        }

        // Song list
        if (item.itemChartList.Length == 0)
        {
            textItemModList.text += "None";
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

                textItemSongList.text += tempChart.songName + " / " + stringMode + " Chart " + y[2] + "\n";
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
    }

    public void PlayMarathon ()
    {
        Game_Control.marathonItem = arrayMarathonItem[intMarathonItemLast];
        Game_Control.intMarathonItem = 0;
        Game_Control.boolAutoplay = false;
        Game_Control.boolCustomSong = false;

        SceneTransition.LoadScene("Game");
    }

    public void ExitMenu ()
    {
        Game_Control.marathonItem = null;

        SceneTransition.LoadScene("Title");
    }
}
