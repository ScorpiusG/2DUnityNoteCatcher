using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Control : MonoBehaviour
{
    public MarathonMenu_Item marathonItemTutorial;
    private bool isLoadingScene = false;

    public void PlayTutorial()
    {
        if (isLoadingScene) return;
        isLoadingScene = true;

        Game_Control.marathonItem = marathonItemTutorial;
        Game_Control.intMarathonItem = 0;
        Game_Control.boolAutoplay = false;
        Game_Control.boolCustomSong = false;

        SceneTransition.LoadScene("Game");
    }

    public void SceneLoad(string scene)
    {
        if (isLoadingScene) return;
        isLoadingScene = true;
        SceneTransition.LoadScene(scene);
    }
}
