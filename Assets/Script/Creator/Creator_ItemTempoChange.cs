using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_ItemTempoChange : MonoBehaviour
{
    public int itemID = -1;
    public Text textItem;

    public void RemoveMe()
    {
        Creator_Control.control.TempoChangeRemove(this);
    }
    public void EditMe()
    {
        Creator_Control.control.TempoChangeEdit(this);
    }
}
