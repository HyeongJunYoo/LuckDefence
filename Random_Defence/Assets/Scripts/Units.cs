using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Units : MonoBehaviour
{
    public enum Type { AType, BType, CType, ATypeSpecial, BTypeSpecial, CTypeSpecial };
    public Type unitType;

    public GameObject head;
    public GameObject aTypeBullet;
    public GameObject bTypeBullet;
    public GameObject cTypeBullet;
    
    GameObject target;
    List<GameObject> targetList;
    AudioSource audioSource;

    public int dmg;
    public bool isDelay;
    float rotateSpeed = 1200f;
    public AudioClip aTypeAudio;
    public AudioClip bTypeAudio;
    public AudioClip cTypeAudio;



    void Start()
    {
        targetList = new List<GameObject>();
        audioSource = GetComponent<AudioSource>();
        isDelay = false;
        target = null;
    }


    void Update()
    {
        AttackStart();
        Rotate();
    }

    void AttackStart()
    {
        if (!isDelay && targetList.Count > 0)
        {
            isDelay = true;
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        if (target != null)
        {
            switch (unitType)
            {
                case Type.AType:
                    MakeProjectiles(aTypeBullet, aTypeAudio);
                    yield return new WaitForSeconds(0.5f);
                    isDelay = false;
                    break;
                case Type.BType:
                    MakeProjectiles(bTypeBullet, bTypeAudio);
                    yield return new WaitForSeconds(0.5f);
                    isDelay = false;
                    break;
                case Type.CType:
                    MakeProjectiles(cTypeBullet, cTypeAudio);
                    yield return new WaitForSeconds(1.0f);
                    isDelay = false;
                    break;
                case Type.ATypeSpecial:
                    MakeProjectiles(aTypeBullet, aTypeAudio);
                    yield return new WaitForSeconds(0.25f);
                    isDelay = false;
                    break;
                case Type.BTypeSpecial:
                    MakeProjectiles(bTypeBullet, bTypeAudio);
                    yield return new WaitForSeconds(0.25f);
                    isDelay = false;
                    break;
                case Type.CTypeSpecial:
                    MakeProjectiles(cTypeBullet, cTypeAudio);
                    yield return new WaitForSeconds(0.5f);
                    isDelay = false;
                    break;
            }
        }
        else
        {
            isDelay = false;
        }
    }

    void MakeProjectiles(GameObject bulletType, AudioClip audioType)
    {
        GameObject projectile = Instantiate(bulletType, transform.position, transform.rotation);
        projectile.GetComponent<Bullet>().target = target;
        projectile.GetComponent<Bullet>().dmg = dmg;
        audioSource.clip = audioType;
        audioSource.Play();
    }

    void Rotate()
    {
        if (target != null)
        {
            Vector3 dir = target.transform.position - head.transform.position;
            Quaternion quaternion = Quaternion.LookRotation(-dir);
            Vector3 rotation = quaternion.eulerAngles;
            Quaternion rotate = Quaternion.RotateTowards(head.transform.rotation, Quaternion.Euler(-90, rotation.y, 0), rotateSpeed * Time.deltaTime);
            head.transform.rotation = rotate;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            targetList.Add(other.gameObject);
            if(target == null)
                target = targetList[targetList.Count - 1];
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            foreach (GameObject count in targetList)
            {
                if (count == other.gameObject)
                {
                    targetList.Remove(count);
                    break;
                }
            }
            if (other.gameObject == target)
            {
                if (targetList.Count > 0)
                {
                    target = targetList[targetList.Count - 1];
                }
                else
                    target = null;

            }
        }
    }
}
