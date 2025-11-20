using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        castDistance = MasterController.ScaleSize(playerSlimeSize) * (.8f / 7.5f);
        //DebugValues();

        discardedSlimeSize = MasterController.getDiscardedSlimeSize(playerSlimeSize);
        MasterController.ControlSize(transform.parent.gameObject, playerSlimeSize);
    }

    // Update is called once per frame
    void Update()
    {
        if(MenuController.menuShow){ return; }

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
            MasterController.restartScene();
        }
        Debug.DrawLine(originDebug, ejectDebug, Color.green);
    }

    //controller Events
    void OnMove(InputValue value)
    {
        if(MenuController.menuShow){ return; }
        movement = value.Get<Vector2>();
    }

    void OnJump()
    {
        if(MenuController.menuShow){ return; }
        if (grounded)
        {
            Jump(Vector2.up);
        }
        else//WallJump
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
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("WallJump") || hit.collider.gameObject.layer == LayerMask.NameToLayer("DiscardSlimeColider"))
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
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("WallJump") || hit.collider.gameObject.layer == LayerMask.NameToLayer("DiscardSlimeColider"))
                {
                    Jump(new Vector2(-1, 1));
                    wallJump = 1;
                }
            }
        }
    }

    void OnSplit()
    {  
        if(MenuController.menuShow){ return; }
        if ((playerSlimeSize > 1) && !splitCoolDown)
        {
            Split();
        }
    }

    void OnPause(){
        if (!MenuController.dead){
            if (MenuController.menuShow){
                MenuController.instance.Hide();
            }else{
                MenuController.instance.Show();
            }
        }
    }

    public void Die()
    {
        Debug.Log("You Are Dead, Not Big Suprise");
        //play a sound or something
        MenuController.instance.Show();
        MenuController.instance.ShowDeathMenu();
        
    }
    public void Split()
    {
        //calculate sizes of the new slimes
        discardedSlimeSize = MasterController.getDiscardedSlimeSize(playerSlimeSize);
        playerSlimeSize -= discardedSlimeSize;

        if (playerSlimeSize < 1) playerSlimeSize = 1;
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
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, eject, MasterController.ScaleSize(playerSlimeSize));
        for (int i = 0; i < hits.Length; i++)
        {

            RaycastHit2D hit = hits[i];
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                inWall = true;
            }
        }

        //Instantiate two new slimes
        GameObject newPlayerSlime = null;
        GameObject newDiscardSlime = null;

        if (inWall)
        {
            //if the player would be in a wall if they split they shouldn't spawn with an offset
            newPlayerSlime = MasterController.EjectedSlimeSpawn(Vector3.zero, PrefabManager.instance.newPlayerSlime, transform.position);
            //if the player splits into a wall the discarded piece flys opisite the force
            newDiscardSlime = MasterController.EjectedSlimeSpawn(eject * -1, PrefabManager.instance.discardedSlime, transform.position);
        }
        else
        {
            newPlayerSlime = MasterController.EjectedSlimeSpawn(eject, PrefabManager.instance.newPlayerSlime, transform.position);
            //figure out what to do with the discarded slime
            if (!grounded)
            {
                newDiscardSlime = MasterController.EjectedSlimeSpawn(eject * -1, PrefabManager.instance.discardedSlime, transform.position);
            }
            else
            {
                newDiscardSlime = MasterController.EjectedSlimeSpawn(new Vector3(0, 0, 0), PrefabManager.instance.discardedSlime, transform.position);
            }
        }

        //Control the Size of the new Slimes
        MasterController.ControlSize(newPlayerSlime, playerSlimeSize);
        MasterController.ControlSize(newDiscardSlime, discardedSlimeSize);

        //transfer Variables to new slimes
        playerMovement newSlimeMovement = newPlayerSlime.GetComponentInChildren<playerMovement>();
        newSlimeMovement.splitCoolDown = true;
        newSlimeMovement.playerSlimeSize = playerSlimeSize;
        newSlimeMovement.discardedSlimeSize = discardedSlimeSize;


        //Discarded Slime
        deadSlime newDeadSlime = newDiscardSlime.GetComponentInChildren<deadSlime>();
        newDeadSlime.deadSize = discardedSlimeSize;
            
        //Delete existing slime
        Destroy(transform.parent.gameObject);
        
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
        MasterController.DrawCircleCast(origin, castRadius, circleDirection, castDistance);
        MasterController.DrawCircleCast(origin, castRadius, Vector2.left, castDistance);
        MasterController.DrawCircleCast(origin, castRadius, Vector2.right, castDistance);
    }

    void DebugValues()
    {
        Debug.Log("Player Slime Size" + playerSlimeSize);
        Debug.Log("Discarded Slime Size" + discardedSlimeSize);
    }
}
