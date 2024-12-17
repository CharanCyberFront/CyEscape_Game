using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    private float length, startPos;
    public GameObject cam;
    public float parallaxEffect; // Speed of parallax

    void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Width of the background image
    }

    void Update()
    {
        // Parallax effect: Move background at a slower rate than the camera
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float dist = (cam.transform.position.x * parallaxEffect);

        // Update background position
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        // Check if background is out of bounds and reposition for seamless scrolling
        if (temp > startPos + length)
        {
            startPos += length;
        }
        else if (temp < startPos - length)
        {
            startPos -= length;
        }
    }
}