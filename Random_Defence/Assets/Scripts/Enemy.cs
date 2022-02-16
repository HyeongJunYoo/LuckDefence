using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public int point;
    public int armor;

    public float maxShiled;
    public float shield;
    public float maxHp;
    public float hp;
    public float speed;

    public Slider hpBarSlider;
    public ParticleSystem particle;
    public GameObject[] points;

    SkinnedMeshRenderer[] meshRenderers;
    Coroutine shiledCoroutine;
    NavMeshAgent navMesh;


    void Start()
    {
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        navMesh = GetComponent<NavMeshAgent>();
        points = GameObject.FindGameObjectsWithTag("Points");
        navMesh.speed = speed;
        maxShiled = shield;
        maxHp = hp;
    }


    void Update()
    {
        Move();
        ShieldRegen();
        Die();
    }


    void Move()
    {
        if (Vector3.Distance(transform.position, points[0].transform.position) <= 5 && point != 1)
        {
            point = 1;
        }
        else if (Vector3.Distance(transform.position, points[1].transform.position) <= 5 && point != 2)
        {
            point = 2;
        }
        else if (Vector3.Distance(transform.position, points[2].transform.position) <= 5 && point != 3)
        {
            point = 3;
        }
        else if (Vector3.Distance(transform.position, points[3].transform.position) <= 5 && point != 0)
        {
            point = 0;
        }
        else
        {
            navMesh.SetDestination(points[point].transform.position);
            //transform.LookAt(points[point].transform);
            //transform.position = Vector3.MoveTowards(transform.position, points[point].transform.position, startSpeed * Time.deltaTime);
        }

        hpBarSlider.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 1));
        
    }

    public void Damage(int dmg)
    {
        if (hpBarSlider.IsActive() == false)
            hpBarSlider.gameObject.SetActive(true);

        if (hp > 0 && shield > 0)
        {
            shield -= 0.5f;
            //if(!particle.isPlaying)
                particle.Play();
            StartCoroutine(Hit(Color.blue));
        }    
        else if (hp > 0 && shield <= 0)
        {    
            hp -= (dmg - armor) > 0 ? dmg - armor : 0.5f;
            hpBarSlider.value = hp / maxHp;
            StartCoroutine(Hit(Color.red));
        }
    }

    void Die()
    {
        if(hp <= 0)
        {
            Destroy(hpBarSlider.gameObject);
            Destroy(gameObject);
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>().money++;
        }
    }

    void ShieldRegen()
    {
        if (shield < maxShiled && shiledCoroutine == null)
            shiledCoroutine = StartCoroutine(Shield());
    }

    IEnumerator Hit(Color color)
    {
        foreach (SkinnedMeshRenderer mesh in meshRenderers)
            mesh.material.color = color;

        if (hp > 0)
        {

            yield return new WaitForSeconds(0.15f);

            foreach (SkinnedMeshRenderer mesh in meshRenderers)
                mesh.material.color = Color.white;
        }             
    }

    IEnumerator Shield()
    {
        yield return new WaitForSeconds(1.5f);

        if (shield < maxShiled)
        {
            if (shield == maxShiled - 0.5f)
                shield += 0.5f;
            else
                shield++;
            StartCoroutine(Shield());
        }
        else
            shiledCoroutine = null;
    }


}

