using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridState : MonoBehaviour
{
    [SerializeField]
    private int Size = 20;
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


    }
    void DeleteField()
    {
        if (Field.childCount != 0) foreach (Transform child in Field) GameObject.Destroy(child.gameObject);
    }
    void InitCam()
    {

    }
}
