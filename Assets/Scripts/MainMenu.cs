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
        CreditsView.SetActive(false);
        MainMenuView.SetActive(false);
        InstructionsView.SetActive(true);
    }
    
     public void ShowMainMenu() {
        MainMenuView.SetActive(true);
        InstructionsView.SetActive(false);
        CreditsView.SetActive(false);
    }
    
    public void ShowCredits() {
        CreditsView.SetActive(true);
        MainMenuView.SetActive(false);
        InstructionsView.SetActive(false);
    }
    
     public void Quit() {
        Application.Quit();
    }
}
