using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Creator_ItemFolder : MonoBehaviour
{
    public Text textFolderName;
    public Text textFolderInfo;

    public void FileChartFieldEdit()
    {
        Creator_Control.control.FileChartFieldEdit(textFolderName.text);
    }
}
