using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridShaper : MonoBehaviour
{

    public List<string> targetTags = new List<string>();
    public List<Sprite> TargetSprites = new List<Sprite>();

    public static GridShaper Instance;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }


    public void InitGrid()
    {

        for (int row = 0; row < GridManager.GridDimension; row++)
            for (int column = 0; column < GridManager.GridDimension; column++)
            {
                CreateColorTiles(column, row);
            }
    }



    void CreateColorTiles(int col , int row)
    {
        int index = Random.Range(0, GridManager.Instance.TilePrefabs.Count);
        Vector2 tempPosition = new Vector2(col, row);
        GameObject newTile = Instantiate(GridManager.Instance.TilePrefabs[index], tempPosition, Quaternion.identity) as GameObject;
        newTile.tag = GridManager.Instance.TagNames[index];

        Tile tile = newTile.AddComponent<Tile>();
        tile.Position = new Vector2Int(col, row);
        newTile.transform.parent = transform;
        newTile.transform.position = new Vector3(col, row, 0);
        newTile.name = "(" + col + "," + row + ")";

        GridManager.Grid[col, row] = newTile;
    }

    void CreateTarget(int col, int row, int type)
    {
        int index = Random.Range(0, GridManager.Instance.TilePrefabs.Count);
        Vector2 tempPosition = new Vector2(col, row);
        GameObject newTile = Instantiate(GridManager.Instance.TilePrefabs[index], tempPosition, Quaternion.identity) as GameObject;
        newTile.GetComponent<SpriteRenderer>().sprite = TargetSprites[type];
        newTile.tag = "target";

        Tile tile = newTile.AddComponent<Tile>();
        tile.Position = new Vector2Int(col, row);
        newTile.GetComponent<Tile>().tarObjLives = 2;
        newTile.transform.parent = transform;
        newTile.transform.position = new Vector3(col, row, 0);
        newTile.name = "(" + col + "," + row + ")";

        GridManager.Grid[col, row] = newTile;
    }
    void CreateEmpty(int col, int row, int type)
    {
        int index = Random.Range(0, GridManager.Instance.TilePrefabs.Count);
        Vector2 tempPosition = new Vector2(col, row);
        GameObject newTile = Instantiate(GridManager.Instance.TilePrefabs[index], tempPosition, Quaternion.identity) as GameObject;
        newTile.GetComponent<SpriteRenderer>().sprite = null;
        newTile.tag = "target";

        Tile tile = newTile.AddComponent<Tile>();
        tile.Position = new Vector2Int(col, row);
        
        newTile.transform.parent = transform;
        newTile.transform.position = new Vector3(col, row, 0);
        newTile.name = "(" + col + "," + row + ")";

        GridManager.Grid[col, row] = newTile;
    }



    public IEnumerator CheckTargetObjs(int col , int row ,string tag)
    {
        if (GridManager.Instance.GetObjectAt(col + 1, row) != null && GridManager.Instance.GetObjectAt(col + 1, row).tag == tag)
        {
            //GridManager.Instance.AddObjToList(GridManager.Instance.GetObjectAt(col + 1, row));
            yield return StartCoroutine(CheckTarObjLives(col + 1, row));
        }

        if (GridManager.Instance.GetObjectAt(col - 1, row) != null && GridManager.Instance.GetObjectAt(col - 1, row).tag == tag)
        {
            //GridManager.Instance.AddObjToList(GridManager.Instance.GetObjectAt(col - 1, row));
            yield return StartCoroutine(CheckTarObjLives(col + 1, row));
        }

        if (GridManager.Instance.GetObjectAt(col, row - 1) != null && GridManager.Instance.GetObjectAt(col, row - 1).tag == tag)
        {
            //GridManager.Instance.AddObjToList(GridManager.Instance.GetObjectAt(col, row - 1));
            yield return StartCoroutine(CheckTarObjLives(col + 1, row));
        }

        if (GridManager.Instance.GetObjectAt(col, row + 1) != null && GridManager.Instance.GetObjectAt(col, row + 1).tag == tag)
        {
            //GridManager.Instance.AddObjToList(GridManager.Instance.GetObjectAt(col, row + 1));
            yield return StartCoroutine(CheckTarObjLives(col + 1, row));
        }
        yield return null;

    }

    // if the tile has target it ll go down to check if there s empty 
    public IEnumerator CheckTarObjLives(int col, int row)
    {
        int _tarobjlives = GridManager.Instance.GetObjectAt(col, row).GetComponent<Tile>().tarObjLives;
        if (_tarobjlives > 0)
        {
            _tarobjlives--;
            GridManager.Instance.GetObjectAt(col, row).GetComponent<SpriteRenderer>().sprite = TargetSprites[_tarobjlives];
        }
        else
        {
            GridManager.Instance.AddObjToList(GridManager.Instance.GetObjectAt(col, row));
            for (int i = row; i > 0; i--)
            {
                GridManager.Instance.AddObjToList(GridManager.Instance.GetObjectAt(col, i));
            }
        }



        yield return null;
    }

}
