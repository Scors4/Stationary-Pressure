using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Debug.Log("Retry has been pressed.");

        int dollar = Random.Range(1000000, 999999999);
        int cent = Random.Range(0, 99);
        LossAmountLine.text = "$" + dollar.ToString() + "." + cent.ToString("d2");
    }

    public void OnExitPressed()
    {
        Application.Quit();
    }
}
