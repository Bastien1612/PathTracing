using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathScript : MonoBehaviour
{
    private Vector2Int Start = new Vector2Int(2, 2);
    private Vector2Int End = new Vector2Int(4, 4);
    public int Iteration = 100;
    [SerializeField]
    private Transform Spheres = null;
    [SerializeField]
    private Transform SpheresPath = null;
    public bool BestV = false;
    public bool SemiBestV = false;

    public bool BestPick = false;
    public bool TwoBestPick = false;
    public bool RemoveWorstPick = false;

    private int ActualBestScore = -600;
    private List<Vector2Int> ActualBestPath = new List<Vector2Int>();

    public void InitGame()
    {
        DeleteAll();

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
        Start.x = RndX;
        Start.y = RndY;
        // Set End
        RndX = Random.Range(2, Size - 2);
        RndY = Random.Range(2, Size - 2);
        while (this.GetComponent<GridState>().GetStateValue(RndX, RndY) < 2 || (RndX == Start.x && RndY == Start.y))
        {
            RndX = Random.Range(2, Size - 2);
            RndY = Random.Range(2, Size - 2);
        }
        End.x = RndX;
        End.y = RndY;
        // Create Points
        GameObject GoStart = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GoStart.GetComponent<MeshRenderer>().material = Resources.Load("Start", typeof(Material)) as Material;
        float PosX;
        float PosY;
        if (Start.x % 2 == 0)
        {
            PosX = ((Start.x % Size) * 7.7f);
            PosY = (Start.y * 8.8f);
        }
        else
        {
            PosX = ((Start.x % Size) * 7.7f);
            PosY = (Start.y * 8.8f) + 4.4f;
        }
        GoStart.transform.position = new Vector3(PosX, PosY, 10 * this.GetComponent<GridState>().GetScaleZ(Start));
        GoStart.transform.localScale = new Vector3(5, 5, 5);
        GoStart.transform.SetParent(Spheres);
        GameObject GoEnd = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GoEnd.GetComponent<MeshRenderer>().material = Resources.Load("End", typeof(Material)) as Material;
        if (End.x % 2 == 0)
        {
            PosX = ((End.x % Size) * 7.7f);
            PosY = (End.y * 8.8f);
        }
        else
        {
            PosX = ((End.x % Size) * 7.7f);
            PosY = (End.y * 8.8f) + 4.4f;
        }
        GoEnd.transform.position = new Vector3(PosX, PosY, 10 * this.GetComponent<GridState>().GetScaleZ(End));
        GoEnd.transform.localScale = new Vector3(5, 5, 5);
        GoEnd.transform.SetParent(Spheres);
    }
    public void LaunchGame()
    {
        // Creations chemins
        DeletePaths();
        List<List<Vector2Int>> AllPath = new List<List<Vector2Int>>();
        List<int> AllCost = new List<int>();
        int coutminend = 99999;
        int MaxParcours = this.GetComponent<GridState>().GetSize() * this.GetComponent<GridState>().GetSize();
        List<Vector2Int> Path = new List<Vector2Int>();
        int Cost = 0;
        bool Success = false;

        for (int cpt = 0; cpt < Iteration; cpt++)
        {
            Path = new List<Vector2Int>();
            Cost = 0;
            Success = false;

            Vector2Int ActualPos = VerifyVoisin(Start, Success);
            if (BestV) ActualPos = VerifyBestVoisin(Start, End, ref Success);
            else if (SemiBestV) ActualPos = VerifySemiBestVoisin(Start, End, ref Success);
            Cost += GetCout(ActualPos);
            for (int cptb = 0; cptb < MaxParcours; cptb++)
            {
                if (Success) break;
                Path.Add(ActualPos);
                if (BestV) ActualPos = VerifyBestVoisin(ActualPos, End, ref Success);
                else if (SemiBestV) ActualPos = VerifySemiBestVoisin(ActualPos, End, ref Success);
                else ActualPos = VerifyVoisin(ActualPos, Success);
                Cost += GetCout(ActualPos);
                if (coutminend <= Cost) break;
            }
            if (Success)
            {
                AllPath.Add(Path);
                AllCost.Add(Cost);
                if (Cost < coutminend) coutminend = Cost;
            }
        }
        if (AllCost.Count < 1)
        {            
            if(ActualBestScore < 0)
            {
                Debug.Log("No Path Find !");
            }
            else
            {
                // Affichage
                foreach (Vector2Int Vec in ActualBestPath)
                {
                    InitPathSphere(Vec);
                }
            }
        }
        else
        {
            // Recherche cout le plus bas
            int indexmax = 0;
            for (int cpt = 0; cpt < AllCost.Count; cpt++)
            {
                if (AllCost[cpt] < AllCost[indexmax]) indexmax = cpt;
            }

            if (ActualBestScore > AllCost[indexmax] || ActualBestScore < 0)
            {
                ActualBestScore = AllCost[indexmax];
                ActualBestPath = AllPath[indexmax];
                // Affichage
                Debug.Log("Cout : " + AllCost[indexmax]);
                foreach (Vector2Int Vec in AllPath[indexmax])
                {
                    InitPathSphere(Vec);
                }
            }
            else
            {
                // Affichage
                foreach (Vector2Int Vec in ActualBestPath)
                {
                    InitPathSphere(Vec);
                }
            }
        }
    }
    Vector2Int VerifyVoisin(Vector2Int V2, bool suc)
    {
        List<Vector2Int> Voisin = GetVoisins(V2);
        foreach (Vector2Int V2a in Voisin)
        {
            if (V2a.x == End.x && V2a.y == End.y)
            {
                suc = true;
                return V2a;
            }
        }
        if (BestPick) PickBestVoisin(ref Voisin);
        if (RemoveWorstPick) DeleteWorstVoisin(ref Voisin);
        if (TwoBestPick) TwoPickBestVoisin(ref Voisin);
        Vector2Int V2b = Voisin[Random.Range(0, Voisin.Count)];
        while (GetCout(V2) == 99999) V2b = Voisin[Random.Range(0, Voisin.Count)];
        return V2b;
    }
    Vector2Int VerifyBestVoisin(Vector2Int V2, Vector2Int V2e, ref bool suc)
    {
        List<Vector2Int> Voisin = GetBestVoisins(V2, V2e);
        foreach (Vector2Int V2a in Voisin)
        {
            if (V2a.x == End.x && V2a.y == End.y)
            {
                suc = true;
                return V2a;
            }
        }
        if (BestPick) PickBestVoisin(ref Voisin);
        if (RemoveWorstPick) DeleteWorstVoisin(ref Voisin);
        if (TwoBestPick) TwoPickBestVoisin(ref Voisin);
        Vector2Int V2b = Voisin[Random.Range(0, Voisin.Count)];
        while (GetCout(V2) == 99999) V2b = Voisin[Random.Range(0, Voisin.Count)];
        return V2b;
    }
    Vector2Int VerifySemiBestVoisin(Vector2Int V2, Vector2Int V2e, ref bool suc)
    {
        List<Vector2Int> Voisin = GetSemiBestVoisins(V2, V2e);
        foreach (Vector2Int V2a in Voisin)
        {
            if (V2a.x == End.x && V2a.y == End.y)
            {
                suc = true;
                return V2a;
            }
        }
        if (BestPick) PickBestVoisin(ref Voisin);
        if (RemoveWorstPick) DeleteWorstVoisin(ref Voisin);
        if (TwoBestPick) TwoPickBestVoisin(ref Voisin);
        Vector2Int V2b = Voisin[Random.Range(0, Voisin.Count)];
        while (GetCout(V2) == 99999) V2b = Voisin[Random.Range(0, Voisin.Count)];
        return V2b;
    }
    List<Vector2Int> GetVoisins(Vector2Int V2)
    {
        List<Vector2Int> V2t = new List<Vector2Int>();
        if (V2.x % 2 == 0)
        {
            // Voisin Nord
            V2t.Add(new Vector2Int(V2.x, V2.y + 1));
            // Voisin Nord Est
            V2t.Add(new Vector2Int(V2.x - 1, V2.y));
            // Voisin Nord Ouest
            V2t.Add(new Vector2Int(V2.x + 1, V2.y));
            // Voisin Sud
            V2t.Add(new Vector2Int(V2.x, V2.y - 1));
            // Voisin Sud Est
            V2t.Add(new Vector2Int(V2.x - 1, V2.y - 1));
            // Voisin Sud Ouest
            V2t.Add(new Vector2Int(V2.x + 1, V2.y - 1));
        }
        else
        {
            // Voisin Nord
            V2t.Add(new Vector2Int(V2.x, V2.y + 1));
            // Voisin Nord Est
            V2t.Add(new Vector2Int(V2.x - 1, V2.y + 1));
            // Voisin Nord Ouest
            V2t.Add(new Vector2Int(V2.x + 1, V2.y + 1));
            // Voisin Sud
            V2t.Add(new Vector2Int(V2.x, V2.y - 1));
            // Voisin Sud Est
            V2t.Add(new Vector2Int(V2.x - 1, V2.y));
            // Voisin Sud Ouest
            V2t.Add(new Vector2Int(V2.x + 1, V2.y));
        }
        return V2t;
    }
    List<Vector2Int> GetBestVoisins(Vector2Int V2s, Vector2Int V2e)
    {
        List<Vector2Int> V2t = new List<Vector2Int>();
        if (V2s.x % 2 == 0)
        {
            if (V2e.y > V2s.y)
            {
                // Voisin Nord
                V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
            }
            else if (V2e.y < V2s.y)
            {
                // Voisin Sud
                V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
            }
            if (V2e.x > V2s.x)
            {
                // Voisin Nord Ouest
                V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                // Voisin Sud Ouest
                V2t.Add(new Vector2Int(V2s.x + 1, V2s.y - 1));
            }
            else if (V2e.x < V2s.x)
            {
                // Voisin Nord Est
                V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                // Voisin Sud Est
                V2t.Add(new Vector2Int(V2s.x - 1, V2s.y - 1));
            }
        }
        else
        {
            if (V2e.y > V2s.y)
            {
                // Voisin Nord
                V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
            }
            else if (V2e.y < V2s.y)
            {
                // Voisin Sud
                V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
            }
            if (V2e.x > V2s.x)
            {
                // Voisin Nord Ouest
                V2t.Add(new Vector2Int(V2s.x + 1, V2s.y + 1));
                // Voisin Sud Ouest
                V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
            }
            else if (V2e.x < V2s.x)
            {
                // Voisin Nord Est
                V2t.Add(new Vector2Int(V2s.x - 1, V2s.y + 1));
                // Voisin Sud Est
                V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
            }
        }
        return V2t;
    }
    List<Vector2Int> GetSemiBestVoisins(Vector2Int V2s, Vector2Int V2e)
    {
        List<Vector2Int> V2t = new List<Vector2Int>();
        if (V2s.x % 2 == 0)
        {
            if (V2e.y > V2s.y)
            {
                if (V2e.x > V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y - 1));
                }
                else if (V2e.x < V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y - 1));
                }
                else
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                }
            }
            else if (V2e.y < V2s.y)
            {
                if (V2e.x > V2s.x)
                {
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y - 1));
                }
                else if (V2e.x < V2s.x)
                {
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y - 1));
                }
                else
                {
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y - 1));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y - 1));
                }
            }
            else
            {
                if (V2e.x > V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y - 1));
                }
                else if (V2e.x < V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y - 1));
                }
            }
        }
        else
        {
            if (V2e.y > V2s.y)
            {
                if (V2e.x > V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y + 1));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                }
                else if (V2e.x < V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y + 1));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                }
                else
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y + 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y + 1));
                }
            }
            else if (V2e.y < V2s.y)
            {
                if (V2e.x > V2s.x)
                {
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y + 1));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                }
                else if (V2e.x < V2s.x)
                {
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y + 1));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                }
                else
                {
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                }
            }
            else
            {
                if (V2e.x > V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y + 1));
                    // Voisin Sud Ouest
                    V2t.Add(new Vector2Int(V2s.x + 1, V2s.y));
                }
                else if (V2e.x < V2s.x)
                {
                    // Voisin Nord
                    V2t.Add(new Vector2Int(V2s.x, V2s.y + 1));
                    // Voisin Sud
                    V2t.Add(new Vector2Int(V2s.x, V2s.y - 1));
                    // Voisin Nord Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y + 1));
                    // Voisin Sud Est
                    V2t.Add(new Vector2Int(V2s.x - 1, V2s.y));
                }
            }
        }
        return V2t;
    }
    void DeleteWorstVoisin(ref List<Vector2Int> LV2)
    {
        if (LV2.Count < 2) return;
        int worst = GetCout(LV2[0]);
        int worstid = 0;
        for (int cpt = 1; cpt < LV2.Count; cpt++)
        {
            if (worst < GetCout(LV2[cpt]))
            {
                worst = GetCout(LV2[cpt]);
                worstid = cpt;
            }
            else if (worst == GetCout(LV2[cpt]))
            {
                int rnd = Random.Range(0, 2);
                if (rnd == 0)
                {
                    worst = GetCout(LV2[cpt]);
                    worstid = cpt;
                }
            }
        }
        LV2.RemoveAt(worstid);
    }
    void PickBestVoisin(ref List<Vector2Int> LV2)
    {
        if (LV2.Count < 2) return;
        int best = GetCout(LV2[0]);
        Vector2Int bestid = LV2[0];
        for (int cpt = 1; cpt < LV2.Count; cpt++)
        {
            if (best > GetCout(LV2[cpt]))
            {
                best = GetCout(LV2[cpt]);
                bestid = LV2[cpt];
            }
            else if (best == GetCout(LV2[cpt]))
            {
                int rnd = Random.Range(0, 2);
                if (rnd == 0)
                {
                    best = GetCout(LV2[cpt]);
                    bestid = LV2[cpt];
                }
            }
        }
        LV2.Clear();
        LV2.Add(bestid);
    }
    void TwoPickBestVoisin(ref List<Vector2Int> LV2)
    {
        if (LV2.Count < 3) return;
        int best = GetCout(LV2[0]);
        Vector2Int bestid1 = LV2[0];
        for (int cpt = 1; cpt < LV2.Count; cpt++)
        {
            if (best > GetCout(LV2[cpt]))
            {
                best = GetCout(LV2[cpt]);
                bestid1 = LV2[cpt];
            }
        }
        LV2.Remove(bestid1);
        best = GetCout(LV2[0]);
        Vector2Int bestid2 = LV2[0];
        for (int cpt = 1; cpt < LV2.Count; cpt++)
        {
            if (best > GetCout(LV2[cpt]))
            {
                best = GetCout(LV2[cpt]);
                bestid2 = LV2[cpt];
            }
            else if (best == GetCout(LV2[cpt]))
            {
                int rnd = Random.Range(0, 2);
                if (rnd == 0)
                {
                    best = GetCout(LV2[cpt]);
                    bestid2 = LV2[cpt];
                }
            }
        }
        LV2.Clear();
        LV2.Add(bestid1);
        LV2.Add(bestid2);
    }
    int GetCout(Vector2Int V2)
    {
        // Cout
        // Ocean = 99999
        // Lac = 6
        // Montagne = 6
        // Plateau = 3
        // Foret = 2
        // Plaine = 1 
        switch (this.GetComponent<GridState>().GetStateValue(V2.x, V2.y))
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
    void InitPathSphere(Vector2Int V2)
    {
        int Size = this.GetComponent<GridState>().GetSize();
        // Create Points
        GameObject GoPath = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        GoPath.GetComponent<MeshRenderer>().material = Resources.Load("Path", typeof(Material)) as Material;
        float PosX;
        float PosY;
        if (V2.x % 2 == 0)
        {
            PosX = ((V2.x % Size) * 7.7f);
            PosY = (V2.y * 8.8f);
        }
        else
        {
            PosX = ((V2.x % Size) * 7.7f);
            PosY = (V2.y * 8.8f) + 4.4f;
        }
        GoPath.transform.position = new Vector3(PosX, PosY, 10 * this.GetComponent<GridState>().GetScaleZ(V2));
        GoPath.transform.localScale = new Vector3(5, 5, 5);
        GoPath.transform.SetParent(SpheresPath);
    }
    public void DeleteSpheres()
    {
        if (Spheres.childCount > 0) foreach (Transform child in Spheres) GameObject.Destroy(child.gameObject);
        ActualBestScore = -600;
        ActualBestPath = new List<Vector2Int>();
    }
    public void DeletePaths()
    {
        if (SpheresPath.childCount > 0) foreach (Transform child in SpheresPath) GameObject.Destroy(child.gameObject);
    }
    public void DeleteAll()
    {
        DeleteSpheres();
        DeletePaths();
    }
}
