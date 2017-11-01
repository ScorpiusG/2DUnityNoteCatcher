using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SongList", menuName = "Song List")]
public class SongMenu_SongList : ScriptableObject
{
    public List<string> listStringSongList = new List<string>();
}
