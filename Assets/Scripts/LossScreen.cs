using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LossScreen : MonoBehaviour
{
    public Text LossAmountLine;
    public Text TimeSurvived;

    public void Start()
    {
        long dollar = (long)Random.Range(1000000, 99999999999);
        int cent = Random.Range(0, 99);
        LossAmountLine.text = "#" + dollar.ToString("N0") + "." + cent.ToString("d2");

        /*float amountLoss = Random.Range(1000000f, 9999999999f);
        amountLoss += Random.Range(0.0f, 0.99f);
        LossAmountLine.text = amountLoss.ToString("C");*/
        
        TimeSurvived.text = "You have died after " + Util.formatTime(GameMgr.inst.timeSurvived);
    }

    public void OnRetryPress()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("StationLevel");
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
