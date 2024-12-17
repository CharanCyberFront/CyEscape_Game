using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    public GameObject concealPanel;
    // Start is called before the first frame update
    void Start()
    {
        concealPanel.SetActive(false);
    }

}
