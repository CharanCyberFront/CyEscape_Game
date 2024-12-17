using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    public GameObject concealPanel;

    public void InfoScreen()
    {
        concealPanel.SetActive(true);
    }
    public void BacktoMenu()
    {
        concealPanel.SetActive(false);
    }
}
