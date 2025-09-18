using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplitTest : MonoBehaviour
{
    public GameObject discSlime;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(discSlime, Vector2.up, Quaternion.identity);
        }
    }
}
