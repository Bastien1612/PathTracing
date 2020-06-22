using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour
{
    private int XStart = 2, Ystart = 2;
    private int XEnd = 4, YEnd = 4;
    public void InitGame()
    {
        int Size = this.GetComponent<GridState>().GetSize();
        if (Size < 10)
        {
            Debug.Log("Incorrect Size");
            return;
        }
        // Set Start
        int RndX = Random.Range(2, Size - 2);
        int RndY = Random.Range(2, Size - 2);
        while (this.GetComponent<GridState>().GetStateValue(RndX, RndY) != 2)
        {
            RndX = Random.Range(2, Size - 2);
            RndY = Random.Range(2, Size - 2);
        }
        XStart = RndX;
        Ystart = RndY;
        // Set End
        RndX = Random.Range(2, Size - 2);
        RndY = Random.Range(2, Size - 2);
        while (this.GetComponent<GridState>().GetStateValue(RndX, RndY) < 2 || (RndX == XStart && RndY == Ystart))
        {
            RndX = Random.Range(2, Size - 2);
            RndY = Random.Range(2, Size - 2);
        }
        XEnd = RndX;
        YEnd = RndY;
        // Create Points
        GameObject GoStart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GoStart.GetComponent<MeshRenderer>().material = Resources.Load("Start", typeof(Material)) as Material;
        float PosX;
        float PosY;
        if (XStart % 2 == 0)
        {
            PosX = ((XStart % Size) * 7.7f);
            PosY = (Ystart * 8.8f);
        }
        else
        {
            PosX = ((XStart % Size) * 7.7f);
            PosY = (Ystart * 8.8f) + 4.4f;
        }        
        GoStart.transform.position = new Vector3(PosX, PosY, 10 * this.GetComponent<GridState>().GetScaleZ(XStart,Ystart));
        GoStart.transform.localScale = new Vector3(5, 5, 5);
        this.GetComponent<GridState>().SetFieldParent(GoStart);
        GameObject GoEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GoEnd.GetComponent<MeshRenderer>().material = Resources.Load("End", typeof(Material)) as Material;
        if (XEnd % 2 == 0)
        {
            PosX = ((XEnd % Size) * 7.7f);
            PosY = (YEnd * 8.8f);
        }
        else
        {
            PosX = ((XEnd % Size) * 7.7f);
            PosY = (YEnd * 8.8f) + 4.4f;
        }
        GoEnd.transform.position = new Vector3(PosX, PosY, 10 * this.GetComponent<GridState>().GetScaleZ(XEnd, YEnd));
        GoEnd.transform.localScale = new Vector3(5, 5, 5);
        this.GetComponent<GridState>().SetFieldParent(GoEnd);
    }
    public void LaunchGame()
    {        

    }

    int GetCout(int x, int y)
    {
        // Cout
        // Ocean = 99999
        // Lac = 6
        // Montagne = 6
        // Plateau = 3
        // Foret = 2
        // Plaine = 1 
        switch (this.GetComponent<GridState>().GetStateValue(x, y))
        {
            case 0:
                return 99999;
            case 1:
                return 6;
            case 2:
                return 1;
            case 3:
                return 2;
            case 4:
                return 3;
            case 5:
                return 6;
        }
        return 100;
    }
}
