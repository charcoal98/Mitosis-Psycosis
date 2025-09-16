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

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Input.GetKey(KeyCode.RightArrow)) {
            _rb.AddForce(Vector2.right * horizontal, ForceMode2D.Force);
            Debug.Log("Test");
        }
        if(Input.GetKey(KeyCode.LeftArrow)) {
            _rb.AddForce(Vector2.left * horizontal, ForceMode2D.Force);
        }
        if(Input.GetKey(KeyCode.Space)) {
            _rb.AddForce(Vector2.up * jump, ForceMode2D.Force);
        }
    }
}
