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

    //Tells whether player is on ground
    public bool grounded;

    //boost force
    public int boostForce;

    //movement parameters
    public float moveSpeed;
    public float jump;

    //Game object for the discarded slime
    public GameObject discardedSlime;
    public deadSlime discardedSlimeScript;
    

    //Game object for the player slime
    public GameObject newPlayerSlime;
    public playerMovement slimeMovement;

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
    public float castRadius = 1.0f;
    public float castDistance = 0.2f;
    public Vector2 direction2 = Vector2.down;
    public Vector2 origin;
    Vector2 movement;


    // Start is called before the first frame update
    void Start()
    {
        //get components from parent
        //_parentrb = GetComponentInParent<Rigidbody2D>();
        
        //get components from current gameObject
        centerBoneRB = GetComponent<Rigidbody2D>();

        //get components from arrow
        anaDirection = arrow.GetComponent<analogDirection>();

        //get player and discared slime scripts
        discardedSlimeScript = discardedSlime.GetComponent<deadSlime>();
        slimeMovement = newPlayerSlime.GetComponentInChildren<playerMovement>();
    }

    // Update is called once per frame
    void Update()
    {   //Needs Fixing to scale with the slime
        origin = transform.position;
        castRadius = scaleSize(playerSlimeSize) / 7.5f;
        castDistance = scaleSize(playerSlimeSize) * (.2f / 7.5f);
        grounded = Physics2D.CircleCast(origin, castRadius, Vector2.down, castDistance);
        Run(movement);
        /*float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Vector2 direction = new Vector2(x, y);
        Run(direction);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump(Vector2.up);
        }*/


        if (grounded)
        {
            splitCoolDown = false;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

        }
    }
    void OnMove(InputValue value)
    {
        movement = value.Get<Vector2>();
    }

    void OnJump()
    {
        Jump(Vector2.up);
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
            eject = new Vector3((anaDirection.rightStickX * scaleSize(playerSlimeSize)), (anaDirection.rightStickY * scaleSize(playerSlimeSize)), 0);
        }
        else
        {
            eject = new Vector3(1, 1, 0);
        }
        SpawnSplitSlimes(eject);
    }
    
    public void SpawnSplitSlimes(Vector3 eject)
    {
        //discardedSlime.playerSlimeSize = discardedSlimeSize;
        //Transform objects before spawning them
        ControlSize(newPlayerSlime, playerSlimeSize);
        ControlSize(discardedSlime, discardedSlimeSize);

        //transfer Variables
        //newPlayerSlime
        slimeMovement.splitCoolDown = true;
        slimeMovement.playerSlimeSize = playerSlimeSize;
        slimeMovement.discardedSlimeSize = discardedSlimeSize;
        newPlayerSlime.transform.position = transform.position;

        //DiscardedSlime
        discardedSlime.transform.position = transform.position;


        
        //Instantiate two new slimes
        float totalSize = scaleSize(discardedSlimeSize) + scaleSize(playerSlimeSize);

        //Instantiate two new slimes
        if (!grounded){
            ejectedSlimeSpawn(eject * -1 /2, discardedSlime);
            ejectedSlimeSpawn(eject / 2, newPlayerSlime);
        } else {
            ejectedSlimeSpawn(new Vector3(0,0,0), discardedSlime);
            ejectedSlimeSpawn((eject), newPlayerSlime);
        }

        //Delete existing slime
        Destroy(transform.parent.gameObject);
    }

    public float scaleSize(int size){
        return size * (float)1.5;
    }

    public void ejectedSlimeSpawn(Vector3 eject, GameObject obj){
        GameObject newProjectile = Instantiate(obj);
        newProjectile.transform.position = transform.position + eject;
        Rigidbody2D newRB = newProjectile.transform.GetChild(0).GetComponent<Rigidbody2D>();
        newRB.velocity = Vector3.zero;
        newRB.AddForce(eject * boostForce, ForceMode2D.Impulse);
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
        slime.transform.localScale = new Vector3(scaleSize(size), scaleSize(size), 1);
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
    /*void OnDrawGizmos()
    {        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin, castRadius);
        
        Gizmos.color = Color.yellow;
        Vector2 endPosition = origin + Vector2.down * .2f;
        Gizmos.DrawWireSphere(endPosition, castRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, endPosition);
    }*/
}
