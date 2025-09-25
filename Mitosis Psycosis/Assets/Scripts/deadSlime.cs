using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class deadSlime : MonoBehaviour
{
    public int slimeSize;
    SpriteRenderer _sr;
    // Start is called before the first frame update
    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();
        ControlSize();
    }

    // Update is called once per frame
    void Update()
    {
        ControlSize();
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
        transform.localScale = new Vector3(slimeSize * (float)1.5,slimeSize * (float)1.5,1);
    }
}
