using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    //Outlets
    Rigidbody2D centerBoneRB;

    SpriteRenderer newPlayerSlimeSprite;

    CircleCollider2D circleCol;

    //Tells whether player is on ground
    public bool grounded;

    //boost force
    public float boostForce;

    //movement parameters
    public float moveSpeed;
    public float jump;

    //Size of the player Slime and the discarded component
    public int playerSlimeSize;
    public int discardedSlimeSize;
    
    //Game object for the arrow
    public GameObject arrow;
    public GameObject parentSlime;
    public analogDirection anaDirection;

    //Determins whether the player can split
    public bool splitCoolDown;
    
    //temporary variables for testing

    //circlecast
    public Vector3 DebugEject;
    public Vector3 DebugOrigin;
    Vector2 movement;

    //Circle Cast Debug
    float castRadius;
    float castDistance;
    Vector2 circleDirection = Vector2.down;
    Vector2 origin;

    // Start is called before the first frame update
    void Start()
    {
        //get components from current gameObject
        centerBoneRB = GetComponent<Rigidbody2D>();

        //get components from arrow
        anaDirection = arrow.GetComponent<analogDirection>();


        circleCol = GetComponent<CircleCollider2D>();
        
        //Circle Cast components

        castRadius = ScaleSize(playerSlimeSize) / 8f;

        castDistance = ScaleSize(playerSlimeSize) * (.3f / 7.5f);
    }

    // Update is called once per frame
    void Update()
    {   //Grounded Check Remove single use Variables later
        grounded = false;
        origin = transform.position;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, castRadius, circleDirection, castDistance);
        Debug.Log(hits.Length);
        for (int i = 0; i < hits.Length; i++)
        {
            
            RaycastHit2D hit = hits[i];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                grounded = true;
                
            }
        }

        //grounded = Physics2D.CircleCast(transform.position, ScaleSize(playerSlimeSize) / 7.5f, Vector2.down, ScaleSize(playerSlimeSize) * (.2f / 7.5f));

        //Move
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Run(movement);

        if (grounded)
        {
            splitCoolDown = false;
        }

        if (Input.GetKey(KeyCode.Space) /*|| (transform.position.y < -16f)*/)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        /*if (Mathf.Abs(transform.position.x) > 11f)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }*/

        Debug.DrawLine(DebugOrigin, DebugEject, Color.green);
    }

    //controller Events
    void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    void OnJump()
    {
        if (grounded)
        {
            Jump(Vector2.up);
        }
    }

    void OnSplit()
    {  
        if ((playerSlimeSize > 1) && !(splitCoolDown))
        {
            Split();
        }
    }

    public void Split()
    {
        switch (playerSlimeSize)
        {
            case 5:
                playerSlimeSize = 3;
                discardedSlimeSize = 2;
                break;
            case 4:
                playerSlimeSize = 2;
                discardedSlimeSize = 2;
                break;
            case 3:
                playerSlimeSize = 2;
                discardedSlimeSize = 1;
                break;
            case 2:
                playerSlimeSize = 1;
                discardedSlimeSize = 1;
                break;
            default:
                return;
        }
        Vector3 eject;
        if (Mathf.Abs(anaDirection.rightStickX) > 0 || Mathf.Abs(anaDirection.rightStickY) > 0)
        {
            eject = new Vector3(anaDirection.rightStickX, anaDirection.rightStickY, 0).normalized;
        }
        else
        {
            eject = new Vector3(1, 1, 0).normalized;
        }
        SpawnSplitSlimes(eject);
    }
    
    public void SpawnSplitSlimes(Vector3 eject) {
        // Prevent the current slime from splitting anymore
        splitCoolDown = true;

        eject.Normalize();
        Debug.Log(eject.magnitude);
        
        //Instantiate two new slimes
        GameObject newPlayerSlime = null;
        GameObject newDiscardSlime = null;

        
        newPlayerSlime = EjectedSlimeSpawn(eject, PrefabManager.instance.newPlayerSlime);

        //figure out what to do with the discarded slime
        if (!grounded)
        {
            newDiscardSlime = EjectedSlimeSpawn(eject * -1, PrefabManager.instance.discardedSlime);
        } else {
            newDiscardSlime = EjectedSlimeSpawn(new Vector3(0, 0, 0), PrefabManager.instance.discardedSlime);
        }
        
        ControlSize(newPlayerSlime, playerSlimeSize);
        ControlSize(newDiscardSlime, discardedSlimeSize);
        
        //transfer Variables
        playerMovement newSlimeMovement = newPlayerSlime.GetComponentInChildren<playerMovement>();
        newSlimeMovement.splitCoolDown = true;
        newSlimeMovement.playerSlimeSize = playerSlimeSize;
        newSlimeMovement.discardedSlimeSize = discardedSlimeSize;

        //Delete existing slime
        Destroy(transform.parent.gameObject);
    }

    

    //will need to add detection in case the spawned pieces end up in a wall
    public GameObject EjectedSlimeSpawn(Vector3 eject, GameObject obj)
    {
        // Debug.Log(obj.tag + " Transform: " + transform.position.x + "x" + transform.position.y);
        // Debug.Log(obj.tag + " Eject: " + eject.x + "x" + eject.y);

        //Create object
        Debug.DrawLine(transform.position, transform.position + eject, Color.yellow);
        DebugEject = eject + transform.position;
        DebugOrigin = transform.position;

        GameObject newProjectile = Instantiate(obj);

        //set object position
        
        newProjectile.transform.position = transform.position + eject;
        //Debug.Log(obj.tag + " Object Transform: " + newProjectile.transform.position.x + "x" + newProjectile.transform.position.y);

        //Add Force
        Rigidbody2D newRB = newProjectile.transform.GetChild(0).GetComponent<Rigidbody2D>();
        newRB.velocity = Vector3.zero;
        newRB.AddForce(eject * boostForce, ForceMode2D.Impulse);

        return newProjectile;
    }
    public void SpawnSlime(Vector3 eject, GameObject obj)
    {
        //Debug.Log(obj.tag + ": " + (transform.position + eject).x + "x" + (transform.position + eject).y);
        GameObject newProjectile = Instantiate(obj);
        newProjectile.transform.position = transform.position + eject;
    }

    public void ControlSize(GameObject slime, int size)
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
        slime.transform.localScale = new Vector3(ScaleSize(size), ScaleSize(size), 1);
    }

    //Returns the size of the transform at current slimeSize
    public float ScaleSize(int size)
    {
        return size * (float)1.5;
    }

    //Movement
    public void Run(Vector2 dir)
    {
        centerBoneRB.AddForce(new Vector2(dir.x * moveSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
    }
    public void Jump(Vector2 dir)
    {
        centerBoneRB.AddForce(dir * jump, ForceMode2D.Impulse);
        // centerBoneRB.velocity += dir * jump;
    }

    //Draw Circle Cast for debuging
    void OnDrawGizmos()
    {        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, castRadius);

        Gizmos.color = Color.yellow;
        Vector2 endPosition = origin + circleDirection * .2f;
        Gizmos.DrawWireSphere(endPosition, castRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, endPosition);
    }
}
