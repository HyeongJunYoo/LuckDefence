using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using cakeslice;

public class GameManager : MonoBehaviour
{
    public GameObject map;
    public GameObject land;
    public GameObject enemyCount;
    public GameObject myUnitCount;

    public GameObject menuPanel;
    public GameObject endPanel;
    public GameObject buttons;

    public GameObject[] units;
    public GameObject[] enemies;

    public LayerMask enemyLayer;
    public LayerMask landLayer;
    public LayerMask unitsLayer;

    public Text moneyImage;
    public Text countImage;
    public Text timeImage;
    public Text stageImage;
    public Text aTypeUpgradeImage;
    public Text bTypeUpgradeImage;
    public Text cTypeUpgradeImage;

    public Canvas enemyHpCanvas;
    public Slider hpBar;
    

    List<GameObject> aTypeList;
    List<GameObject> bTypeList;
    List<GameObject> cTypeList;

    public int mapSize;
    public int stage;

    public int aTypeUpgradeCost;
    public int bTypeUpgradeCost;
    public int cTypeUpgradeCost;

    public int aTypeUpgradeCount;
    public int bTypeUpgradeCount;
    public int cTypeUpgradeCount;
   
    public int money;

    public float countTime;

    public bool isPause;
    public bool isPlay;

    Vector3 spawnPos;
    Vector3 enemySpawnPos;

    public Component[] outLine;
    Transform beforeClick;
    Transform unit;

