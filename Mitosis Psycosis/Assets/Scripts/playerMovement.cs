using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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


    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(transform.position.x + "x" + transform.position.y);
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
        castRadius = ScaleSize(playerSlimeSize) / 7.5f;
        castDistance = ScaleSize(playerSlimeSize) * (.2f / 7.5f);
        grounded = Physics2D.CircleCast(origin, castRadius, Vector2.down, castDistance);

        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Vector2 direction = new Vector2(x, y);
        Run(direction);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump(Vector2.up);
        }

        if (Input.GetAxis("Split") > 0 && (playerSlimeSize > 1))
        {
            if (!splitCoolDown)
            {
                Split();
            }
        }
        else if (grounded)
        {
            splitCoolDown = false;
        }
        if (Input.GetKey(KeyCode.Space) || (Mathf.Abs(transform.position.y) > 5f))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        /*if (Mathf.Abs(transform.position.x) > 11f)
        {
            centerBoneRB.velocity = Vector3.zero;
            float largestX = transform.position.x / Mathf.Abs(transform.position.x) * 10;
            Debug.Log(largestX);
            //Create new slime in Bounds
            SpawnSlime(new Vector3(largestX, 0, 0), newPlayerSlime);
            //Delete existing slime
            Destroy(transform.parent.gameObject);
        }*/
        
    
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
            eject = new Vector3(1,1,0).normalized;
        }
        SpawnSplitSlimes(eject);
    }
    
    public void SpawnSplitSlimes(Vector3 eject)
    {
        eject.Normalize();
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
        float totalSize = ScaleSize(discardedSlimeSize) + ScaleSize(playerSlimeSize);

        //Instantiate two new slimes
        if (!grounded){
            EjectedSlimeSpawn(eject, newPlayerSlime);
            EjectedSlimeSpawn(eject * -1, discardedSlime);
            
        } else {
            EjectedSlimeSpawn(eject*2, newPlayerSlime);
            EjectedSlimeSpawn(new Vector3(0,0,0), discardedSlime);
            
        }

        //Delete existing slime
        Destroy(transform.parent.gameObject);
    }

    public float ScaleSize(int size){
        return size * (float)1.5;
    }

    //will need to add detection in case the spawned pieces end up in a wall
    public void EjectedSlimeSpawn(Vector3 eject, GameObject obj)
    {
        Debug.Log(obj.tag + "Transform: " + transform.position.x + "x" + transform.position.y);
        Debug.Log(obj.tag + "Eject: " + eject.x + "x" + eject.y);
        GameObject newProjectile = Instantiate(obj);
        newProjectile.transform.position = transform.position + eject;
        Rigidbody2D newRB = newProjectile.transform.GetChild(0).GetComponent<Rigidbody2D>();
        newRB.velocity = Vector3.zero;
        newRB.AddForce(eject * boostForce, ForceMode2D.Impulse);

    }
    public void SpawnSlime(Vector3 eject, GameObject obj)
    {
        Debug.Log(obj.tag + ": " + (transform.position + eject).x + "x" + (transform.position + eject).y);
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
}
