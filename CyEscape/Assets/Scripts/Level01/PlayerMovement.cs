using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller; // Reference to character controller script

    public Animator animator;

    public float runSpeed = 60f; // Speed at which the player runs

    float horizontalMove = 0f; // Horizontal movement input

    bool jump = false; //Jump input 
    bool crouch = false; //crouch input

    // Update is called once per frame
    void Update () {
        // Get horizontal input (left/right movement)
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;  

        // If horizontalMove is not zero, the player is moving. Otherwise, the player is idle.
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));  

        if (Input.GetButtonDown("Jump")) {
            jump = true;  
        }

        if (SceneManager.GetActiveScene().name.Equals("Level06"))
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                animator.SetBool("crouch", true);
                GetComponent<SpriteRenderer>().flipX = true;
                crouch = true;
            }
            else if (Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S))
            {
                animator.SetBool("crouch", false);
                GetComponent<SpriteRenderer>().flipX = false;
                crouch = false;
            }
        }
    }

    // FixedUpdate is called at a fixed interval, ideal for physics calculations
    void FixedUpdate () {
        // Move the player character
        //Debug.Log(Input.GetAxisRaw("Horizontal")); 
        controller.Move(horizontalMove * Time.fixedDeltaTime, crouch, jump); 
        jump = false; //Stop Jump  
        
    }
}
 