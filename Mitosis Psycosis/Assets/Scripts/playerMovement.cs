using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    //Outlets
    Rigidbody2D _rb;

    //Config
    public float horizontal = 2;
    public float jump = 2;

    public GameObject discSlime;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontral");
        
        Vector2 direction = new Vector2(x, y);

        if (Input.GetButtonDown("Jump"))
        {
            Jump(Vector2.up);
        }
        // if (Input.GetKey(KeyCode.LeftArrow))
        //     {
        //         transform.position += new Vector3(-0.2f, 0, 0);
        //     }
        // if (Input.GetKey(KeyCode.RightArrow))
        // {
        //     transform.position += new Vector3(0.2f, 0, 0);
        // }
        // if (Input.GetKey(KeyCode.Space))
        // {
        //     _rb.AddForce(Vector2.up * jump, ForceMode2D.Force);
        // }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(discSlime, Vector2.up, Quaternion.identity);
        }

    }
    public void Run(Vector2 dir)
    {
        _rb.AddForce(new Vector2(dir.x * horizontal, 0));
    }
    public void Jump(Vector2 dir)
    {
        _rb.velocity = new Vector2(_rb.velocity.x, 0);
        _rb.velocity += dir * jump;
    }
}
