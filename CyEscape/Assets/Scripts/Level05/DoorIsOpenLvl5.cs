using UnityEngine;

public class DoorIsOpenLvl5 : MonoBehaviour
{
    public Animator doorAnimator;
    private bool isEnabled;

    void Awake()
    {
        this.enabled = isEnabled = false;
    }

    void Update()
    {
        isEnabled = this.enabled;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isEnabled)
        {
            doorAnimator.SetBool("isOpen", true);
            Debug.Log("Player entered. Door is opening.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && isEnabled)
        {
            doorAnimator.SetBool("isOpen", false);
            Debug.Log("Player exited. Door is closing.");
        }
    }
}