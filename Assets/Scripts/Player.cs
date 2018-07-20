using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    public float speedMultiplier = 1;
    int x, y;

    public Vector2 position
    {
        get { return new Vector2(x, y); }
    }
    public AudioClip loseSound;
    public AudioClip victorySound;

    Level level;
    ActionsReciever actionsUI;
    public Transform[] tail;

    Coroutine moveCoroutine;
    Coroutine effectCoroutine;

    public Effect currentEffect;
    public int EffectDuration;

    public Queue<Vector2> movesQueue;
    public List<Direction> actions;

    Tutorial tutorial;
    bool tutorialMode = false;
    bool dead = false;

    void Start()
    {
        level = FindObjectOfType<Level>();
        actionsUI = FindObjectOfType<ActionsReciever>();
        tutorial = FindObjectOfType<Tutorial>();

        if(tutorial != null)
        {
            tutorialMode = true;
            tutorial.onTutorialFinish += OnFinishTutorial;
        }

        x = Mathf.RoundToInt(transform.position.x);
        y = Mathf.RoundToInt(transform.position.y);
        movesQueue = new Queue<Vector2>();

        for (int i = 0; i < tail.Length; i++)
        {
            tail[i].SetParent(null);
        }

        GameManager.Instance.onPhaseChange += OnPhaseChange;
    }

    private void OnDisable()
    {
        GameManager.Instance.onPhaseChange -= OnPhaseChange;

        if (tutorial != null)
            tutorial.onTutorialFinish -= OnFinishTutorial;
    }

    void Update()
    {
        if (!GameManager.Instance.isMoving)
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                AddMove(new Vector2(1, 0));
                actionsUI.AddAction(Direction.Right);
                actions.Add(Direction.Right);
            }
            if (Input.GetKeyDown(KeyCode.A))
            {
                AddMove(new Vector2(-1, 0));
                actionsUI.AddAction(Direction.Left);
                actions.Add(Direction.Left);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                AddMove(new Vector2(0, 1));
                actionsUI.AddAction(Direction.Up);
                actions.Add(Direction.Up);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                AddMove(new Vector2(0, -1));
                actionsUI.AddAction(Direction.Bottom);
                actions.Add(Direction.Bottom);
            }
        }
    }

    void AddMove(Vector2 move)
    {
        movesQueue.Enqueue(move);
    }

    void OnPhaseChange(bool start)
    {
        if (start)
        {
            if (tutorialMode)
            {
                if(tutorial.CompareTutorial(actions))
                {
                    moveCoroutine = StartCoroutine(MoveByTurns());
                }
                else
                {
                    movesQueue.Clear();
                    actions.Clear();
                    actionsUI.ClearActionsList();
                    tutorial.FinishedTutorialMove();
                }
            }
            else
            {
                moveCoroutine = StartCoroutine(MoveByTurns());
            }
        }
        else
        {
            actions.Clear();
            movesQueue.Clear();
            actionsUI.ClearActionsList();
        }
    }

    public void OnDie()
    {
        if (dead)
            return;

        GetComponent<AudioSource>().PlayOneShot(loseSound);
        // Play animation
        for (int i = 0; i < tail.Length; i++)
        {
            Rigidbody2D body = tail[i].gameObject.AddComponent<Rigidbody2D>();
            Vector2 direction = new Vector2(Random.Range(-15, 15), 15);
            body.AddForce(direction, ForceMode2D.Impulse);
        }
        dead = true;
        GameManager.Instance.ShowCurtain(false);
    }

    void OnVictory()
    {
        GetComponent<AudioSource>().PlayOneShot(victorySound);
        tail[0].SetParent(transform);

        for (int i = 1; i < tail.Length; i++)
        {
            tail[i].SetParent(tail[i - 1]);
        }

        // Play animation
        GetComponent<Animation>().Play();
        GameManager.Instance.ShowVictoryCurtain();
    }

    void OnFinishTutorial()
    {
        tutorialMode = false;
    }

    IEnumerator Move(Vector2 direction)
    {
        int xDir = Mathf.RoundToInt(direction.x);
        int yDir = Mathf.RoundToInt(direction.y);

        if (level.IsInsideGrid(x + xDir, y + yDir))
        {
            if (level.grid[x + xDir, y + yDir].tileType != TileType.Impassable)
            {
                switch (currentEffect)
                {
                    case Effect.Normal:
                        yield return MoveTo(xDir, yDir);
                        break;
                    case Effect.Slowed:
                        yield return SpendStuckTurn();
                        break;
                    case Effect.Stuck:
                        yield return SpendStuckTurn();
                        break;
                    default:
                        break;
                }
            }
        }
        actionsUI.RemoveAction();
        GameManager.Instance.SpendTurns(1);
    }

    IEnumerator MoveByTurns()
    {
        // Take moves list and move by it
        while (movesQueue.Count > 0)
        {            
            Debug.Log("Turns left : " + movesQueue.Count);
            yield return Move(movesQueue.Dequeue());
        }

        Debug.Log("Stopping movement");
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        if(tutorialMode)
        {
            tutorial.ShowTutorial();
            tutorial.FinishedTutorialMove();
        }
        else if (tutorial != null)
        {
            tutorial.FinishedTutorialMove();
        }
    }

    IEnumerator MoveTo(int x, int y, float speedMult = 1)
    {
        Vector2 oldPosition = transform.position;
        while((Vector2)transform.position != oldPosition + new Vector2(x, y))
        {
            transform.position = Vector2.MoveTowards(transform.position, oldPosition + new Vector2(x, y), speedMult * speed * Time.deltaTime);
            
            Vector3 tailDirection = tail[0].position - transform.position;
            tailDirection = Vector3.ClampMagnitude(tailDirection, 0.2f);
            tail[0].position = transform.position + tailDirection;

            for (int i = 1; i < tail.Length; i++)
            {
                tail[i].position = tail[i - 1].position + tailDirection;
            }

            yield return new WaitForEndOfFrame();
        }

        this.x += x;
        this.y += y;

        if(level.grid[this.x, this.y].tileType == TileType.Dead)
        {
            // Game over
            OnDie();
        }
        else if (level.grid[this.x, this.y].tileType == TileType.Victory)
        {
            OnVictory();
        }

        Effect effect = level.GetTileEffect(this.x, this.y);

        if (effect != Effect.Normal)
        {
            currentEffect = effect;

            switch (currentEffect)
            {
                case Effect.Slowed:
                    EffectDuration = 1;
                    break;
                case Effect.Stuck:
                    EffectDuration = 2;
                    break;
                default:
                    break;
            }
        }
    }

    IEnumerator SpendStuckTurn()
    {
        for (int i = 0; i < 10; i++)
        {
            // Play some animation
            yield return null;
        }

        EffectDuration--;

        if (EffectDuration == 0)
        {
            currentEffect = Effect.Normal;
        }
    }
}

public enum Effect
{
    Normal,
    Slowed,
    Stuck
}