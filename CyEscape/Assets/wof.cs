using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class wof : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach(Transform child in gameObject.transform)
        {
            child.GetComponent<Animator>().SetTrigger("isWalking");
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(-8f * Time.deltaTime, 0f, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            StartCoroutine(waiter(other));
            
        }
    }

    private IEnumerator waiter(Collider2D other)
    {
        other.gameObject.GetComponent<PlayerMovement>().enabled = false;
        other.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        other.gameObject.transform.Rotate(new Vector3(0, 0, 90));
        yield return new WaitForSeconds(2);
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
    }
}
