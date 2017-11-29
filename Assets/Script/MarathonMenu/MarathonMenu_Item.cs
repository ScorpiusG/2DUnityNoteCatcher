using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MarathonItem0000", menuName = "Marathon Item")]
public class MarathonMenu_Item : ScriptableObject
{
    /// <summary>
    /// Level of the item. Heavily reflects points gained at the end of a cleared marathon.
    /// </summary>
    public int itemLevel = 1;

    /// <summary>
    /// Score multiplier of the item, which is applied if the marathon is successful.
    /// </summary>
    public float itemScoreMultiplier = 1.5f;

    /// <summary>
    /// Sprite of the item displayed in the menu.
    /// </summary>
    public Sprite itemSpriteIcon;

    /// <summary>
    /// Name of the item to be displayed.
    /// </summary>
    public string itemName = "Name";

    /// <summary>
    /// Description of the item to be displayed.
    /// </summary>
    public string itemDescription = "Description";

    /// <summary>
    /// Custom accuracy threshold as set by the item. Overwrites player's set accuracy threshold.
    /// </summary>
    public int itemAccuracyThreshold = 0;

    /// <summary>
    /// Number of missed notes (total) allowed in a marathon before a forced exit.
    /// Set to zero to turn it off.
    /// </summary>
    public int itemNoteMissThreshold = 0;

    /// <summary>
    /// A string array containing the charts to be played in order.
    /// Format: song file name | game mode | chart ID
    /// </summary>
    public string[] itemChartList = { "song|0|0" };

    /// <summary>
    /// A string array containing special modifiers to be used for the marathon. These are entirely separate from player-induced modifiers.
    /// Leave it blank to not apply modifiers.
    /// </summary>
    public string[] itemModList = { };
}
