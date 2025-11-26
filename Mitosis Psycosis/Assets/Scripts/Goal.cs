using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Acts as a gamecontroller as well since there should only be one goal per level


public class Goal : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<playerMovement>())
        {
            SoundManager.instance.PlayVictory();
            MenuController.instance.Show();
        }
    }
}
