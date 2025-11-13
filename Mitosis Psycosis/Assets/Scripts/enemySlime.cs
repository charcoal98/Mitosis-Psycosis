using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySlime : MonoBehaviour
{
    public int enemySize = 3;

    void Start()
    {
        MasterController.ControlSize(transform.parent.gameObject, enemySize);
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInChildren<playerMovement>() && other.gameObject.GetComponentInChildren<playerMovement>().playerSlimeSize < 5)
        {
            playerMovement pm = other.gameObject.GetComponentInChildren<playerMovement>();
            if (pm.playerSlimeSize < enemySize)
            {
                //Damage Slime
                pm.Die();
            }else if (!pm.splitCoolDown && !MasterController.mergeInProgress)
            {
                MasterController.Merge(other.gameObject, transform.gameObject, enemySize, pm);
            }
        }
    }
}
