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

    //Tells whether the player can wall jump
    //a value of 0 means either wall, 1 means only off the left wall, and 2 means only off the right;
    public int wallJump;


    //movement parameters
    public float moveSpeed;
    public float jump;

    public float maxSpeed;
    public float currentVelocity;

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
    public Vector3 ejectDebug;
    public Vector3 originDebug;
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

        //currently unused
        circleCol = GetComponent<CircleCollider2D>();
        
        //Circle Cast components

        castRadius = playerSlimeSize / 15f;

        castDistance = Goal.ScaleSize(playerSlimeSize) * (.8f / 7.5f);
    }

    // Update is called once per frame
    void Update()
    {

        
        currentVelocity = centerBoneRB.velocity.x;
        //Cap Max Velocity
        centerBoneRB.velocity = new Vector2(Mathf.Clamp(centerBoneRB.velocity.x, maxSpeed * -1, maxSpeed), centerBoneRB.velocity.y);
        
        //Grounded Check Remove single use Variables later
        grounded = false;
        origin = transform.position;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(origin, castRadius, circleDirection, castDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit2D hit = hits[i];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("DiscardSlimeColider"))
            {
                grounded = true;
                wallJump = 0;
            }
        }

        
        

        //grounded = Physics2D.CircleCast(transform.position, Goal.ScaleSize(playerSlimeSize) / 7.5f, Vector2.down, Goal.ScaleSize(playerSlimeSize) * (.2f / 7.5f));

        //Move
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Run(movement);


        if (grounded)
        {
            splitCoolDown = false;
        }
        


        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        Debug.DrawLine(originDebug, ejectDebug, Color.green);
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
        else
        {
            //Check for wall jump
            switch (wallJump)
            {
                case 1:
                    leftWallDetection();
                    break;

                case 2:
                    rightWallDetection();
                    break;

                default:
                    leftWallDetection();
                    rightWallDetection();
                    break;
            }
        }
        void leftWallDetection()
        {
            RaycastHit2D[] left = Physics2D.CircleCastAll(origin, castRadius, Vector2.left, castDistance); 
            for (int i = 0; i < left.Length; i++)
            {
                RaycastHit2D hit = left[i];
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("DiscardSlimeColider"))
                {
                    Jump(new Vector2(1, 1));
                    wallJump = 2;
                }
            }
        }
        void rightWallDetection()
        {
            RaycastHit2D[] right = Physics2D.CircleCastAll(origin, castRadius, Vector2.right, castDistance);
            for (int i = 0; i < right.Length; i++)
            {
                RaycastHit2D hit = right[i];
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground") || hit.collider.gameObject.layer == LayerMask.NameToLayer("DiscardSlimeColider"))
                {
                    Jump(new Vector2(-1, 1));
                    wallJump = 1;
                }
            }
        }
    }

    void OnSplit()
    {  
        if ((playerSlimeSize > 1) && !splitCoolDown)
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
    
        // Prevent the current slime from splitting anymore
        splitCoolDown = true;

        //Normalize Eject for consistancy
        eject.Normalize(); // Might Be Redundant

        //Check to see if new slime will spawn in wall
        bool inWall = false;
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, eject, Goal.ScaleSize(playerSlimeSize));
        for (int i = 0; i < hits.Length; i++)
        {

            RaycastHit2D hit = hits[i];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                Debug.Log("Blarg");
                inWall = true;
            }
        }

        //Instantiate two new slimes
        GameObject newPlayerSlime = null;
        GameObject newDiscardSlime = null;

        if (inWall)
        {
            //if the player would be in a wall if they split they shouldn't spawn with an offset
            newPlayerSlime = Goal.EjectedSlimeSpawn(Vector3.zero, PrefabManager.instance.newPlayerSlime, transform.position);
            //if the player splits into a wall the discarded piece flys opisite the force
            newDiscardSlime = Goal.EjectedSlimeSpawn(eject * -1, PrefabManager.instance.discardedSlime, transform.position);
        }
        else
        {
            newPlayerSlime = Goal.EjectedSlimeSpawn(eject, PrefabManager.instance.newPlayerSlime, transform.position);
            //figure out what to do with the discarded slime
            if (!grounded)
            {
                newDiscardSlime = Goal.EjectedSlimeSpawn(eject * -1, PrefabManager.instance.discardedSlime, transform.position);
            }
            else
            {
                newDiscardSlime = Goal.EjectedSlimeSpawn(new Vector3(0, 0, 0), PrefabManager.instance.discardedSlime, transform.position);
            }
        }

        //Control the Size of the new Slimes
        Goal.ControlSize(newPlayerSlime, playerSlimeSize);
        Goal.ControlSize(newDiscardSlime, discardedSlimeSize);

        //transfer Variables to new slimes
        playerMovement newSlimeMovement = newPlayerSlime.GetComponentInChildren<playerMovement>();
        newSlimeMovement.splitCoolDown = true;
        newSlimeMovement.playerSlimeSize = playerSlimeSize;
        newSlimeMovement.discardedSlimeSize = discardedSlimeSize;


        //Discarded Slime
        deadSlime newDeadSlime = newDiscardSlime.GetComponent<deadSlime>();
        newDeadSlime.deadSize = discardedSlimeSize;
            
        //Delete existing slime
        Destroy(transform.parent.gameObject);
        
    }
    public void SpawnSlime(Vector3 eject, GameObject obj)
    {
        //Debug.Log(obj.tag + ": " + (transform.position + eject).x + "x" + (transform.position + eject).y);
        GameObject newProjectile = Instantiate(obj);
        newProjectile.transform.position = transform.position + eject;
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
        Goal.DrawCircleCast(origin, castRadius, circleDirection, castDistance);
        Goal.DrawCircleCast(origin, castRadius, Vector2.left, castDistance);
        Goal.DrawCircleCast(origin, castRadius, Vector2.right, castDistance);
    }

}