    void Awake()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                Instantiate(land, new Vector3((5 - i) * 1.2f, 0, (5 - j) * 1.2f), Quaternion.identity ).transform.SetParent(map.transform);
            }
        }
    }


    void Start()
    {
        aTypeList = new List<GameObject>();
        bTypeList = new List<GameObject>();
        cTypeList = new List<GameObject>();

        spawnPos = transform.position;
        enemySpawnPos = new Vector3(-9.5f, 0, 9.5f);

        beforeClick = null;
        unit = null;

        RandomUnitSpawn();
    }


    // Update is called once per frame
    void Update()
    {
        if (isPlay && !isPause)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            ClickObject(ray);
            LandEffect(ray);
        }
    }

    void ClickObject(Ray ray)
    {
        RaycastHit hit;
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out hit, float.MaxValue, unitsLayer))
            {
                if (unit != null)
                {
                    if (unit != hit.transform)
                    {
                        outLine = unit.GetComponentsInChildren<cakeslice.Outline>();
                        foreach (cakeslice.Outline outline in outLine)
                            outline.eraseRenderer = true;

                    }
                }
                outLine = hit.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in outLine)
                    outline.eraseRenderer = false;
                unit = hit.transform;
                Debug.Log("유닛클릭됨");
            }
            else if (Physics.Raycast(ray, out hit, float.MaxValue, landLayer))
            {
                if (unit != null)
                {
                    unit.position = hit.transform.position;
                    outLine = unit.GetComponentsInChildren<cakeslice.Outline>();
                    foreach (cakeslice.Outline outline in outLine)
                        outline.eraseRenderer = true;
                    unit = null;
                    Debug.Log("유닛 이동함");
                }
            }
            else
            {
                if (unit != null)
                {
                    outLine = unit.GetComponentsInChildren<cakeslice.Outline>();
                    foreach (cakeslice.Outline outline in outLine)
                        outline.eraseRenderer = true;
                    unit = null;
                }
            }

        }
    }

    void LandEffect(Ray ray)
    {
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, float.MaxValue, unitsLayer))
        {

        }
        else if (Physics.Raycast(ray, out hit, float.MaxValue, landLayer))
        {
            if (beforeClick != null)
            {
                if (beforeClick != hit.transform)
                {
                    beforeClick.GetComponent<MeshRenderer>().material.color = Color.black;
                    beforeClick.GetComponent<cakeslice.Outline>().eraseRenderer = true;
                }
            }
            hit.transform.GetComponent<MeshRenderer>().material.color = Color.white;
            hit.transform.GetComponent<cakeslice.Outline>().eraseRenderer = false;
            beforeClick = hit.transform;
        }
        else
        {
            if (beforeClick != null)
            {
                beforeClick.GetComponent<cakeslice.Outline>().eraseRenderer = true;
                beforeClick.GetComponent<MeshRenderer>().material.color = Color.black;
            }
        }

    }

    void CheckOverlap(int type, int curCount, int maxCount, int sign)
    {
        if (type == 1)
        {
            int posX = 0, posZ = 0;
            while (Physics.Raycast(spawnPos + Vector3.down, transform.up, 5f, unitsLayer))
            {

                if (curCount < maxCount / 2)
                {
                    curCount++;
                    posX += sign;
                }
                else if (curCount == maxCount)
                {
                    maxCount += 2;
                    sign *= -1;
                    curCount = 0;
                }
                else if (curCount >= maxCount / 2)
                {
                    curCount++;
                    posZ += sign;
                }

                spawnPos = new Vector3(posX * 1.2f, 0, posZ * 1.2f);
            }
        }
        else if(type == 0)
        {
            float posX = -9.5f, posZ = 9.5f;
            while (Physics.Raycast(enemySpawnPos + Vector3.down, Vector3.up, 5f, enemyLayer))
            {

                if (curCount < maxCount / 2)
                {
                    curCount++;
                    posX += sign;
                    Debug.Log("X값 추가");
                }
                else if (curCount == maxCount)
                {
                    Debug.Log("sign 바꾸기");
                    maxCount += 2;
                    sign *= -1;
                    curCount = 0;
                }
                else if (curCount >= maxCount / 2)
                {
                    curCount++;
                    posZ += sign;
                    Debug.Log("Z값 추가");
                }

                enemySpawnPos = new Vector3(posX, 0, posZ);
            }
        }
    }


    IEnumerator Spawn()
    {
        for (int i = 0; i < 60; i++)
        {
            enemySpawnPos = new Vector3(-9.5f, 0, 9.5f);
            if (Physics.Raycast(enemySpawnPos + Vector3.down, Vector3.up, 5f, enemyLayer))
            {
                CheckOverlap(0, 0, 2, 1);
            }
            GameObject enemyObj = Instantiate(enemies[stage - 1], enemySpawnPos, enemies[stage - 1].transform.rotation);
            enemyObj.transform.SetParent(enemyCount.transform);

            Slider hpSlider = Instantiate(hpBar, Camera.main.WorldToScreenPoint(enemySpawnPos), Quaternion.identity);
            hpSlider.transform.SetParent(enemyHpCanvas.transform);

            hpSlider.gameObject.SetActive(false);
            enemyObj.GetComponent<Enemy>().hpBarSlider = hpSlider;
            if(enemyCount.transform.childCount >= 100)
            {
                GameEnd();
                break;
            }
            yield return new WaitForSeconds(0.35f);
        }
    }


    public void RandomUnitButton()
    {
        if (myUnitCount.transform.childCount > 120)
            return;

        spawnPos = transform.position;

        if (Physics.Raycast(spawnPos + Vector3.down, transform.up, 5f, unitsLayer))
        {
            CheckOverlap(1, 0, 2, 1);
        }


        if (money >= 20)
        {
            RandomUnitSpawn();
            money -= 20;
        }
    }

    public void RandomUnitSpawn()
    {
        int randomNum = Random.Range(0, 3);
        switch (randomNum)
        {
            case 0:
                if (Random.Range(0, 10) < 2)
                {
                    GameObject aType = Instantiate(units[randomNum + 3], spawnPos, units[3].transform.rotation);
                    aType.GetComponent<Units>().dmg = (aTypeUpgradeCount + 1) * 2;
                    aType.transform.SetParent(myUnitCount.transform);
                    aTypeList.Add(aType);
                    
                }
                else
                {
                    GameObject aType = Instantiate(units[randomNum], spawnPos, units[randomNum].transform.rotation);
                    aType.GetComponent<Units>().dmg = (aTypeUpgradeCount + 1) * 2;
                    aType.transform.SetParent(myUnitCount.transform);
                    aTypeList.Add(aType);
                }
                break;
            case 1:
                if (Random.Range(0, 10) < 2)
                {
                    GameObject bType = Instantiate(units[randomNum + 3], spawnPos, units[4].transform.rotation);
                    bType.GetComponent<Units>().dmg = (bTypeUpgradeCount + 1) * 3;
                    bType.transform.SetParent(myUnitCount.transform);
                    bTypeList.Add(bType);
                }
                else
                {
                    GameObject bType = Instantiate(units[randomNum], spawnPos, units[randomNum].transform.rotation);
                    bType.GetComponent<Units>().dmg = (bTypeUpgradeCount + 1) * 3;
                    bType.transform.SetParent(myUnitCount.transform);
                    bTypeList.Add(bType);
                }
                break;
            case 2:
                if (Random.Range(0, 10) < 2)
                {
                    GameObject cType = Instantiate(units[randomNum + 3], spawnPos, units[5].transform.rotation);
                    cType.GetComponent<Units>().dmg = (cTypeUpgradeCount + 1) * 5;
                    cType.transform.SetParent(myUnitCount.transform);
                    cTypeList.Add(cType);
                }
                else
                {
                    GameObject cType = Instantiate(units[randomNum], spawnPos, units[randomNum].transform.rotation);
                    cType.GetComponent<Units>().dmg = (cTypeUpgradeCount + 1) * 5;
                    cType.transform.SetParent(myUnitCount.transform);
                    cTypeList.Add(cType);
                }
                break;          
        }
     }

    public void Upgrade1()
    {
        if(money >= aTypeUpgradeCost && aTypeUpgradeCount < 17)
        {
            money -= aTypeUpgradeCost;
            foreach (GameObject count in aTypeList)
                count.GetComponent<Units>().dmg += 2;
            aTypeUpgradeCost += 4;
            aTypeUpgradeCount++;
            if (aTypeUpgradeCount != 17)
                aTypeUpgradeImage.text = "$ " + aTypeUpgradeCost;
            else
                aTypeUpgradeImage.text = "MAX";
        }
    }
    public void Upgrade2()
    {
        if (money >= bTypeUpgradeCost && bTypeUpgradeCount < 11)
        {
            money -= bTypeUpgradeCost;
            foreach (GameObject count in bTypeList)
                count.GetComponent<Units>().dmg += 3;
            bTypeUpgradeCost += 9;
            bTypeUpgradeCount++;
            if (bTypeUpgradeCount != 11)
                bTypeUpgradeImage.text = "$ " + bTypeUpgradeCost;
            else
                bTypeUpgradeImage.text = "MAX";
        }
    }
    public void Upgrade3()
    {
        if (money >= cTypeUpgradeCost && cTypeUpgradeCount < 9)
        {
            money -= cTypeUpgradeCost;
            foreach (GameObject count in cTypeList)
                count.GetComponent<Units>().dmg += 5;
            cTypeUpgradeCost += 10;
            cTypeUpgradeCount++;
            if (cTypeUpgradeCount != 9)
                cTypeUpgradeImage.text = "$ " + cTypeUpgradeCost;
            else
                cTypeUpgradeImage.text = "MAX";
        }
    }

    public void MouseEnterUpgrade1()
    {
        foreach (GameObject count in aTypeList)
        {
            if (unit != count.transform)
            {
                Component[] obj = count.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in obj)
                {
                    outline.color = 2;
                    outline.eraseRenderer = false;
                }
            }
        }
    }
    public void MouseExitUpgrade1()
    {

        foreach (GameObject count in aTypeList)
        {
            if (unit != count.transform)
            {
                Component[] obj = count.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in obj)
                {
                    outline.color = 1;
                    outline.eraseRenderer = true;
                }
            }
        }
    }

    public void MouseEnterUpgrade2()
    {
        foreach (GameObject count in bTypeList)
        {
            if (unit != count.transform)
            {
                Component[] obj = count.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in obj)
                {
                    outline.color = 2;
                    outline.eraseRenderer = false;
                }
            }
        }
    }
    public void MouseExitUpgrade2()
    {

        foreach (GameObject count in bTypeList)
        {
            if (unit != count.transform)
            {
                Component[] obj = count.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in obj)
                {
                    outline.color = 1;
                    outline.eraseRenderer = true;
                }
            }
        }
    }

    public void MouseEnterUpgrade3()
    {
        foreach (GameObject count in cTypeList)
        {
            if (unit != count.transform)
            {
                Component[] obj = count.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in obj)
                {
                    outline.color = 2;
                    outline.eraseRenderer = false;
                }
            }
        }
    }
    public void MouseExitUpgrade3()
    {

        foreach (GameObject count in cTypeList)
        {
            if (unit != count.transform)
            {
                Component[] obj = count.transform.GetComponentsInChildren<cakeslice.Outline>();
                foreach (cakeslice.Outline outline in obj)
                {
                    outline.color = 1;
                    outline.eraseRenderer = true;
                }
            }
        }
    }

    void UiChange()
    {
        if (Mathf.Floor(countTime) > 0)
        {
            countTime -= Time.deltaTime;
            if(countTime < 10)
                timeImage.text = "0:0" + Mathf.Floor(countTime);
            else
                timeImage.text = "0:" + Mathf.Floor(countTime);
        }
        else if(isPlay)
        {
            countTime = 42;
            SpawnStart();
        }

        if (money < 10)
            moneyImage.text = "$   " + money;
        else
            moneyImage.text = "$ " + money;

        countImage.text = "Count " + enemyCount.transform.childCount;

        stageImage.text = "Stage " + stage;

    }

    void SpawnStart()
    {   
        stage++;
        StartCoroutine(Spawn());
    }

    public void ClickMenu()
    {
        if (isPause == false)
        {
            menuPanel.SetActive(true);
            buttons.SetActive(false);
            isPause = true;
        }
        else
        {
            menuPanel.SetActive(false);
            buttons.SetActive(true);
            isPause = false;
        }
    }

    public void ClickRestart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void ClickExit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void GameEnd()
    {
        endPanel.SetActive(true);
        menuPanel.SetActive(false);
        buttons.SetActive(false);
        isPlay = false;
        Destroy(myUnitCount);
    }

    void LateUpdate()
    {
        UiChange();

        if (Input.GetKeyDown(KeyCode.Escape) && isPlay)
        {
            ClickMenu();
        }
    }
}
