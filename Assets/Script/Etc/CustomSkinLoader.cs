using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomSkinLoader : MonoBehaviour
{
    public static CustomSkinLoader loader;
    public string stringSkinCurrent = "";
    private List<string> stringSkinInfo = new List<string>();

    public Sprite spriteCrosshair;
    public Sprite spriteDodger;
    public Sprite spriteCatcher;
    public Sprite spriteNoteCatch;
    public Sprite spriteNoteCatchHighlight;
    public Sprite spriteNoteTap;
    public Sprite spriteNoteTapHighlight;

    /// <summary>
    /// Returns a string list of available skins in the Skin folder.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetListStringAvailableSkins()
    {
        List<string> listSkins = new List<string>();

        string path = Directory.GetCurrentDirectory() + "/Skin";
        DirectoryInfo main = new DirectoryInfo(path);
        DirectoryInfo[] sub = main.GetDirectories();

        foreach (DirectoryInfo x in sub)
        {
            listSkins.Add(x.Name);
        }

        return listSkins;
    }

    /// <summary>
    /// Have the loader load the skin assets from the folder and save the skin name for future use.
    /// </summary>
    /// <param name="name">The folder's name of the skin.</param>
    public void LoadSkin(string name)
    {
        stringSkinCurrent = name;
        PlayerPrefs.SetString("CustomSkinLoader-skin", name);
        PlayerPrefs.Save();
        LoadSkin();
    }

    /// <summary>
    /// Try to get the stored string from the text document. If it fails, the second argument will be used as the main text.
    /// </summary>
    /// <param name="code">The code value before the '|' character.</param>
    /// <param name="defaultValue">If it fails to get the string, it will return as this default value.</param>
    /// <returns></returns>
    public static string GetStringInfo(string code, string defaultValue = "")
    {
        foreach (string x in loader.stringSkinInfo)
        {
            string[] y = x.Split('|');

            if (y[0] == code)
            {
                string t = "";
                if (y.Length > 1)
                {
                    for (int i = 1; i < y.Length; i++)
                    {
                        t += y[i];
                        if (i + 1 < y.Length)
                        {
                            t += "\n";
                        }
                    }
                }
                t.Replace("`", "\n");
                return t;
            }
        }

        return defaultValue;
    }

    private void LoadSkin()
    {
        // Remove all info from previous skin
        ClearAll();

        // If skin name is empty, do nothing
        if (stringSkinCurrent.Length <= 0) return;


        // Else, load the assets
        string path = "Skin/" + stringSkinCurrent;
        // Read the info file
        if (!File.Exists(path + "/info.txt"))
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: The translation file does not exist! Path: " + path + "/info.txt");
#endif
            return;
        }
        StreamReader reader = new StreamReader(path + "/info.txt");
        while (!reader.EndOfStream)
        {
            stringSkinInfo.Add(reader.ReadLine());
        }
        reader.Close();

        // Then read the sprites
        spriteCrosshair = LoadSprite(path + "/crosshair.png", 800f);
        spriteDodger = LoadSprite(path + "/dodger.png", 4800f);
        spriteCatcher = LoadSprite(path + "/catcher.png", 2400f);
        spriteNoteCatch = LoadSprite(path + "/notecatch.png", 1600f);
        spriteNoteCatchHighlight = LoadSprite(path + "/notecatchhl.png", 1600f);
        spriteNoteTap = LoadSprite(path + "/notetap.png", 456f);
        spriteNoteTapHighlight = LoadSprite(path + "/notetaphl.png", 8f);
    }

    private Sprite LoadSprite(string path, float PPUMultiplier = 100f)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        WWW www = new WWW("file://" + Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/")) + path);
        Texture2D sTex = www.texture;
        Rect sRect = new Rect(0f, 0f, sTex.width, sTex.height);

        float largerLength = sTex.width;
        if (sTex.height > largerLength) largerLength = sTex.height;
        
        return Sprite.Create(sTex, sRect, Vector2.zero, PPUMultiplier * largerLength / 1024f);
    }

    private void ClearAll()
    {
        // Nullifies all skin info
        stringSkinInfo.Clear();
        spriteCrosshair = null;
        spriteDodger = null;
        spriteCatcher = null;
        spriteNoteCatch = null;
        spriteNoteCatchHighlight = null;
        spriteNoteTap = null;
        spriteNoteTapHighlight = null;
    }

	private void Start ()
    {
        // Startup - Load previously used skin if applicable
        loader = this;
        stringSkinCurrent = PlayerPrefs.GetString("CustomSkinLoader-skin", "");
        LoadSkin();
    }
}
