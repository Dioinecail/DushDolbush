using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTilesReadyEvent();

public class Level : MonoBehaviour
{
    public event OnTilesReadyEvent onTilesReady;

    public GridTile[,] grid;
    TileHolder[,] tiles;
    public int sizeX, sizeY;

    public int currentGridEnd;
    public int stickSpawnChance;
    public GameObject tilePrefab;
    public GameObject StickPrefab;
    public Sprite TopSide;
    public Sprite BottomSide;
    Player player;
    Coroutine fillWaitCoroutine;

    private void Awake()
    {
        BuildLevel();
        CameraControl.verticalPosition = sizeY / 2;
        player = FindObjectOfType<Player>();
    }

    public void BuildLevel()
    {
        grid = new GridTile[sizeX, sizeY];
        tiles = new TileHolder[sizeX, sizeY];
    }

    public void SetTile(TileHolder tile)
    {
        if(IsInsideGrid(tile.x, tile.y) && tile.tileType != TileType.Normal)
        {
            grid[tile.x, tile.y] = new GridTile(tile.x, tile.y, tile.tileType);
            tiles[tile.x, tile.y] = tile;
            if (tile.y == grid.GetLength(1) - 1)
            {
                tile.SetSprite(TopSide);
                if (Random.Range(0, 101) < stickSpawnChance)
                {
                    Transform stick = SpawnStick(true);
                    stick.SetParent(tile.transform);
                    stick.localPosition = new Vector3(-0.25f, 0.9f);
                }

            }
            else if (tile.y == 0)
            {
                tile.SetSprite(BottomSide);
                if (Random.Range(0, 101) < stickSpawnChance)
                {
                    Transform stick = SpawnStick(false);
                    stick.SetParent(tile.transform);
                    stick.localPosition = new Vector3(-0.25f, -0.9f);
                }
            }
            if (fillWaitCoroutine != null)
                StopCoroutine(fillWaitCoroutine);

            fillWaitCoroutine = StartCoroutine(WaitForTiles());
        }
    }

    public Transform SpawnStick(bool topSide)
    {
        Transform stick = Instantiate(StickPrefab).transform;
        stick.GetComponent<SpriteRenderer>().flipX = true;
        stick.GetComponent<SpriteRenderer>().flipY = !topSide;
        return stick;
    }

    /// <summary>
    /// Destroys rows of tiles based by parameters
    /// </summary>
    /// <param name="rows"></param>
    public void DestroyTiles(int rows)
    {
        StartCoroutine(DestroyTilesByPacks());
    }



    void FillEmptySpaces()
    {
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                if (grid[x,y].tileType == TileType.Normal)
                {
                    TileHolder tile = Instantiate(tilePrefab, transform).GetComponent<TileHolder>();
                    tile.x = x;
                    tile.y = y;
                    tiles[x, y] = tile;

                    grid[x, y].X = x;
                    grid[x, y].Y = y;

                    tile.transform.position = new Vector3(x, y);
                }
                if(x == grid.GetLength(0) - 1)
                {
                    if(grid[x,y].tileType != TileType.Impassable)
                        grid[x, y].tileType = TileType.Victory;
                }
            }
        }

        if(onTilesReady != null)
        {
            onTilesReady();
        }
    }

    public Effect GetTileEffect(int x, int y)
    {
        switch (grid[x,y].tileType)
        {
            case TileType.Slow:
                return Effect.Slowed;
            case TileType.Web:
                return Effect.Stuck;
            case TileType.Trap:
                return Effect.Normal;
            default:
                return Effect.Normal;
        }
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && x < sizeX & y >= 0 && y < sizeY;
    }

    IEnumerator WaitForTiles()
    {
        yield return new WaitForSeconds(0.1f);
        FillEmptySpaces();
    }

    IEnumerator DestroyTilesByPacks()
    {
        if (currentGridEnd >= grid.GetLength(0))
        {
            yield break;
        }
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            DestroyTile(currentGridEnd, i);
            if(i % 3 == 0)
            {
                yield return new WaitForSeconds(0.2f);
            }
        }
        currentGridEnd++;
    }

    public void DestroyTile(int x, int y)
    {
        tiles[x, y].GetDestroyed();
        grid[x, y].tileType = TileType.Dead;
        if(player != null)
        {
            if(player.position.x == x && player.position.y == y)
            {
                player.OnDie();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(new Vector3(sizeX / 2 - 0.5f, sizeY / 2), new Vector3(sizeX, sizeY));
    }
}