using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class analogDirection : MonoBehaviour
{
    SpriteRenderer _sr;
    public float rightStickX;
    public float  rightStickY;
    public float stickAngle;

    public Vector2 rightStick = new Vector2(1, 1);
    public GameObject pseudoParent;
    public Sprite[] sprites;
    
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(MenuController.menuShow){ return; }
        //Create Vector Based on the right stick x and y
        rightStickX = rightStick.x;
        rightStickY = rightStick.y;
        rightStick = new Vector2(rightStickX, rightStickY);
        Debug.DrawLine(transform.position, transform.position + new Vector3(rightStickX, rightStickY, 0), Color.yellow);

        //calculate the angle of rightStick reletive to Vector2.right
        stickAngle = Vector2.SignedAngle(Vector2.right, rightStick);

        //Change rotation based on the current stickAngle
        transform.rotation = Quaternion.Euler(0, 0, stickAngle);

        //bind position of current gameObject to another with out making it a child and inheriting all of the other properties
        transform.position = pseudoParent.transform.position;

        if (rightStickX == 0 && rightStickY == 0)
        {
            Hide();
            rightStick = new Vector2(1, 1);
        }
    }

    void OnRightStick(InputValue value)
    {
        if(MenuController.menuShow){ return; }
        Show();
        rightStick = value.Get<Vector2>().normalized;
    }

    void Show()
    {
        gameObject.GetComponent<Renderer>().enabled = true;
    }
    void Hide()
    {
        gameObject.GetComponent<Renderer>().enabled = false;
    }
}
