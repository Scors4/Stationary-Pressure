using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public void OnResumePressed()
    {
        GameMgr.inst.UnpauseGame();
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
