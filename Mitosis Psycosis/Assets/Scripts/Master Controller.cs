using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MasterController : MonoBehaviour
{
    //Contains a bunch of static methods
    static public float boostForce = 87.5f;
    static public bool hasKey = false;
    static int lastMethodCallFrameCount;

    //Static Functions
    //Returns the size of the transform at current slimeSize
    static public float ScaleSize(int size)
    {
        if(size == 1)
        {
            return size * 2;
        } 
        return size * (float)1.5;
    }

    //Draws a debug circle of what a circle cast would be
    static public void DrawCircleCast(Vector2 start, float radius, Vector2 direction, float distance)
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(start, radius);

        Gizmos.color = Color.yellow;
        Vector2 endPosition = start + direction * distance;
        Gizmos.DrawWireSphere(endPosition, radius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(start, endPosition);
    }

    static public void ControlSize(GameObject slime, int size)
    {
        SpriteRenderer _sr = slime.GetComponent<SpriteRenderer>();
        switch (size)
        {
            case 5:
                _sr.color = Color.red;
                break;
            case 4:
                _sr.color = Color.yellow;
                break;
            case 3:
                _sr.color = Color.green;
                break;
            case 2:
                _sr.color = Color.cyan;
                break;
            case 1:
                _sr.color = Color.magenta;
                break;
            default:
                return;
        }
        slime.transform.localScale = new Vector3(MasterController.ScaleSize(size), MasterController.ScaleSize(size), 1);
    }
    static public GameObject EjectedSlimeSpawn(Vector3 eject, GameObject obj, Vector3 startingPosition)
    {
        //Create object
        GameObject newProjectile = Instantiate(obj);

        //set object position

        newProjectile.transform.position = startingPosition + eject;

        //Add Force
        Rigidbody2D newRB = newProjectile.transform.GetChild(0).GetComponent<Rigidbody2D>();
        //newRB.velocity = centerBoneRB.velocity;
        newRB.AddForce(eject * boostForce, ForceMode2D.Impulse);

        return newProjectile;
    }

    static public void Merge(GameObject playerSlime, GameObject mergedSlime, int mergeSize, playerMovement pm)
    {
        if(lastMethodCallFrameCount == Time.frameCount) { return; } lastMethodCallFrameCount = Time.frameCount;
        SoundManager.instance.PlayAbsorb();
        if (pm.playerSlimeSize >= mergeSize)
        {
            //Debug.Log(pm.playerSlimeSize);
            float x = ((playerSlime.transform.position.x - mergedSlime.transform.position.x) / 2) + mergedSlime.transform.position.x;
            float y = ((playerSlime.transform.position.y - mergedSlime.transform.position.y) / 2) + mergedSlime.transform.position.y;
            Vector3 spawnPoint = new Vector3(x, y, 0);
            //Instantiate newPlayerSlime
            GameObject newPlayerSlime = null;
            //Spawn New Player Slime
            newPlayerSlime = MasterController.EjectedSlimeSpawn(Vector3.zero, PrefabManager.instance.newPlayerSlime, spawnPoint);
            //give new player slime velocity
            //newPlayerSlime.transform.GetChild.velocity = playerSlime.transform.velocity;
            int newSlimeSize = pm.playerSlimeSize + mergeSize;
            int remainder = 0;

            //set the upperbound of slime size and create a remainder slime for conservation of mass
            if (newSlimeSize > 5) { 
                remainder = newSlimeSize - 5;
                newSlimeSize = 5; 
            }
            //Control the Size of the new Slimes
            ControlSize(newPlayerSlime, newSlimeSize);

            //Transfer variables to new slime
            playerMovement newSlimeMovement = newPlayerSlime.GetComponentInChildren<playerMovement>();

            //newSlimeMovement.splitCoolDown = true; //prevents the player from merging imediatly after splitting
            newSlimeMovement.playerSlimeSize = newSlimeSize;
            newSlimeMovement.discardedSlimeSize = getDiscardedSlimeSize(newSlimeSize);

            //Destroy old Slimes
            Destroy(playerSlime.transform.parent.gameObject);
            Destroy(mergedSlime.transform.parent.gameObject);

            //Spawn remainder Slime if needed
            if (remainder != 0)
            {
                GameObject remainderSlime = null;
                Vector3 remainderSpawnPoint = new Vector3(x - (ScaleSize(5)/4),y, 0);
                remainderSlime = MasterController.EjectedSlimeSpawn(Vector3.zero, PrefabManager.instance.discardedSlime, remainderSpawnPoint);
                ControlSize(remainderSlime, remainder);
                deadSlime remainderSlimeScript = remainderSlime.GetComponentInChildren<deadSlime>();
                remainderSlimeScript.deadSize = remainder;
            }
        }
    }

    static public int getDiscardedSlimeSize(int playerSlimeSize)
    {
        switch (playerSlimeSize)
        {
            case 5:
                return 2;
            case 4:
                return 2;
            case 3:
                return 1;
            case 2:
                return 1;
            default:
                return 0;
        }
    }

    static public void restartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    static public void blarg()
    {
        Debug.Log("blarg");
    }


    
    // Start is called before the first frame update
    void Start()
    {
       Debug.Log(SceneManager.GetActiveScene().name); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
