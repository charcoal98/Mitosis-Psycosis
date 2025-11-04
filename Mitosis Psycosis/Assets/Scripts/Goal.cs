using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour
{
    //Static Functions
    //Returns the size of the transform at current slimeSize
    static public float ScaleSize(int size)
    {
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
                _sr.color = Color.blue;
                break;
            default:
                return;
        }
        slime.transform.localScale = new Vector3(Goal.ScaleSize(size), Goal.ScaleSize(size), 1);
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
        newRB.AddForce(eject, ForceMode2D.Impulse);

        return newProjectile;
    }

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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
