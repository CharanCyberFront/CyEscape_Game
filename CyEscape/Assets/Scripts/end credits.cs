using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class endcredits : MonoBehaviour
{
    float speed = 50.0f;
    //float textPosBegin = -1100.0f;
    float boundaryTextEnd = 2200.0f;

    RectTransform myGorectTransform;
    [SerializeField]
    TextMeshProUGUI mainText;



    // Start is called before the first frame update
    void Start()
    {
        myGorectTransform = gameObject.GetComponent<RectTransform>();
        StartCoroutine(AutoScrollText());
    }

    IEnumerator AutoScrollText() 
    {
        while(myGorectTransform.localPosition.y < boundaryTextEnd)
        {
            myGorectTransform.Translate(Vector3.up * speed * Time.deltaTime);
            if(myGorectTransform.localPosition.y > boundaryTextEnd)
            {
                yield return new WaitForSeconds(5);
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 7); // Load next scene
                break;
            
            }
            yield return null;
        }
    }


}
