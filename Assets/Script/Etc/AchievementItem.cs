using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementItem", menuName = "Achievement Item")]
public class AchievementItem : ScriptableObject
{
    // Please refer to AchievementSystem.cs on how to use this.

    public string achievementCode = "CODE";
    public string achievementName = "Name";
    [TextArea] public string achievementDescription = "Description";
    public int achievementValue = 0;
    public Sprite achievementIcon;
}
