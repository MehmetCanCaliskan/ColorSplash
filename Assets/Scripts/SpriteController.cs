using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour
{
    
    public List<GameObject> similarNeighbors;
    public List<Sprite> blueSprites = new List<Sprite>();
    public List<Sprite> greenSprites = new List<Sprite>();
    public List<Sprite> yellowSprites = new List<Sprite>();
    public List<Sprite> redSprites = new List<Sprite>();
    public int _count = 0;


    public static SpriteController Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        //Debug.Log("similarNeighbors sayısı= " + similarNeighbors.Count);
    }

    public IEnumerator CheckTileSprites()
    {
        
        Debug.Log("5-CheckTileSprites fonk a girdi ");
        for (int column = 0; column < GridManager.GridDimension; column++)
            for (int row = 0; row < GridManager.GridDimension; row++)
            {
                if (GetObjectAt(column, row) == null)
                {
                    Debug.Log("---------- FOUND NULL -----------------");
                }
                while (GetObjectAt(column, row) == null)
                {
                    yield return null;
                }
                similarNeighbors.Clear();
                _count = 0;
                string _t = GridManager.Instance.GetObjectAt(column,row).tag;
                if (_t== "blue" || _t == "green" || _t == "red" || _t == "yellow")
                {
                    FindNeighbors(column, row);
                }
                
            }
        GridManager.isInAction = false;
        yield return null;
        Debug.Log("5-CheckTileSprites dan çıkıyor ");
    }

    public GameObject GetObjectAt(int column, int row)
    {
        if (column < 0 || column >= GridManager.GridDimension
             || row < 0 || row >= GridManager.GridDimension)
            return null;
        GameObject tile = GridManager.Grid[column, row];
        return tile;
    }

    public List<GameObject> DirectNeighbors(int col, int row)
    {

        bool _alreadyExist = similarNeighbors.Contains(GetObjectAt(col, row));
        if (!_alreadyExist)
        {
            similarNeighbors.Add(GetObjectAt(col, row));
        }

        GameObject _left = GetObjectAt(col - 1, row);
        if (_left != null && _left.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = similarNeighbors.Contains(_left);
            if (!alreadyExist)
            {
                similarNeighbors.Add(_left);
            }
        }

        GameObject _right = GetObjectAt(col + 1, row);
        if (_right != null && _right.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = similarNeighbors.Contains(_right);
            if (!alreadyExist)
            {
                similarNeighbors.Add(_right);
            }
        }

        GameObject _up = GetObjectAt(col, row + 1);
        if (_up != null && _up.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = similarNeighbors.Contains(_up);
            if (!alreadyExist)
            {
                similarNeighbors.Add(_up);
            }
        }

        GameObject _down = GetObjectAt(col, row - 1);
        if (_down != null && _down.tag == GetObjectAt(col, row).tag)
        {
            bool alreadyExist = similarNeighbors.Contains(_down);
            if (!alreadyExist)
            {
                similarNeighbors.Add(_down);
            }
        }
        return similarNeighbors;
    }

    public void FindNeighbors(int col, int row)
    {
        
        
        int clickedCol = col;
        int clickedRow = row;
        
        for (int i = 0; i < DirectNeighbors(col, row).Count; i++)
        {
            int _col = DirectNeighbors(col, row)[i].gameObject.GetComponent<Tile>().Position.x;
            int _row = DirectNeighbors(col, row)[i].gameObject.GetComponent<Tile>().Position.y;
            DirectNeighbors(_col, _row);
        }
        _count = similarNeighbors.Count;
        string color = GetObjectAt(clickedCol, clickedRow).tag;

        for (int i = 0; i < similarNeighbors.Count; i++)
        {
            string _tag = similarNeighbors[i].tag;
            int _index = _ReturnSprIndex(_count);
            similarNeighbors[i].GetComponent<SpriteRenderer>().sprite = _ReturnSprList(_tag)[_index];
            similarNeighbors[i].GetComponent<Tile>().howManySimilars = _count;
        }

        

    }

    public List<Sprite> _ReturnSprList(string _tag)
    {

        switch (_tag)
        {
            case "blue":
                return blueSprites;
            case "green":
                return greenSprites;
            case "yellow":
                return yellowSprites;
            case "red":
                return redSprites;
            default:
                return null;

        }
    }

    public int _ReturnSprIndex(int _howManySimilars)
    {

        if (_howManySimilars <= 3)
        {
            return 0;
        }
        else if (_howManySimilars == 4)
        {
            return 1;
        }
        else if (_howManySimilars == 5)
        {
            return 2;
        }
        else if (6 <= _howManySimilars && _howManySimilars < 8)
        {
            return 3;
        }
        else if (8 <= _howManySimilars)
        {
            return 4;
        }
        else
        {
            return 0;
        }
    }

}
