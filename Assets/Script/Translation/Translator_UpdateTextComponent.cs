using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using TMPro;

public class Translator_UpdateTextComponent : MonoBehaviour
{
    // Attach this to objects with text-based components to localize the strings.

    public string stringTranslateCode = "";
    public bool boolChangeFont = true;
    public bool boolTranslateOnUpdate = false;

    void Start()
    {
        if (Translator.mTranslator != null)
        {
            Translate();
        }
        else
        {
            Destroy(this);
        }
    }

    void Update()
    {
        if (boolTranslateOnUpdate)
        {
            Translate();
        }
    }

    void Translate()
    {
        // Edit the texts in each component
        Font nFont = Translator.GetLanguageFont();
        TextMesh mTextMesh = GetComponent<TextMesh>();
        if (mTextMesh != null)
        {
            if (stringTranslateCode.Length > 0)
            {
                mTextMesh.text = Translator.GetStringTranslation(stringTranslateCode, mTextMesh.text);
            }
            if (boolChangeFont && nFont != null)
            {
                mTextMesh.font = nFont;
            }
        }

        Text mText = GetComponent<Text>();
        if (mText != null)
        {
            if (stringTranslateCode.Length > 0)
            {
                mText.text = Translator.GetStringTranslation(stringTranslateCode, mText.text);
            }
            if (boolChangeFont && nFont != null)
            {
                mText.font = nFont;
            }
        }

        /*
        TextMeshPro mTMP = GetComponent<TextMeshPro>();
        if (mTMP != null && stringTranslateCode.Length > 0)
        {
            mTMP.text = Translator.GetStringTranslation(stringTranslateCode, mTMP.text);
        }

        TextMeshProUGUI mTMPU = GetComponent<TextMeshProUGUI>();
        if (mTMPU != null && stringTranslateCode.Length > 0)
        {
            mTMPU.text = Translator.GetStringTranslation(stringTranslateCode, mTMPU.text);
        }
        */

        // Destroy this component if it has no other purpose
        if (!boolTranslateOnUpdate)
        {
            Destroy(this);
        }
    }
}
