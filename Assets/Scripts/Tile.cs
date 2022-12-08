using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Tile tile; // 1    
    public Vector2Int Position;
    public int howManySimilars = 0;

    

    // target objelerinin yanında kaç kere patlatılacaksa o kadar olmalı. Misal yanında 3. kere normal tile patlatınca yok olacaksa 2 olmalı.
    // 
    public int tarObjLives = 0;



    private SpriteRenderer Renderer; // 2
    


    private void Start() // 3
    {
        Renderer = GetComponent<SpriteRenderer>();
        
    }

    private void Update()
    {
        
    }

    


    private void OnMouseDown() 
    {
        if (!GridManager.isInAction && IsItColorTile() == true)
        {
            tile = this;
            GridManager.clickedX = tile.Position.x;
            GridManager.clickedY = tile.Position.y;
            GridManager.Instance.FindAll(tile.Position.x, tile.Position.y);
            
        }
        else if (!GridManager.isInAction && IsItColorTile() == false)
        {
            switch (tile.tag)
            {
                case "helicopter":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 0));
                    break;
                case "rotorHor":                    
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 1));
                    break;
                case "rotorVer":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 2));
                    break;
                case "TNT":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 3));
                    break;
                case "colorbombblue":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 4));
                    break;
                case "colorbombgreen":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 5));
                    break;
                case "colorbombred":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 6));
                    break;
                case "colorbombyellow":
                    StartCoroutine(GridManager.Instance.BoostClicked(tile.Position.x, tile.Position.y, 7));
                    break;
                default:
                    break;
            }
        }                
    }


    // checks if clicked object is color tile , if yes than returns true
    private bool IsItColorTile()
    {
        tile = this;
        switch (tile.tag)
        {
            case "blue":
                return true;
            case "yellow":
                return true;
            case "red":
                return true;
            case "green":
                return true;
            
            default:
                return false;
        }
    }






}
