using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHolder : MonoBehaviour
{
    public TileType tileType;
    public int x, y;

    SpriteRenderer sprite;

	void Start ()
    {
        Level level = FindObjectOfType<Level>();
        sprite = GetComponent<SpriteRenderer>();

        level.SetTile(this);
        GameManager.Instance.SubscribeForTurn(OnTurn);
    }

    [ContextMenu("Set position")]
    public void SetPosition()
    {
        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);

        transform.position = new Vector3(x, y);
    }
    [ContextMenu("Randomize")]
    public void Randomize()
    {
        int rnd = Random.Range(0, 2);

        tileType = (TileType)rnd;
    }

    public void SetSprite(Sprite spr)
    {
        sprite.sprite = spr;
    }

    public void GetDestroyed()
    {
        Rigidbody2D block = gameObject.AddComponent<Rigidbody2D>();
        Vector2 force = new Vector2(Random.Range(-45, 46), 45);
        force.Normalize();
        float randomForce = Random.Range(0, 45);

        if(block != null)
            block.AddForce(force * randomForce, ForceMode2D.Impulse);

        transform.SetParent(null);
        Destroy(gameObject, 5);
    }

    void OnDisable()
    {
        GameManager.Instance.Unsubscribe(OnTurn);    
    }

    protected virtual void OnTurn()
    {
        // Do turn action here
    }

    void OnDrawGizmos()
    {
        Color tileColor = Color.white;

        switch (tileType)
        {
            case TileType.Impassable:
                tileColor = Color.black;
                break;
            case TileType.Slow:
                tileColor = Color.green;
                break;
            case TileType.Web:
                tileColor = Color.grey;
                break;
            case TileType.Trap:
                tileColor = Color.red;
                break;
            default:
                break;
        }

        Gizmos.color = tileColor;

        int roundedX = Mathf.RoundToInt(transform.position.x);
        int roundedY = Mathf.RoundToInt(transform.position.y);

        Gizmos.DrawWireCube(new Vector3(roundedX, roundedY), Vector3.one);
    }
}
