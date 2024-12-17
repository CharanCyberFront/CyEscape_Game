using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public GameObject accessCard;


    public void ObtainAccessCard()
    {
        accessCard.SetActive(false);
        
        Debug.Log("Access card obtained!");
    }
}