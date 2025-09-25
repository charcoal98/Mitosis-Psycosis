using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    //Outlets
    Rigidbody2D _rb;
    //Rigidbody2d _parentrb;
    SpriteRenderer _sr;

    //Config

    //Tells whether player is on ground
    public bool grounded;
    //movement parameters
    public float moveSpeed;
    public float jump;
    //Game object for the discarded slime
    public GameObject discSlimeObj;
    public deadSlime discSlimeScript;
    public int discSize;
    //Game object for the arrow
    public GameObject arrow;
    public GameObject parentSlime;
    public analogDirection anaDirection;
    //Determins whether the player can split
    public bool splitCoolDown = false;
    //Size of the player Slime and the discarded component
    public int slimeSize = 5;


    // Start is called before the first frame update
    void Start()
    {
        //get components from parent
        //_parentrb = GetComponentInParent<Rigidbody2D>();
        _sr = GetComponentInParent<SpriteRenderer>();
        //get components from current gameObject
        _rb = GetComponent<Rigidbody2D>();
        //get components from arrow
        anaDirection = arrow.GetComponent<analogDirection>();
        discSlimeScript = discSlimeObj.GetComponent<deadSlime>();

        //Initalize the slime scale and color
        ControlSize();
    }

    // Update is called once per frame
    void Update()
    {   //Needs Fixing to scale with the slime
        grounded = Physics2D.CircleCast(transform.position, transform.parent.localScale.x / 30, Vector2.down, .14f);
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");

        Vector2 direction = new Vector2(x, y);
        Run(direction);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump(Vector2.up);
        }
        if (Input.GetAxis("Split") > 0 && (slimeSize > 1))
        {
            if (!splitCoolDown)
            {
                Split();
                splitCoolDown = true;
            }
        }
        else if (grounded)
        {
            splitCoolDown = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            slimeSize = 4;
            Jump(Vector2.up);
            ControlSize();
        }
    }

    public void Split()
    {
        switch (slimeSize)
        {
            case 5:
                slimeSize = 3;
                discSize = 2;
                break;
            case 4:
                slimeSize = 2;
                discSize = 2;
                break;
            case 3:
                slimeSize = 2;
                discSize = 1;
                break;
            case 2:
                slimeSize = 1;
                discSize = 1;
                break;
            default:
                return;
        }
        if (Mathf.Abs(anaDirection.rightStickX) > 0 || Mathf.Abs(anaDirection.rightStickY) > 0)
        {
            Boost(anaDirection.rightStick);
        }
        else
        {
            Boost(new Vector2(1, 1));
        }
        ControlSize();
        SpawnDesc();




    }
    public void ControlSize()
    {
        switch (slimeSize)
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
        transform.parent.localScale = new Vector3(slimeSize * (float)1.5, slimeSize * (float)1.5, 1);
    }
    public void SpawnDesc()
    {
        //Debug.Log(transform.localScale.x);
        //transform eject = new transform();
        //Posistion of eject transform is found 
        //Vector3 eject = new Vector3((anaDirection.rightStickX * transform.localScale.x * -1), (anaDirection.rightStickY * transform.localScale.y * -1), 0);
        //discSlime.slimeSize = discSize;
        //Instantiate(discSlime, eject, Quaternion.identity);
    }
    public void Run(Vector2 dir)
    {
        _rb.AddForce(new Vector2(dir.x * moveSpeed * Time.deltaTime, 0), ForceMode2D.Impulse);
    }
    public void Jump(Vector2 dir)
    {
        _rb.AddForce(dir * jump, ForceMode2D.Impulse);
        // _rb.velocity += dir * jump;
    }
    public void Boost(Vector2 dir)
    {
        Debug.Log(dir.x + " " + dir.y);
        Vector2 x = dir * 10 * slimeSize;
        Debug.Log(x.x + " " + x.y);
        _rb.AddForce(dir * 10 * slimeSize, ForceMode2D.Impulse);
    }
}
