using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadSlime : MonoBehaviour
{
    public int deadSize;
    Rigidbody2D centerBoneRB;
    void start(){
        centerBoneRB = GetComponent<Rigidbody2D>();
    }
    void update()
    {
        centerBoneRB.velocity = new Vector2(Mathf.Clamp(centerBoneRB.velocity.x, -20, 20), Mathf.Clamp(centerBoneRB.velocity.y, -110, 120));
    }
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
