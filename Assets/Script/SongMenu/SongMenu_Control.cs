using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SongMenu_Control : MonoBehaviour
{
    public bool isLoadCustomSongs = true;

    public string stringSceneNameTitle = "Title";
    public string stringSceneNameGame = "Game";

    public string stringSongDirectoryPath = "/Songs";
    //public string[] arrayStringSongDirectory;
    public List<string> listStringSongDirectory;

    public RectTransform rectSongListParent;
    public Button buttonSongIndividual;
    public float floatVertDistanceBetweenButtons = 80f;

    void Start ()
    {
        string path = Directory.GetCurrentDirectory() + stringSongDirectoryPath;
#if UNITY_EDITOR
        Debug.Log(path);
#endif

        // The "Songs" folder does not exist in the game directory
        if (!Directory.Exists(path))
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: \"Songs\" folder does not exist!");
#endif

            Destroy(buttonSongIndividual.gameObject);
            buttonSongIndividual = null;
            return;
        }

        // Returns the full path of the directory
        //arrayStringSongDirectory = Directory.GetDirectories(path);

        // Gets the folder names only
        DirectoryInfo main = new DirectoryInfo(path);
        DirectoryInfo[] sub = main.GetDirectories();
        foreach (DirectoryInfo x in sub)
        {
            listStringSongDirectory.Add(x.Name);
        }
        // Sort the names
        listStringSongDirectory.Sort();

        // Each name gets its own button
        for (int i = 0; i < listStringSongDirectory.Count; i++)
        {
            Button newBtn = Instantiate(buttonSongIndividual);
            newBtn.name = listStringSongDirectory[i];
            newBtn.GetComponentInChildren<Text>().text = listStringSongDirectory[i];
            newBtn.transform.SetParent(rectSongListParent.transform);
            newBtn.transform.localPosition = Vector3.down * i * floatVertDistanceBetweenButtons;
            newBtn.transform.localScale = Vector3.one;
        }
        // Destroy the template button
        Destroy(buttonSongIndividual.gameObject);
        buttonSongIndividual = null;
    }

    public void PlaySong(string songName, int gameType, int gameStage)
    {
        // TODO: Store song name, game type, game stage, and custom song (bool) information for use in the game scene

        SceneManager.LoadScene(stringSceneNameGame);
    }

    public void UseSongFolder(Button folder)
    {
        Debug.Log(folder.name);
        // TODO: Display chart information from first chart in the folder
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadTitleScene()
    {
        SceneManager.LoadScene(stringSceneNameTitle);
    }

    public void LoadGameScene()
    {
        SceneManager.LoadScene(stringSceneNameGame);
    }
}
