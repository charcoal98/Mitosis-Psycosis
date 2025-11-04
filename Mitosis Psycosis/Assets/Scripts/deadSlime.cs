using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadSlime : MonoBehaviour
{

    public int deadSize;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionStay2D(Collision2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerSlimeColider"))
        {
            Debug.Log("Blarg");
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<playerMovement>())
        {
            Debug.Log("Blarg");
        }
    }
    void Merge(GameObject playerSlime)
    {
        
    }
}
