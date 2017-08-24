using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Translator : MonoBehaviour
{
    /*
     *  Reads localization information from a text document.
     */

    public static Translator mTranslator;
    
    public string stringLanguageCode;
    public Font[] fontLanguageCode;

    private List<string> stringTranslation = new List<string>();

    /// <summary>
    /// Try to get the stored localized string from the text document. If it fails, the second argument will be used as the main text.
    /// </summary>
    /// <param name="code">The code value before the '|' character.</param>
    /// <param name="defaultValue">If it fails to get the localized string, it will return as this default value.</param>
    /// <returns></returns>
    public static string GetStringTranslation(string code, string defaultValue = "")
    {
        foreach (string x in mTranslator.stringTranslation)
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
                t.Replace("`","\n");
                //string t = y[1].Replace("`","\n");
                return t;
            }
        }

        return defaultValue;
    }

    /// <summary>
    /// Returns font being used with the current language.
    /// </summary>
    /// <returns></returns>
    public static Font GetLanguageFont()
    {
        if (mTranslator.stringLanguageCode.Length <= 0)
        {
            return null;
        }

        return mTranslator.fontLanguageCode[int.Parse(GetStringTranslation("TRANSLATION_FONT", "0")) - 1];
    }

    /// <summary>
    /// Switch current language with the language from the assigned ID.
    /// </summary>
    /// <param name="ID"></param>
    public static void SwitchLanguage(string id)
    {
        PlayerPrefs.SetString("manual_translation_code", id);
        PlayerPrefs.Save();
        mTranslator.stringLanguageCode = id;
        mTranslator.LoadTranslationDocument();
    }

    /// <summary>
    /// Returns a string list of available languages in the Translation folder.
    /// </summary>
    /// <returns></returns>
    public static List<string> GetListStringAvailableLanguages()
    {
        List<string> listLanguage = new List<string>();

        string path = Directory.GetCurrentDirectory() + "/Translation";
        DirectoryInfo main = new DirectoryInfo(path);
        FileInfo[] sub = main.GetFiles("*.txt", SearchOption.TopDirectoryOnly);
        
        foreach (FileInfo x in sub)
        {
            listLanguage.Add(x.Name);
        }

        return listLanguage;
    }

    private void LoadTranslationDocument()
    {
        if (stringLanguageCode.Length <= 0)
        {
            return;
        }

        string nameFile = "Translation/" + stringLanguageCode;

        // Read the text file
        if (!File.Exists(nameFile))
        {
#if UNITY_EDITOR
            Debug.LogWarning("WARNING: The translation file does not exist! Path: " + nameFile);
#endif
            return;
        }

        StreamReader reader = new StreamReader(nameFile);
        stringTranslation.Clear();
        while (!reader.EndOfStream)
        {
            stringTranslation.Add(reader.ReadLine());
        }
        reader.Close();
    }

    private void Awake()
    {
        if (mTranslator != null)
        {
            Destroy(gameObject);
            return;
        }

        mTranslator = this;
        DontDestroyOnLoad(gameObject);

        stringLanguageCode = PlayerPrefs.GetString("manual_translation_code", "joke");
        LoadTranslationDocument();
    }
}
