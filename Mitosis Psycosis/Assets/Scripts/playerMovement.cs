using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    //Outlets
    Rigidbody2D _rb;

    //Config
    public bool grounded;
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
        grounded = Physics2D.CircleCast(transform.position, 0.14f, Vector2.down, 0.05f);
        float y = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        
        Vector2 direction = new Vector2(x, y);
        Run(direction);

        if (Input.GetButtonDown("Jump") && grounded)
        {
            Jump(Vector2.up);
        }


    }

    public void Split()
    {
        Instantiate(discSlime, Vector2.up, Quaternion.identity);
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
