using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridState : MonoBehaviour
{
    [SerializeField]
    private int Size = 24;
    [SerializeField]
    private Transform Field = null;
    [SerializeField]
    private GameObject PrefabHexagon = null;
    private int[,] State = new int[1, 1];


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) TotalInit();
    }
    void TotalInit()
    {
        InitState();
        InitField();
    }
    void InitState()
    {
        DeleteState();
        // 0 = Océan
        // 1 = Lac
        // 2 = Plaine
        // 3 = Foret
        // 4 = Plateau
        // 5 = Montagne
        for (int cptx = 0; cptx < Size; cptx++)
        {
            for (int cpty = 0; cpty < Size; cpty++)
            {
                if (cptx == 0 || cpty == 0 || cptx == Size - 1 || cpty == Size - 1) State[cptx, cpty] = 0;
                if (cptx == 1 || cpty == 1 || cptx == Size - 2 || cpty == Size - 2) State[cptx, cpty] = 1;
                else State[cptx, cpty] = Random.Range(1, 6);
            }
        }
    }
    void DeleteState()
    {
        State = new int[Size, Size];
    }
    void InitField()
    {
        DeleteField();
        // 0 = Océan
        // 1 = Plaine
        // 2 = Foret
        // 3 = Plateau
        // 4 = Montagne
        // 5 = Lac
        Material Forest = Resources.Load("/Hexagon/Forest", typeof(Material)) as Material;
        Material Plain = Resources.Load("/Hexagon/Plain", typeof(Material)) as Material;
        Material Mountain = Resources.Load("/Hexagon/Mountain", typeof(Material)) as Material;
        Material Plateau = Resources.Load("/Hexagon/Plateau", typeof(Material)) as Material;
        Material Ocean = Resources.Load("/Hexagon/Ocean", typeof(Material)) as Material;
        Material Lac = Resources.Load("/Hexagon/Lac", typeof(Material)) as Material;
        for (int cptx = 0; cptx < Size; cptx++)
        {
            for (int cpty = 0; cpty < Size; cpty++)
            {
                GameObject Go = Instantiate(PrefabHexagon);
                float PosX;
                float PosY;
                if (cptx % 2 == 0)
                {
                    PosX = 0;
                    PosY = 0;
                }
                else
                {
                    PosX = 0;
                    PosY = 0;
                }
                Go.transform.position = new Vector3(PosX, PosY, 0);
                switch (State[cptx, cpty])
                {
                    case 0:
                        Go.GetComponent<Renderer>().material = Ocean;
                        break;
                    case 1:
                        Go.GetComponent<Renderer>().material = Plain;
                        break;
                    case 2:
                        Go.GetComponent<Renderer>().material = Forest;
                        break;
                    case 3:
                        Go.GetComponent<Renderer>().material = Plateau;
                        Go.transform.localScale = new Vector3(Go.transform.localScale.x, Go.transform.localScale.y, Go.transform.localScale.z * 1.3f);
                        break;
                    case 4:
                        Go.GetComponent<Renderer>().material = Mountain;
                        Go.transform.localScale = new Vector3(Go.transform.localScale.x, Go.transform.localScale.y, Go.transform.localScale.z * 1.6f);
                        break;
                    case 5:
                        Go.GetComponent<Renderer>().material = Lac;
                        break;
                }
            }
        }
    }
    void DeleteField()
    {
        if (Field.childCount != 0) foreach (Transform child in Field) GameObject.Destroy(child.gameObject);
    }
    void InitCam()
    {

    }
}
