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
        textTranslationName.text = Translator.GetStringTranslation("TRANSLATION_NAME", "");
        textTranslationCredit.text = Translator.GetStringTranslation("TRANSLATION_AUTHOR", "");
        textTranslationDescription.text = Translator.GetStringTranslation("TRANSLATION_DESCRIPTION", "You're currently not using a translation setting.\nClick a text file below to use it.");
        textTranslationButtonConfirm.text = Translator.GetStringTranslation("TRANSLATION_CONFIRM", "Use translation");
        textTranslationButtonDefault.text = Translator.GetStringTranslation("TRANSLATION_DEFAULT", "Use default");
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
