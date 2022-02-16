using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    float speed = 12f;

    void Start()
    {
        
    }

    void Update()
    {
        
        Vector3 cursorPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        if (cursorPos.x <= 0 && transform.position.x > -20)
        {
            transform.position = transform.position + Vector3.left * Time.deltaTime * speed; 
        }
        if (cursorPos.x >= 0.95f && transform.position.x < 20)
        {
            transform.position = transform.position + Vector3.right * Time.deltaTime * speed;
        }
        if (cursorPos.y <= 0 && transform.position.z > -15)
        {
            transform.position = transform.position + Vector3.back * Time.deltaTime * speed;
        }
        if (cursorPos.y >= 0.95f && transform.position.z < 15)
        {
            transform.position = transform.position + Vector3.forward * Time.deltaTime * speed;
        }
        
    }
}
