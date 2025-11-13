using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadSlime : MonoBehaviour
{
    public int deadSize;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInChildren<playerMovement>() && other.gameObject.GetComponentInChildren<playerMovement>().playerSlimeSize < 5)
        {
            playerMovement pm = other.gameObject.GetComponentInChildren<playerMovement>();
            if (!pm.splitCoolDown && !MasterController.mergeInProgress)
            {
                MasterController.Merge(other.gameObject, transform.gameObject, deadSize, pm);
            }
        }
    }
    
}
