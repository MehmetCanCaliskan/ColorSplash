
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class GridManager : MonoBehaviour
{
    public List<Sprite> TileSprites = new List<Sprite>();
    public List<string> TagNames = new List<string>();
    public List<GameObject> TilePrefabs = new List<GameObject>();
    public List<Sprite> BoostSprites = new List<Sprite>();

   

    public static int GridDimension = 7;
    
    public static int clickedX;
    public static int clickedY;
    public static int _whichBoostAround;
    
    public float Distance = 1.0f;
    public static GameObject[,] Grid;

    public static bool isInAction;

    public List<GameObject> directNeighbors;
    public List<GameObject> destroyableObjs = new List<GameObject>();
    public List<GameObject> colorTileDesObjs = new List<GameObject>();


    public List<string> targetTags = new List<string>();

    public delegate void DestroyedColorsDelegate();
    public static event DestroyedColorsDelegate destroyDelegate;

    public int countForSmoothLerpForCreated = 0;
    public int targetforsmoothlerpForcreated = 0;

    public int countForSmoothLerp = 0;
    public int targetForSmoothLerp = 0;

    public static GridManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        Grid = new GameObject[GridDimension, GridDimension];

        GridShaper.Instance.InitGrid();
        StartCoroutine(SpriteController.Instance.CheckTileSprites());
        isInAction = false;

        targetTags.Add("blue");
        targetTags.Add("green");
        targetTags.Add("red");
        targetTags.Add("yellow");
    }


    void Update()
    {
        
    }

    


    

    // verilen column row daki nesneyi alır.(Tile ı )
    public GameObject GetObjectAt(int column, int row)
    {
        if (column < 0 || column >= GridDimension
             || row < 0 || row >= GridDimension)
            return null;
        GameObject tile = Grid[column, row];        
        return tile;
    }


    // bir tile elemanını alıp etrafındaki bütün eşleşen elemanları tespit edip directNeighbors listesine ekliyor.
    public List<GameObject> DirectNeighbors(int col, int row)
    {
  
        GameObject _left = GetObjectAt(col - 1, row);
        if (_left != null && _left.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = directNeighbors.Contains(_left);
            if (!alreadyExist)
            {
                directNeighbors.Add(_left);
            }            
        }
        
        GameObject _right = GetObjectAt(col + 1, row);
        if (_right != null && _right.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = directNeighbors.Contains(_right);
            if (!alreadyExist)
            {
                directNeighbors.Add(_right);
            }
        }
        
        GameObject _up = GetObjectAt(col , row + 1);
        if (_up != null && _up.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = directNeighbors.Contains(_up);
            if (!alreadyExist)
            {
                directNeighbors.Add(_up);
            }
        }
        
        GameObject _down = GetObjectAt(col, row - 1);
        if (_down != null && _down.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = directNeighbors.Contains(_down);
            if (!alreadyExist)
            {
                directNeighbors.Add(_down);
            }
        }        
        return directNeighbors;
    }

    // Tile.cs scriptindeki kontrol fonk. na yazdırılacak fonk bu. Yukarıdaki komşularını bul fonk unu alıp for döngüsü içinde
    // i sayısı liste eleman sayısına eşitlenene kadar devam ediyor. Aynı zamanda fonk dan ötürü liste genişliyor. 
    public void FindAll(int col, int row) 
    {
        isInAction = true;

        int clickedCol  = col;
        int clickedRow = row;

        for (int i = 0; i < DirectNeighbors(col,row).Count; i++)
        {
            int _col = DirectNeighbors(col, row)[i].gameObject.GetComponent<Tile>().Position.x;
            int _row = DirectNeighbors(col, row)[i].gameObject.GetComponent<Tile>().Position.y;
            DirectNeighbors(_col,_row) ;            
        }
        int _count = directNeighbors.Count;
        string color = GetObjectAt(clickedCol, clickedRow).tag;
        colorTileDesObjs = directNeighbors;
        
        StartCoroutine(ExecuteProcess(clickedCol, clickedRow, _count, colorTileDesObjs, color));

        directNeighbors = new List<GameObject>();

    }

    // Renk tile larını patlatma ve yenisini getirme işlemlerini başlatır.
    IEnumerator ExecuteProcess(int _col, int _row, int _count, List<GameObject> _list, string _color)
    {
        
        int col = _col;
        int row = _row;
        int count = _count;
        string color = _color;
        List<GameObject> list = _list;


        
        yield return StartCoroutine(ClickedColorTile(col,row,count,list,color));
        //yield return new WaitForSecondsRealtime(0.6f);
        //yield return StartCoroutine(MoveObjects());
        //yield return new WaitForSecondsRealtime(0.6f);
        //yield return StartCoroutine(CreateObjects());
        //yield return StartCoroutine(MoveCreatedObjects());

        //yield return StartCoroutine(SpriteController.Instance.CheckTileSprites());

        isInAction = false;
    }

    // Boost tile larını patlatma ve yenisini getirme işlemlerini başlatır.(rotor , TNT gibi)
    public IEnumerator BoostClicked(int _col, int _row, int _primaryBoost)
    {
        
        isInAction = true;
        int col = _col;
        int row = _row;
        int primaryBoost = _primaryBoost;

        WhichBoostAround(col, row);
        

        switch (_primaryBoost)
        {
        case 0:
            yield return StartCoroutine(HeliDes(col, row));
            break;
        case 1:
            yield return StartCoroutine(RotDesHor(col,row));
            break;
        case 2:
            yield return StartCoroutine(RotDesVer(col, row));
            break;
        case 3:
            yield return StartCoroutine(TNT(col, row, destroyableObjs));
            break;
        case 4:
            yield return StartCoroutine(Colorbomb(col, row, "blue"));
            break;
        case 5:
            yield return StartCoroutine(Colorbomb(col, row,  "green"));
            break;
         case 6:
            yield return StartCoroutine(Colorbomb(col, row, "red"));
            break;
         case 7:
            yield return StartCoroutine(Colorbomb(col, row, "yellow"));
            break;

            default:
            print("Incorrect ");
            break;
        }
        yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
    }

    // +++++++++++++++++++++++++++++++++++++++++++  Exact Boost Scripts Start Here ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

    // Explodes itself and random same color one tile. 
    public IEnumerator HeliDes(int col, int row)
    {
        
        List<GameObject> tempHeliDesRandom = new List<GameObject>();
        if (WhichBoostAround(col, row) == 0)
        {
            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    if (GetObjectAt(c, r) != null)
                    {
                        if (targetTags.Contains(GetObjectAt(c, r).tag))
                        {
                            tempHeliDesRandom.Add(GetObjectAt(c, r));
                        }
                    }
                }
            }
            // i included this script not to get first element , it will get a random element.
            int index = Random.Range(0, tempHeliDesRandom.Count);
            AddObjToList(tempHeliDesRandom[index]);
            AddObjToList(GetObjectAt(col, row));

            yield return null;
            //yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
        }
        else if (WhichBoostAround(col,row) == 1)
        {
            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    if (targetTags.Contains(GetObjectAt(c, r).tag))
                    {
                        tempHeliDesRandom.Add(GetObjectAt(c, r));
                    }
                }
            }
            // combo da iki tane heli eleman oluşturmak istediğimden . temphelidesrandom listesinden önceki eklediğimi çıkartmam gerekti.
            // aynı eleman olursa zaten addobjtolist fonk. eklemez tek olur . combo olmasının bir anlamı kalmaz.
            int index = Random.Range(0, tempHeliDesRandom.Count);
            int index2 = Random.Range(0, tempHeliDesRandom.Count);            
            
            tempHeliDesRandom[index].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
            tempHeliDesRandom[index2].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
            AddObjToList(tempHeliDesRandom[index]);
            AddObjToList(tempHeliDesRandom[index2]);
            yield return StartCoroutine(CheckNeighbors(col, row, "helicopter"));
            AddObjToList(GetObjectAt(col, row));

            yield return null;
        }
        else if (WhichBoostAround(col, row) == 2)                                   // heli + rotor
        {
            yield return StartCoroutine(RotorHeliCombo(col, row));
            yield return null;
        }
        else if (WhichBoostAround(col, row) == 3)                                   // heli + TNT
        {
            yield return StartCoroutine(TNTHeliCombo(col, row));

            yield return null;
        }
        else if (WhichBoostAround(col, row) == 4 || WhichBoostAround(col, row) == 5 || WhichBoostAround(col, row) == 6 || WhichBoostAround(col, row) == 7) // colorbomb combo
        {
            if (WhichBoostAround(col, row) == 4)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "blue")
                        {
                            Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
                            AddObjToList(GetObjectAt(c, r));
                        }
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombblue"));
            }
            else if (WhichBoostAround(col, row) == 5)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "green")
                        {
                            Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
                            AddObjToList(GetObjectAt(c, r));
                        }
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombgreen"));
            }
            else if (WhichBoostAround(col, row) == 6)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "red")
                        {
                            Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
                            AddObjToList(GetObjectAt(c, r));
                        }
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombred"));
            }
            else if (WhichBoostAround(col, row) == 7)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "yellow")
                        {
                            Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
                            AddObjToList(GetObjectAt(c, r));
                        }
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombyellow"));
            }
            yield return null;
            //yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
        }
    }


    // Rotor Destroys Horizontal function. Checks if there is another boost in it's way
    public IEnumerator RotDesHor(int col, int row)
    {
        targetTags.Add("blue");
        targetTags.Add("green");
        targetTags.Add("red");
        targetTags.Add("yellow");
        List<GameObject> tempHeliDesRandom = new List<GameObject>();

        if (WhichBoostAround(col, row) == 0)
        {
            for (int i = 0; i < SortForRotor(col).Count; i++)
            {
                string tileTag = GetObjectAt(SortForRotor(col)[i], row).tag;
                if(IsItBoostTile(tileTag)== 0)
                {
                    AddObjToList(GetObjectAt(col, row));
                    AddObjToList(GetObjectAt(SortForRotor(col)[i], row));
                }
                else
                {
                    StartCoroutine(SuperBoostCode(SortForRotor(col)[i], row,tileTag));
                }
            }
        }
        else if (WhichBoostAround(col, row) == 1)
        {
            yield return StartCoroutine(RotorHeliCombo(col, row));
            yield return null;
        }
        else if (WhichBoostAround(col, row) == 2)                                   // rotor + rotor
        {
            for (int i = 0; i < SortForRotor(col).Count; i++)
            {
                string tileTag = GetObjectAt(SortForRotor(col)[i], row).tag;
                if (IsItBoostTile(tileTag) == 0)
                {
                    AddObjToList(GetObjectAt(col, row));
                    AddObjToList(GetObjectAt(SortForRotor(col)[i], row));
                }
                else
                {
                    StartCoroutine(SuperBoostCode(SortForRotor(col)[i], row, tileTag));
                }
            }
            for (int i = 0; i < SortForRotor(row).Count; i++)
            {
                string tileTag = GetObjectAt(col, SortForRotor(row)[i]).tag;
                if (IsItBoostTile(tileTag) == 0)
                {
                    AddObjToList(GetObjectAt(col, row));
                    AddObjToList(GetObjectAt(col, SortForRotor(row)[i]));
                }
                else
                {
                    StartCoroutine(SuperBoostCode(col, SortForRotor(row)[i], tileTag));
                }
            }
            StartCoroutine(CheckNeighbors(col, row, "rotorHor"));
            StartCoroutine(CheckNeighbors(col, row, "rotorVer"));
        }
        else if (WhichBoostAround(col, row) == 3)                                   // rotor + TNT
        {

            yield return StartCoroutine(RotorTNTCombo(col, row));
            
        }
        else if (WhichBoostAround(col, row) == 4 || WhichBoostAround(col, row) == 5 || WhichBoostAround(col, row) == 6 || WhichBoostAround(col, row) == 7) // rotor+Colorbomb Combo
        {
            if (WhichBoostAround(col, row) == 4)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "blue")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }
                        
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombblue"));
            }
            if (WhichBoostAround(col, row) == 5)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "green")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombgreen"));
            }
            if (WhichBoostAround(col, row) == 6)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "red")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombred"));
            }
            if (WhichBoostAround(col, row) == 7)
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "yellow")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombyellow"));
            }
            yield return new WaitForSecondsRealtime(0.4f);
        }
        yield return null;
        //yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
    }


    // Rotor Destroys Vertical function . 
    public IEnumerator RotDesVer(int col, int row)
    {        
        targetTags.Add("blue");
        targetTags.Add("green");
        targetTags.Add("red");
        targetTags.Add("yellow");
        List<GameObject> tempHeliDesRandom = new List<GameObject>();

        if (WhichBoostAround(col, row) == 0)
        {
            for (int i = 0; i < SortForRotor(row).Count; i++)
            {
                string tileTag = GetObjectAt(col, SortForRotor(row)[i]).tag;
                if (IsItBoostTile(tileTag) == 0)
                {
                    AddObjToList(GetObjectAt(col, row));
                    AddObjToList(GetObjectAt(col, SortForRotor(row)[i]));
                }
                else
                {
                    StartCoroutine(SuperBoostCode(col, SortForRotor(row)[i], tileTag));
                }
            }
        }
        else if (WhichBoostAround(col, row) == 1)                                   // rotor + helicopter
        {
            yield return StartCoroutine(RotorHeliCombo(col, row));
            yield return null;
        }
        else if (WhichBoostAround(col, row) == 2)                                   // rotor + rotor
        {
            for (int i = 0; i < SortForRotor(col).Count; i++)
            {
                string tileTag = GetObjectAt(SortForRotor(col)[i], row).tag;
                if (IsItBoostTile(tileTag) == 0)
                {
                    AddObjToList(GetObjectAt(col, row));
                    AddObjToList(GetObjectAt(SortForRotor(col)[i], row));
                }
                else
                {
                    StartCoroutine(SuperBoostCode(SortForRotor(col)[i], row, tileTag));
                }
            }
            for (int i = 0; i < SortForRotor(row).Count; i++)
            {
                string tileTag = GetObjectAt(col, SortForRotor(row)[i]).tag;
                if (IsItBoostTile(tileTag) == 0)
                {
                    AddObjToList(GetObjectAt(col, row));
                    AddObjToList(GetObjectAt(col, SortForRotor(row)[i]));
                }
                else
                {
                    StartCoroutine(SuperBoostCode(col, SortForRotor(row)[i], tileTag));
                }
            }
            StartCoroutine(CheckNeighbors(col, row, "rotorHor"));
            StartCoroutine(CheckNeighbors(col, row, "rotorVer"));
        }
        else if (WhichBoostAround(col, row) == 3)                                   // rotor + TNT
        {
            yield return StartCoroutine(RotorTNTCombo(col, row));
            StartCoroutine(CheckNeighbors(col, row, "TNT"));

        }
        else if (WhichBoostAround(col, row) == 4 || WhichBoostAround(col, row) == 5 || WhichBoostAround(col, row) == 6 || WhichBoostAround(col, row) == 7) // rotor+Colorbomb Combo
        {
            if (WhichBoostAround(col, row) == 4)                                    // rotor + color bomb
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "blue")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombblue"));
            }
            if (WhichBoostAround(col, row) == 5)                                    // rotor + color bomb
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "green")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombgreen"));
            }
            if (WhichBoostAround(col, row) == 6)                                    // rotor + color bomb
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "red")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombred"));
            }
            if (WhichBoostAround(col, row) == 7)                                    // rotor + color bomb combo
            {
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r).tag == "yellow")
                        {
                            StartCoroutine(RotorExplosion(c, r));
                            //AddObjToList(GetObjectAt(c, r));
                        }

                    }
                }
                AddObjToList(GetObjectAt(col, row));
                StartCoroutine(CheckNeighbors(col, row, "colorbombyellow"));
            }
            yield return new WaitForSecondsRealtime(0.4f);
        }
        yield return null;
    }


    // Bomb Destroys 5x5 function
    public IEnumerator TNT(int col, int row, List<GameObject> list)
    {

        if (WhichBoostAround(col, row) == 0)
        {
            StartCoroutine(TNTExplosion(col, row));
                 
        }
        else if (WhichBoostAround(col, row) == 1)                                   // TNT Helicopter combo
        {
            yield return StartCoroutine(TNTHeliCombo(col, row));
        }
        else if (WhichBoostAround(col,row) == 2)                                    // TNT rotor combo
        {
            yield return StartCoroutine(RotorTNTCombo(col, row));
        }
        else if (WhichBoostAround(col, row) == 3)                                   // TNT TNT combo
        {
            yield return StartCoroutine(TNTTNTCombo(col, row));
        }
        else if (WhichBoostAround(col, row) == 4 || WhichBoostAround(col, row) == 5 || WhichBoostAround(col, row) == 6 || WhichBoostAround(col, row) == 7)
        {
            yield return StartCoroutine(ColorBombTNTCombo(col, row));
        }
        yield return null;
    }

    public IEnumerator Colorbomb(int col, int row,string color)
    {
        if (WhichBoostAround(col, row) == 0) // simple same color explosion
        {
            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    if (GetObjectAt(c,r).tag == color)
                    {
                        AddObjToList(GetObjectAt(c, r));
                        
                    }
                }
            }
            AddObjToList(GetObjectAt(col, row));    // kendisini de eklememiz gerekiyor
            
        }
        else if (WhichBoostAround(col, row) == 1) // helicopter combo 
        {
            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    if (GetObjectAt(c, r).tag == color)
                    {
                        Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
                        AddObjToList(GetObjectAt(c, r));
                    }
                }
            }
            StartCoroutine(CheckNeighbors(col, row, "helicopter"));
            AddObjToList(GetObjectAt(col, row));
            
        }

        else if (WhichBoostAround(col, row) == 2) // rotor combo random ver or hor
        {
            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    if (GetObjectAt(c, r).tag == color)
                    {
                        StartCoroutine(RotorExplosion(c, r));
                    }
                }
            }
            AddObjToList(GetObjectAt(col, row));
            StartCoroutine(CheckNeighbors(col, row, "rotorVer"));
            StartCoroutine(CheckNeighbors(col, row, "rotorHor"));
            yield return new WaitForSecondsRealtime(0.4f);
        }

        // IMPORTANT  ColorBombTNTCombo coroutine is not working here because of Whichboost is not 4-7 , it s 3.
        else if (WhichBoostAround(col, row) == 3)                                   // Color Bomb TNT combo
        {

            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    if (GetObjectAt(c, r) != null)
                    {
                        if (GetObjectAt(c, r).tag == color)
                        {
                            Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[3];
                            StartCoroutine(TNTExplosion(c, r));
                        }
                    }

                }
            }
            yield return StartCoroutine(CheckNeighbors(col, row, "TNT"));
            AddObjToList(GetObjectAt(col, row));     // kendisini de eklememiz gerekiyor

        }
        else if (WhichBoostAround(col, row) == 4 || WhichBoostAround(col, row) == 5 || WhichBoostAround(col, row) == 6 || WhichBoostAround(col, row) == 7) // colorbomb combo
        {
            for (int r = 0; r < GridDimension; r++)
            {
                for (int c = 0; c < GridDimension; c++)
                {
                    AddObjToList(GetObjectAt(c, r));
                }
            }
            AddObjToList(GetObjectAt(col, row));     // kendisini de eklememiz gerekiyor
            
        }
        yield return null;
        //yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
    }

    // +++++++++++++++++++++++++++++++++++++++++++  Esas  Boost Fonksiyonları Bitişi ++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++


    // burada listenin elemanlarını çekerken ana objeye en yakından başlatmak gerekiyor 
    public IEnumerator MoveUpRenameObj( List<GameObject> list)                      // NUMBER - 1 BOOST EXECUTION
    {
        
        for (int i = 0; i < list.Count; i++)
        {
            int tempX = list[i].GetComponent<Tile>().Position.x;
            int tempY = list[i].GetComponent<Tile>().Position.y;
            list[i].transform.position = new Vector3(tempX, tempY + 15, 0);
            int index = Random.Range(0, TileSprites.Count);

            list[i].GetComponent<SpriteRenderer>().sprite = TileSprites[index];
            list[i].tag = TagNames[index];
            Grid[list[i].GetComponent<Tile>().Position.x, list[i].GetComponent<Tile>().Position.y] = null;
            yield return null;
        }
        yield return StartCoroutine(MoveObjects());
    }

    // Moves existing objects after explosion to the blank positions
    public IEnumerator MoveObjects()                                                // NUMBER - 2 BOOST EXECUTION
    {
        countForSmoothLerp = 0;
        targetForSmoothLerp = 0;

        for (int i = 0; i < GridDimension; i++)
        {
            for (int j = 0; j < GridDimension; j++)
            {
                int howmanyNulls = CountNulls(i, j);
                if (howmanyNulls > 0 && GetObjectAt(i,j) != null)
                {
                    targetForSmoothLerp++;
                }
                if (i == GridDimension - 1 && j == GridDimension - 1)
                {
                    break;
                }
            }
        }
        
        Debug.Log("targetForSmoothLerp = " + targetForSmoothLerp);

        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                int howmanyNulls = CountNulls(column, row);

                if (howmanyNulls > 0)
                {
                    StartCoroutine(SmoothLerp(0.3f, howmanyNulls, column, row));
                }
            }
        }
        if (targetForSmoothLerp == 0)
        {
            yield return StartCoroutine(RenameDestroyableObjs(destroyableObjs));
        }
        yield return null;
        
    }


    public IEnumerator RenameDestroyableObjs(List<GameObject> list)                 // NUMBER - 3 BOOST EXECUTION
    {
        for (int row = 0; row < GridDimension; row++)
            for (int column = 0; column < GridDimension; column++)
            {
                if (Grid[column, row] == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Grid[column, row] = list[i];
                        GetObjectAt(column, row).GetComponent<Tile>().Position.x = column;
                        GetObjectAt(column, row).GetComponent<Tile>().Position.y = row;
                        GetObjectAt(column, row).transform.position = new Vector3(column, row + 15, 0);
                        GetObjectAt(column, row).name = "(" + column + "," + row + ")";
                        
                        list.Remove(list[i]);
                        break;
                    }
                }
            }        
        yield return StartCoroutine(MoveCreatedObjects());
    }


    // moves new objects to board
    IEnumerator MoveCreatedObjects()                                            // NUMBER - 4 BOOST EXECUTION
    {
        countForSmoothLerpForCreated = 0;
        targetforsmoothlerpForcreated = 0;
        for (int i = 0; i < GridDimension; i++)
        {
            for (int j = 0; j < GridDimension; j++)
            {
                if (GetObjectAt(i, j).transform.position.y > GridDimension)
                {
                    targetforsmoothlerpForcreated++;
                }
            }
        }
        Debug.Log("4-MoveCreatedObjects fonk daki target değişkeni = "+targetforsmoothlerpForcreated);

        for (int row = 0; row < GridDimension; row++)
            for (int column = 0; column < GridDimension; column++)
            {
                if (GetObjectAt(column, row).transform.position.y > GridDimension)
                {
                    StartCoroutine(SmoothLerpForCreated(0.5f, column, row));
                }
            }
        
        destroyableObjs = new List<GameObject>();
        
        yield return null;

    }

    // moves smooth for MoveObjects() function
    private IEnumerator SmoothLerp(float time, int nullCount, int col, int row)
    {
        GameObject go = GetObjectAt(col, row);

        int targetY = row - nullCount;
        Vector2 startingPos = new Vector2(col, row);
        Vector2 finalPos = new Vector2(col, targetY);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            if (go != null)
            {
                go.transform.position = Vector2.Lerp(startingPos, finalPos, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        if (go.transform.position.y != finalPos.y)
        {
            go.transform.position = new Vector3(finalPos.x, finalPos.y, 0);
            countForSmoothLerp++;
        }
        else
        {
            countForSmoothLerp++;
        }
        Grid[col, row] = null;
        StartCoroutine(ChangeGameObject(go));
        if (countForSmoothLerp == targetForSmoothLerp)
        {
            yield return StartCoroutine(RenameDestroyableObjs(destroyableObjs));
        }
        

    }

    // smooth move action for new objects
    private IEnumerator SmoothLerpForCreated(float time, int col, int row)
    {
        GameObject go = GetObjectAt(col, row);

        int targetY = go.GetComponent<Tile>().Position.y;
        Vector2 startingPos = go.transform.position;
        Vector2 finalPos = new Vector2(col, targetY);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            go.transform.position = Vector2.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (go.transform.position.y != finalPos.y)
        {
            go.transform.position = new Vector3(finalPos.x, targetY, 0);
            countForSmoothLerpForCreated++;
        }
        else
        {
            countForSmoothLerpForCreated++;
        }

        if (countForSmoothLerpForCreated == targetforsmoothlerpForcreated)
        {
            yield return StartCoroutine(SpriteController.Instance.CheckTileSprites());
        }

        yield return null;
    }


    // üstüne tıklanan boostun etrafında baska boost varsa en etkili olanını seçer seçmişken

    public int WhichBoostAround(int col, int row)
    {
        
        List<int> whichBoost = new List<int>();
        List<string> aroundObjs = new List<string>();

        if (GetObjectAt(col + 1, row) != null)
        {
            aroundObjs.Add(GetObjectAt(col + 1, row).tag);
        }

        if (GetObjectAt(col - 1, row) != null)
        {
            aroundObjs.Add(GetObjectAt(col - 1, row).tag);
        }

        if (GetObjectAt(col, row - 1) != null)
        {
            aroundObjs.Add(GetObjectAt(col, row - 1).tag);
        }

        if (GetObjectAt(col, row + 1) != null)
        {
            aroundObjs.Add(GetObjectAt(col, row + 1).tag);
                AddObjToList(GetObjectAt(col , row + 1));
        }




        for (int i = 0; i < aroundObjs.Count; i++)
        {
            whichBoost.Add(IsItBoostTile(aroundObjs[i]));
        }
        int maxValue = Mathf.Max(whichBoost.ToArray());
        _whichBoostAround = maxValue;
        return maxValue;
    }

    // etrafında boost var mı diye kontrol etmemin sebebi combo yapmak için.
    private int IsItBoostTile(string objTag)
    {
       
        switch (objTag)
        {
            case "green":
                return 0;
                
            case "blue":
                return 0;
                
            case "red":
                return 0;
                
            case "yellow":
                return 0;
                
            case "helicopter":
                return 1;
                
            case "rotorHor":
                return 2;
                
            case "rotorVer":
                return 2;
                
            case "TNT":
                return 3;
                
            case "colorbombblue":
                return 4;
                
            case "colorbombgreen":
                return 5;
                
            case "colorbombred":
                return 6;
               
            case "colorbombyellow":
                return 7;
                

            default:
                return 0;
                  

        }
    }

    // counts empty positions under objects (it tells how many position to move) 
    private int CountNulls(int col , int row )
    {
        int nullCount = 0;

        for (int i = row; 0 <= i; i--)
        {
            if (GetObjectAt(col, i) == null)
            {
                nullCount++;
            }
        }        
        return nullCount;
    }

    
    // changes objects' properties like Name, Position, Grid[] vs
    IEnumerator ChangeGameObject(GameObject go)
    {
        int goX = (int)go.transform.position.x;
        int goY = (int)go.transform.position.y;

        if (GetObjectAt(goX,goY) == null)
        {
            
            Grid[goX, goY] = go;
            
            Grid[goX, goY].transform.gameObject.GetComponent<Tile>().Position.x = (int)go.transform.position.x;
            Grid[goX, goY].transform.gameObject.GetComponent<Tile>().Position.y = (int)go.transform.position.y;
            go.name = "(" + (int)go.transform.position.x + "," + (int)go.transform.position.y + ")";
        }
        yield return null;
    }


    // combolarda kullanmak için yaptım . Colorbomb da aynı renkte olanları tnt yapmak için kullanıyorum.
    private IEnumerator TNTExplosion(int col, int row)
    {
        for (int j = row - 1; j < row + 2; j++)
        {
            for (int i = col - 1; i < col + 2; i++)
            {
                if (GetObjectAt(i, j) != null)
                {
                    string tileTag = GetObjectAt(i,j).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(i, j));
                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(i,j,tileTag));
                    }
                                      
                }
            }
        }
        yield return null;
    }
    // combolarda kullanmak için yaptım. Colorbomb da aynı renkte olanları rotor yapmak için kullanıyorum. Yön rastgele seçiyor.
    private IEnumerator RotorExplosion(int col, int row)
    {
        int index = Random.Range(1, 10);
        if (index % 2 == 0)
        {
            // yatay rotor oluşturur
            GetObjectAt(col,row).GetComponent<SpriteRenderer>().sprite = BoostSprites[1];
            yield return new WaitForSecondsRealtime(0.05f);
            for (int i = 0; i < SortForRotor(col).Count; i++)
            {
                if (GetObjectAt(SortForRotor(col)[i], row) != null)
                {
                    string tileTag = GetObjectAt(SortForRotor(col)[i], row).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(col, row));
                        AddObjToList(GetObjectAt(SortForRotor(col)[i], row));
                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(SortForRotor(col)[i], row, tileTag));
                    }
                }
                
            }
        }
        else
        {
            // dikey rotor oluşturur
            GetObjectAt(col, row).GetComponent<SpriteRenderer>().sprite = BoostSprites[2];
            yield return new WaitForSecondsRealtime(0.05f);
            for (int i = 0; i < SortForRotor(row).Count; i++)
            {
                if (GetObjectAt(col, SortForRotor(row)[i]) != null)
                {
                    string tileTag = GetObjectAt(col, SortForRotor(row)[i]).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(col, row));
                        AddObjToList(GetObjectAt(col, SortForRotor(row)[i]));
                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(col, SortForRotor(row)[i], tileTag));
                    }
                }
                
            }
        }
        yield return null;
        //yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
    }

    


    // çok amaçlı kullanılabilecek fonksiyon etrafında tag parametresine eşit olanları alıp destroyableObjs listesine atıyor
    public IEnumerator CheckNeighbors(int col , int row , string tag)
    {
        
        if (GetObjectAt(col + 1, row) != null && GetObjectAt(col + 1, row).tag == tag )
        {
            AddObjToList(GetObjectAt(col + 1, row));
        }

        if (GetObjectAt(col -1 , row) != null && GetObjectAt(col - 1, row).tag == tag)
        {
            AddObjToList(GetObjectAt(col -1 , row));
        }

        if (GetObjectAt(col, row - 1) != null && GetObjectAt(col, row - 1).tag == tag)
        {
            AddObjToList(GetObjectAt(col, row - 1));
        }

        if (GetObjectAt(col, row + 1) != null && GetObjectAt(col, row + 1).tag == tag)
        {
            AddObjToList(GetObjectAt(col, row + 1));
        }
        yield return null;
    }

    // rotor boostlarında ilk elemandan başlayarak altlı ve üstlü devam etmesi gereken patlatmada kullanıyorum. Alakasız 0,0 noktasından
    // başlamasın diye. 
    public List<int> SortForRotor(int colOrVer)
    {
        List<int> sortedList = new List<int>();
        
        int [] standardNums = { colOrVer, colOrVer + 1 , colOrVer - 1, colOrVer + 2, colOrVer - 2 ,
            colOrVer + 3, colOrVer - 3, colOrVer + 4, colOrVer - 4, colOrVer + 5, colOrVer-5,colOrVer+6,
            colOrVer-6,colOrVer+7,colOrVer-7,colOrVer+8, colOrVer-8,colOrVer+9,colOrVer-9,colOrVer+10,colOrVer-10,colOrVer+11,colOrVer-11 };
        for (int i = 0; i < standardNums.Length; i++)
        {
            if (standardNums[i] >= 0 && standardNums[i] < GridDimension)
            {
                sortedList.Add(standardNums[i]);
            }
        }
        return sortedList;
    }

    // şu kod ya rotor un vurduklarında boost varsa  ya da TNT etrafında caprazlarda boost varsa çalışacak
    public IEnumerator SuperBoostCode(int col, int row, string tag)
    {
        switch (tag)
        {
            case "helicopter":             // heli
                List<GameObject> tempHeliDesRandom = new List<GameObject>();

                targetTags.Add("blue");
                targetTags.Add("green");
                targetTags.Add("red");
                targetTags.Add("yellow");

                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (targetTags.Contains(GetObjectAt(c, r).tag))
                        {
                            tempHeliDesRandom.Add(GetObjectAt(c, r));
                        }
                    }
                }

                int index = Random.Range(0, tempHeliDesRandom.Count);
                AddObjToList(tempHeliDesRandom[index]);
                AddObjToList(GetObjectAt(col, row));
                break;
            case "rotorHor":
                for (int i = 0; i < SortForRotor(col).Count; i++)
                {
                    if (GetObjectAt(SortForRotor(col)[i], row) != null)
                    {
                        string tileTag = GetObjectAt(SortForRotor(col)[i], row).tag;
                        if (IsItBoostTile(tileTag) == 0)
                        {
                            AddObjToList(GetObjectAt(SortForRotor(col)[i], row));
                        }
                    }
                    
                }
                break;
            case "rotorVer":
                for (int i = 0; i < SortForRotor(row).Count; i++)
                {
                    if (GetObjectAt(col, SortForRotor(row)[i]) != null)
                    {
                        string tileTag = GetObjectAt(col, SortForRotor(row)[i]).tag;
                        if (IsItBoostTile(tileTag) == 0)
                        {
                            AddObjToList(GetObjectAt(col, SortForRotor(row)[i]));
                        }
                    }
                                       
                }
                break;
            case "TNT":
                for (int j = row - 1; j < row + 2; j++)
                {
                    for (int i = col - 1; i < col + 2; i++)
                    {
                        if (GetObjectAt(i, j) != null)
                        {
                            AddObjToList(GetObjectAt(i, j));
                        }
                    }
                }
                break;
            case "colorbombblue":
                string color = "blue";
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r) != null)
                        {
                            if (GetObjectAt(c, r).tag == color)
                            {
                                AddObjToList(GetObjectAt(c, r));
                            }
                        }
                        
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                break;
            case "colorbombgreen":
                color = "green";
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r) != null)
                        {
                            if (GetObjectAt(c, r).tag == color)
                            {
                                AddObjToList(GetObjectAt(c, r));
                            }
                        }
                        
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                break;
            case "colorbombred":
                color = "red";
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r) != null)
                        {
                            if (GetObjectAt(c, r).tag == color)
                            {
                                AddObjToList(GetObjectAt(c, r));
                            }
                        }
                        
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                break;
            case "colorbombyellow":
                color = "yellow";
                for (int r = 0; r < GridDimension; r++)
                {
                    for (int c = 0; c < GridDimension; c++)
                    {
                        if (GetObjectAt(c, r) != null)
                        {
                            if (GetObjectAt(c, r).tag == color)
                            {
                                AddObjToList(GetObjectAt(c, r));
                            }
                        }
                        
                    }
                }
                AddObjToList(GetObjectAt(col, row));
                break;

        }
        yield return null;
        //yield return StartCoroutine(MoveUpRenameObj(destroyableObjs));
    }


    // this method checks obj if it is in destroyableOBjs list if it has that obj doesnt add .
    public void AddObjToList(GameObject obj)
    {
        bool alreadyExist = destroyableObjs.Contains(obj);
        if (!alreadyExist && obj != null)
        {
            destroyableObjs.Add(obj);
            
        }
    }

    // bu fonksiyon patlamadan sonra hangi boost olacağına karar veriyor
    IEnumerator ClickedColorTile(int col, int row, int count, List<GameObject> list, string _color)     // NUMBER - 1 COLOR TILE EXECUTION
    {
        GameObject clicked = GetObjectAt(col, row);
        Debug.Log("destroyableObjs count is ====== " + colorTileDesObjs.Count);

        

        if (count == 4)
        {
            list.Remove(clicked);
            clicked.tag = "helicopter";
            clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[0];
        }
        else if (count == 5)
        {
            list.Remove(clicked);
            int index = Random.Range(1, 10);
            if (index % 2 == 0)
            {
                clicked.tag = "rotorHor";
                clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[1];
            }
            else
            {
                clicked.tag = "rotorVer";
                clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[2];
            }
        }
        else if (6 <= count && count < 8)
        {
            list.Remove(clicked);
            clicked.tag = "TNT";
            clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[3];
        }
        else if (8 <= count)
        {
            list.Remove(clicked);
            switch (_color)
            {
                case "blue":
                    clicked.tag = "colorbombblue";
                    clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[4];
                    break;
                case "green":
                    clicked.tag = "colorbombgreen";
                    clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[5];
                    break;
                case "red":
                    clicked.tag = "colorbombred";
                    clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[6];
                    break;
                case "yellow":
                    clicked.tag = "colorbombyellow";
                    clicked.GetComponent<SpriteRenderer>().sprite = BoostSprites[7];
                    break;

            }
        }

        yield return StartCoroutine(MoveUpRenameColorTile(colorTileDesObjs));
    }

    public IEnumerator MoveUpRenameColorTile(List<GameObject> list)                      // NUMBER - 1 COLOR TILE EXECUTION
    {
        for (int i = 0; i < list.Count; i++)
        {
            int tempX = list[i].GetComponent<Tile>().Position.x;
            int tempY = list[i].GetComponent<Tile>().Position.y;
            list[i].transform.position = new Vector3(tempX, tempY + 15, 0);
            int index = Random.Range(0, TileSprites.Count);

            list[i].GetComponent<SpriteRenderer>().sprite = TileSprites[index];
            list[i].tag = TagNames[index];
            Grid[list[i].GetComponent<Tile>().Position.x, list[i].GetComponent<Tile>().Position.y] = null;
            yield return null;
        }
        yield return StartCoroutine(MoveObjectsColorTile());
    }

    // Moves existing objects after explosion to the blank positions
    public IEnumerator MoveObjectsColorTile()                                                // NUMBER - 2 COLOR TILE EXECUTION
    {
        Debug.Log("2-MoveObjectsColorTile fonk a girdi");

        countForSmoothLerp = 0;
        targetForSmoothLerp = 0;

        for (int i = 0; i < GridDimension; i++)
        {
            for (int j = 0; j < GridDimension; j++)
            {
                int howmanyNulls = CountNulls(i, j);
                if (howmanyNulls > 0 && GetObjectAt(i, j) != null)
                {
                    targetForSmoothLerp++;
                }
                if (i == GridDimension - 1 && j == GridDimension - 1)
                {
                    break;
                }
            }
        }

        

        for (int row = 0; row < GridDimension; row++)
        {
            for (int column = 0; column < GridDimension; column++)
            {
                int howmanyNulls = CountNulls(column, row);

                if (howmanyNulls > 0)
                {
                    StartCoroutine(SmoothLerpColorTile(0.3f, howmanyNulls, column, row));
                }
                if (column == GridDimension - 1 && row == GridDimension - 1)
                {
                    break;
                }
            }
        }
        if (targetForSmoothLerp == 0)
        {
            
            yield return StartCoroutine(RenameColorTileObjs(colorTileDesObjs));
        }
        
        yield return null;
    }



    public IEnumerator RenameColorTileObjs(List<GameObject> list)                 // NUMBER - 3 COLOR TILE EXECUTION
    {
        for (int row = 0; row < GridDimension; row++)
            for (int column = 0; column < GridDimension; column++)
            {
                if (Grid[column, row] == null)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        Grid[column, row] = list[i];
                        GetObjectAt(column, row).GetComponent<Tile>().Position.x = column;
                        GetObjectAt(column, row).GetComponent<Tile>().Position.y = row;
                        GetObjectAt(column, row).transform.position = new Vector3(column, row + 15, 0);
                        GetObjectAt(column, row).name = "(" + column + "," + row + ")";

                        list.Remove(list[i]);
                        break;
                        
                        
                    }
                }
            }
        //yield return null;
        yield return StartCoroutine(MoveCreatedObjectsColorTile());
    }

    // moves new objects to board
    IEnumerator MoveCreatedObjectsColorTile()                                            // NUMBER - 4 COLOR TILE EXECUTION
    {
        countForSmoothLerpForCreated = 0;
        targetforsmoothlerpForcreated = 0;
        for (int i = 0; i < GridDimension; i++)
        {
            for (int j = 0; j < GridDimension; j++)
            {
                if (GetObjectAt(i, j).transform.position.y > GridDimension)
                {
                    targetforsmoothlerpForcreated++;
                }
            }
        }
        Debug.Log("4-MoveCreatedObjects fonk daki target değişkeni = " + targetforsmoothlerpForcreated);

        for (int row = 0; row < GridDimension; row++)
            for (int column = 0; column < GridDimension; column++)
            {
                if (GetObjectAt(column, row).transform.position.y > GridDimension)
                {
                    StartCoroutine(SmoothLerpForCreatedCT(0.5f, column, row));
                }
            }

        colorTileDesObjs = new List<GameObject>();

        yield return null;

    }

    // moves smooth for MoveObjects() function
    private IEnumerator SmoothLerpColorTile(float time, int nullCount, int col, int row)
    {
        GameObject go = GetObjectAt(col, row);

        int targetY = row - nullCount;
        Vector2 startingPos = new Vector2(col, row);
        Vector2 finalPos = new Vector2(col, targetY);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            if (go != null)
            {
                go.transform.position = Vector2.Lerp(startingPos, finalPos, (elapsedTime / time));
                elapsedTime += Time.deltaTime;
            }

            yield return null;
        }

        if (go.transform.position.y != finalPos.y)
        {
            go.transform.position = new Vector3(finalPos.x, finalPos.y, 0);
            countForSmoothLerp++;
        }
        else
        {
            countForSmoothLerp++;
        }
        Grid[col, row] = null;
        StartCoroutine(ChangeGameObject(go));
        Debug.Log("countForSmoothLerpCT = " + countForSmoothLerp);
        if (countForSmoothLerp == targetForSmoothLerp)
        {
            yield return StartCoroutine(RenameColorTileObjs(colorTileDesObjs));
        }
    }

    // smooth move action for new objects
    private IEnumerator SmoothLerpForCreatedCT(float time, int col, int row)
    {
        GameObject go = GetObjectAt(col, row);

        int targetY = go.GetComponent<Tile>().Position.y;
        Vector2 startingPos = go.transform.position;
        Vector2 finalPos = new Vector2(col, targetY);
        float elapsedTime = 0;

        while (elapsedTime < time)
        {
            go.transform.position = Vector2.Lerp(startingPos, finalPos, (elapsedTime / time));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        if (go.transform.position.y != finalPos.y)
        {
            go.transform.position = new Vector3(finalPos.x, targetY, 0);
            countForSmoothLerpForCreated++;
        }
        else
        {
            countForSmoothLerpForCreated++;
        }

        if (countForSmoothLerpForCreated == targetforsmoothlerpForcreated)
        {
            yield return StartCoroutine(SpriteController.Instance.CheckTileSprites());
        }

        yield return null;
    }

    public IEnumerator RotorTNTCombo(int col, int row)
    {
        for (int _c = col - 1; _c < col + 2; _c++)
        {
            for (int _r = 0; _r < row; _r++)
            {
                if (GetObjectAt(_c, _r) != null)
                {
                    string tileTag = GetObjectAt(_c, _r).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(_c, _r));

                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(_c, _r, tileTag));
                    }
                }

            }
            for (int _r = row; _r < GridDimension; _r++)
            {
                if (GetObjectAt(_c, _r) != null)
                {
                    string tileTag = GetObjectAt(_c, _r).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(_c, _r));
                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(_c, _r, tileTag));
                    }
                }

            }
        }
        for (int r = row - 1; r < row + 2; r++)
        {
            for (int c = 0; c < col; c++)
            {
                if (GetObjectAt(c, r) != null)
                {
                    string tileTag = GetObjectAt(c, r).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(c, r));

                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(c, r, tileTag));
                    }
                }

            }
            for (int c = col; c < GridDimension; c++)
            {
                if (GetObjectAt(c, r) != null)
                {
                    string tileTag = GetObjectAt(c, r).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(c, r));
                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(c, r, tileTag));
                    }
                }

            }
        }
        StartCoroutine(CheckNeighbors(col, row, "rotorHor"));
        StartCoroutine(CheckNeighbors(col, row, "rotorVer"));
        StartCoroutine(CheckNeighbors(col, row, "TNT"));
        yield return null;
    }


    public IEnumerator RotorHeliCombo(int col , int row)
    {
        List<GameObject> tempHeliDesRandom = new List<GameObject>();
        for (int r = 0; r < GridDimension; r++)
        {
            for (int c = 0; c < GridDimension; c++)
            {
                if (targetTags.Contains(GetObjectAt(c, r).tag))
                {
                    tempHeliDesRandom.Add(GetObjectAt(c, r));
                }
            }
        }
        // bunu ilk elemanı almasın, board'da herhangi bir elemanı alsın diye yazdım.
        int index = Random.Range(0, tempHeliDesRandom.Count);
        // rotor olacak elemanı seçtikten sonra sprite ı nı değiştirip rotor u çalıştırmamız gerekiyor.
        int _c = tempHeliDesRandom[index].GetComponent<Tile>().Position.x;
        int _r = tempHeliDesRandom[index].GetComponent<Tile>().Position.y;

        yield return StartCoroutine(RotorExplosion(_c, _r));
        yield return StartCoroutine(CheckNeighbors(col, row, "rotorHor"));
        yield return StartCoroutine(CheckNeighbors(col, row, "rotorVer"));
        yield return StartCoroutine(CheckNeighbors(col, row, "helicopter"));
        AddObjToList(GetObjectAt(col, row));
    }


    public IEnumerator TNTHeliCombo(int col , int row)
    {
        List<GameObject> tempHeliDesRandom = new List<GameObject>();
        for (int r = 0; r < GridDimension; r++)
        {
            for (int c = 0; c < GridDimension; c++)
            {
                if (targetTags.Contains(GetObjectAt(c, r).tag))
                {
                    tempHeliDesRandom.Add(GetObjectAt(c, r));
                }
            }
        }
        int index = Random.Range(0, tempHeliDesRandom.Count);

        int _c = tempHeliDesRandom[index].GetComponent<Tile>().Position.x;
        int _r = tempHeliDesRandom[index].GetComponent<Tile>().Position.y;
        Grid[_c, _r].GetComponent<SpriteRenderer>().sprite = BoostSprites[3];
        yield return new WaitForSecondsRealtime(0.2f);
        StartCoroutine(TNTExplosion(_c, _r));
        AddObjToList(tempHeliDesRandom[index]);             // Rastgele seçilen tile'ı yokeder.
        yield return StartCoroutine(CheckNeighbors(col, row, "TNT"));
        yield return StartCoroutine(CheckNeighbors(col, row, "helicopter"));
        AddObjToList(GetObjectAt(col, row));
    }


    public IEnumerator TNTTNTCombo(int col , int row)
    {
        for (int j = row - 2; j < row + 3; j++)
        {
            for (int i = col - 2; i < col + 3; i++)
            {
                if (GetObjectAt(i, j) != null)
                {
                    string tileTag = GetObjectAt(i, j).tag;
                    if (IsItBoostTile(tileTag) == 0)
                    {
                        AddObjToList(GetObjectAt(i, j));
                    }
                    else
                    {
                        StartCoroutine(SuperBoostCode(i, j, tileTag));
                    }

                }
            }
        }
        yield return StartCoroutine(CheckNeighbors(col, row, "TNT"));
        yield return StartCoroutine(CheckNeighbors(col, row, "TNT"));
        yield return null;
    }

    public IEnumerator ColorBombTNTCombo (int col, int row)
    {
        string _color = "";
        switch (WhichBoostAround(col,row))
        {
            case 4:
                _color = "blue";
                break;
            case 5:
                _color = "green";
                break;
            case 6:
                _color = "red";
                break;
            case 7:
                _color = "yellow";
                break;
        }
        for (int r = 0; r < GridDimension; r++)
        {
            for (int c = 0; c < GridDimension; c++)
            {
                if (GetObjectAt(c, r) != null)
                {
                    if (GetObjectAt(c, r).tag == _color)
                    {
                        Grid[c, r].GetComponent<SpriteRenderer>().sprite = BoostSprites[3];
                        StartCoroutine(TNTExplosion(c, r));
                    }
                }

            }
        }

        AddObjToList(GetObjectAt(col, row));
        yield return StartCoroutine(CheckNeighbors(col, row, "TNT"));
        yield return StartCoroutine(CheckNeighbors(col, row, "colorbombblue"));
        yield return StartCoroutine(CheckNeighbors(col, row, "colorbombgreen"));
        yield return StartCoroutine(CheckNeighbors(col, row, "colorbombred"));
        yield return StartCoroutine(CheckNeighbors(col, row, "colorbombyellow"));
        yield return null;
    }




}

