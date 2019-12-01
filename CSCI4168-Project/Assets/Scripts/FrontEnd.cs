using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrontEnd : MonoBehaviour
{
    public GameObject mainPanel;
    public GameObject rulesPanel;

    public void ShowRulesPanel()
    {
        mainPanel?.SetActive(false);
        rulesPanel?.SetActive(true);
    }

    public void ShowMainPanel()
    {
        mainPanel?.SetActive(true);
        rulesPanel?.SetActive(false);
    }
}
