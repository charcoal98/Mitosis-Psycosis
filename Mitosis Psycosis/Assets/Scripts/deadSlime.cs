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
        
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInChildren<playerMovement>())
        {
            playerMovement pm = other.gameObject.GetComponentInChildren<playerMovement>();
            if (!pm.splitCoolDown)
            {
                Goal.Merge(other.gameObject, transform.gameObject, deadSize, pm);
            }
        }
    }

    
}
