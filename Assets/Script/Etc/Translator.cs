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

    public int intLanguageCurrent = 0;
    public string[] stringLanguageCode;
    public Font[] fontLanguageCode;

    private List<string> stringTranslation = new List<string>();

    /// <summary>
    /// Try to get the stored localized string from the text document. If it fails, the second argument will be used as the main text.
    /// </summary>
    /// <param name="code">The code value before the '|' character.</param>
    /// <param name="origin">If it fails to get the localized string, it will return as this default value.</param>
    /// <returns></returns>
    public static string GetStringTranslation(string code, string origin = "")
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

        return origin;
    }

    /// <summary>
    /// Returns font being used with the current language.
    /// </summary>
    /// <returns></returns>
    public static Font GetLanguageFont()
    {
        if (mTranslator.intLanguageCurrent <= 0 ||
            mTranslator.intLanguageCurrent + 1 < mTranslator.fontLanguageCode.Length)
        {
            return null;
        }

        return mTranslator.fontLanguageCode[mTranslator.intLanguageCurrent - 1];
    }

    /// <summary>
    /// Switch current language with the language from the assigned ID.
    /// </summary>
    /// <param name="ID"></param>
    public static void SwitchLanguage(int id)
    {
        PlayerPrefs.SetInt("manual_translation_id", id);
        PlayerPrefs.Save();
        mTranslator.intLanguageCurrent = id;
        mTranslator.LoadTranslationDocument();
    }

    private void LoadTranslationDocument()
    {
        if (intLanguageCurrent <= 0 ||
            intLanguageCurrent + 1 < stringLanguageCode.Length)
        {
            return;
        }

        string nameFile = "translation_" + stringLanguageCode[intLanguageCurrent - 1] + ".txt";

        // Read the translation_<x>.txt file
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

    void Awake()
    {
        if (mTranslator != null)
        {
            Destroy(gameObject);
            return;
        }

        mTranslator = this;
        DontDestroyOnLoad(gameObject);

        intLanguageCurrent = PlayerPrefs.GetInt("manual_translation_id", 1);
        LoadTranslationDocument();
    }
}
