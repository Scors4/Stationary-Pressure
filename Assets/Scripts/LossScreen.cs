using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LossScreen : MonoBehaviour
{
    public Text LossAmountLine;

    public void Start()
    {
        int dollar = Random.Range(1000000, 999999999);
        int cent = Random.Range(0, 99);
        LossAmountLine.text = "$" + dollar.ToString() + "." + cent.ToString("d2");
    }

    public void OnRetryPress()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("DevScene");
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
