using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPassedMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GoToMenu()
    {
        Loader.Load("Main Menu");
    }

    public void Redo()
    {
        Loader.ReloadCurrentScene();
    }
    public void GoToNextLevel()
    {
        Loader.LoadNextScene();
    }
}
