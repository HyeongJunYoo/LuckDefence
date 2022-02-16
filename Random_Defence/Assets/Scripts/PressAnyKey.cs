using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PressAnyKey : MonoBehaviour
{

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x > 90)
            transform.position = new Vector3(-90, 0, 0);
        transform.position += Vector3.right * Time.deltaTime * 5f;

        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
