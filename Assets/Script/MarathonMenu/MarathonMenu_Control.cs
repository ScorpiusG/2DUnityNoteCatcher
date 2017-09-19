using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarathonMenu_Control : MonoBehaviour
{
    private static int intMarathonItemLast = 0;
    public MarathonMenu_Item[] arrayMarathonItem;
    
	private void Start ()
    {
        arrayMarathonItem = Resources.LoadAll("MarathonItem", typeof(MarathonMenu_Item)) as MarathonMenu_Item[];

        // TODO: create template button, dupe button and insert details for each item
    }

    public void ViewItemDetails (MarathonMenu_Button button)
    {
        MarathonMenu_Item item = arrayMarathonItem[button.itemID];

        // TODO: show item details
    }

    public void PlayMarathon ()
    {
        Game_Control.marathonItem = arrayMarathonItem[intMarathonItemLast];
        Game_Control.intMarathonItem = 0;

        SceneTransition.LoadScene("Game");
    }

    public void ExitMenu ()
    {
        SceneTransition.LoadScene("Title");
    }
}
