using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameracontroller6 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, -2.77f, transform.position.z);
    }
}
