using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionsReciever : MonoBehaviour
{
    Queue<GameObject> actionsList;
    Queue<GameObject> tutorialList;
    
    public Transform actionsTransform;
    public Transform tutorialActionsTransform;

    public GameObject UpArrow;
    public GameObject RightArrow;
    public GameObject BottomArrow;
    public GameObject LeftArrow;

    private void Awake()
    {
        actionsList = new Queue<GameObject>();
        tutorialList = new Queue<GameObject>();
    }

    public void AddAction(Direction direction)
    {
        switch (direction)
        {
            case Direction.Up:
                actionsList.Enqueue(Instantiate(UpArrow, actionsTransform));
                break;
            case Direction.Right:
                actionsList.Enqueue(Instantiate(RightArrow, actionsTransform));
                break;
            case Direction.Bottom:
                actionsList.Enqueue(Instantiate(BottomArrow, actionsTransform));
                break;
            case Direction.Left:
                actionsList.Enqueue(Instantiate(LeftArrow, actionsTransform));
                break;
            default:
                break;
        }
    }

    public void AddTutorialActions(Queue<Direction> actions)
    {
        foreach (Direction direction in actions)
        {
            switch (direction)
            {
                case Direction.Up:
                    tutorialList.Enqueue(Instantiate(UpArrow, tutorialActionsTransform));
                    break;
                case Direction.Right:
                    tutorialList.Enqueue(Instantiate(RightArrow, tutorialActionsTransform));
                    break;
                case Direction.Bottom:
                    tutorialList.Enqueue(Instantiate(BottomArrow, tutorialActionsTransform));
                    break;
                case Direction.Left:
                    tutorialList.Enqueue(Instantiate(LeftArrow, tutorialActionsTransform));
                    break;
                default:
                    break;
            }
        }
    }

    public void RemoveAction()
    {
        if(actionsList.Count > 0)
            Destroy(actionsList.Dequeue());
    }

    public void ClearActionsList()
    {
        foreach (GameObject action in actionsList)
        {
            Destroy(action);
        }
        actionsList.Clear();
    }    
    public void ClearTutorialActions()
    {
        if(tutorialList != null)
        {
            foreach (GameObject action in tutorialList)
            {
                Destroy(action);
            }
            tutorialList.Clear();
        }
    }
}

public enum Direction
{
    Up,
    Right,
    Bottom,
    Left
}
