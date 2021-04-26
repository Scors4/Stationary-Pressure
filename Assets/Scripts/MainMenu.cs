using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject MainMenuView;
    public GameObject InstructionsView;
     public GameObject CreditsView;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void Play() {
        SceneManager.LoadScene("DevScene");
    }
    
    public void ShowInstructions() {
        CreditsView.active = false;
        MainMenuView.active = false;
        InstructionsView.active = true;
    }
    
     public void ShowMainMenu() {
        MainMenuView.active = true;
        InstructionsView.active = false;
        CreditsView.active = false;
    }
    
    public void ShowCredits() {
        CreditsView.active = true;
        MainMenuView.active = false;
        InstructionsView.active = false;
    }
    
     public void Quit() {
        Application.Quit();
    }
}
