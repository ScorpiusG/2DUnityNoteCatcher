using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TranslationMenu_Control : MonoBehaviour
{
    public Transform transformContentListButton;
    public Button buttonTranslationTemplate;
    public string stringNextScene = "Title";

    public Text textTranslationName;
    public Text textTranslationCredit;
    public Text textTranslationDescription;
    public Text textTranslationButtonConfirm;
    public Text textTranslationButtonDefault;

    public void LoadTitleScene()
    {
        SceneTransition.LoadScene(stringNextScene);
    }

    public void DisableTranslation()
    {
        Translator.SwitchLanguage("");
    }

    public void LoadTranslation(Button x)
    {
        Translator.SwitchLanguage(x.name);
        RefreshTexts();
    }

    private void RefreshTexts()
    {
        if (PlayerPrefs.GetString("manual_translation_code", "").Length > 0)
        {
            textTranslationName.text = Translator.GetStringTranslation("TRANSLATION_NAME", "(no name)");
            textTranslationCredit.text = Translator.GetStringTranslation("TRANSLATION_AUTHOR", "(no creator name)");
            textTranslationDescription.text = Translator.GetStringTranslation("TRANSLATION_DESCRIPTION", "(no description)");
            textTranslationButtonConfirm.text = Translator.GetStringTranslation("TRANSLATION_CONFIRM", "Use translation");
            textTranslationButtonDefault.text = Translator.GetStringTranslation("TRANSLATION_DEFAULT", "Use default");
        }
        else
        {
            textTranslationName.text = "";
            textTranslationCredit.text = "";
            textTranslationDescription.text = "You're currently not using a translation setting.\nClick a text file below to use it.";
            textTranslationButtonConfirm.text = "Use translation";
            textTranslationButtonDefault.text = "Use default";
        }
    }

    private void Start()
    {
        List<string> listStringTranslation = Translator.GetListStringAvailableLanguages();
        listStringTranslation.Sort();

        int itemID = 0;
        foreach(string x in listStringTranslation)
        {
            string y = x;
            y = y.TrimEnd('t');
            y = y.TrimEnd('x');
            y = y.TrimEnd('t');
            y = y.TrimEnd('.');

            Button nBtn = Instantiate(buttonTranslationTemplate);
            nBtn.name = y;
            nBtn.transform.SetParent(transformContentListButton);
            nBtn.transform.localScale = Vector3.one;
            nBtn.transform.localPosition = Vector3.down * 60f * itemID;
            nBtn.GetComponentInChildren<Text>().text = y + ".txt";

            itemID++;
        }
        Destroy(buttonTranslationTemplate.gameObject);

        RefreshTexts();
    }
}
