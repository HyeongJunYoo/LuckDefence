using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public GameObject target;
    public GameObject effect;
    public int speed;
    public int dmg;

    void Start()
    {

    }


    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, speed * Time.deltaTime);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == target)
        {
            Destroy(gameObject);
            Destroy(Instantiate(effect, target.transform.position + Vector3.up, Quaternion.identity), 1f);
            target.GetComponent<Enemy>().Damage(dmg);
        }
    }
}