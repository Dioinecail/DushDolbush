using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public event CustomEvent onTutorialFinish;

    public DirectionArray[] actionsArray;
    Queue<Direction>[] actions;
    public GameObject[] tutorialTargets;

    ActionsReciever actionsUI;
    public int currentTutorial;

    public Text TutorialText;

    bool thinkPhase;

    private void Start()
    {
        actionsUI = FindObjectOfType<ActionsReciever>();
        actions = new Queue<Direction>[actionsArray.Length];

        for (int i = 0; i < actions.Length; i++)
        {
            actions[i] = new Queue<Direction>();
        }


        for (int i = 0; i < actions.Length; i++)
        {
            for (int j = 0; j < actionsArray[i].directions.Length; j++)
            {
                actions[i].Enqueue(actionsArray[i].directions[j]);
            }
        }

        StartCoroutine(SwitchPhases());
        ShowTutorial();
    }

    public void ShowTutorial()
    {
        currentTutorial++;
        if (currentTutorial < actions.Length)
        {
            actionsUI.ClearTutorialActions();
            tutorialTargets[currentTutorial].SetActive(true);

            if(currentTutorial > 0)
            {
                tutorialTargets[currentTutorial - 1].SetActive(false);
            }

            actionsUI.AddTutorialActions(actions[currentTutorial]);
        }
        else
        {
            // No more tutorials
            TutorialText.text = "Try to complete the level by yourself now!";
            actionsUI.ClearTutorialActions();
            if(onTutorialFinish != null)
            {
                onTutorialFinish();
            }
        }
    }

    void SwitchPhase()
    {
        GameManager.Instance.ChangePhase();        
    }

    public void FinishedTutorialMove()
    {
        SwitchPhase();
        StartCoroutine(SwitchPhases());
    }

    IEnumerator SwitchPhases()
    {
        yield return new WaitForSeconds(5);

        SwitchPhase();
    }

    public bool CompareTutorial(List<Direction> acts)
    {
        List<Direction> tutorial = actions[currentTutorial].ToList();
        if(tutorial.Count != acts.Count)
        {
            return false;
        }
        for (int i = 0; i < tutorial.Count; i++)
        {
            if (acts[i] == tutorial[i])
            {
                continue;
            }
            else
            {
                return false;
            }
        }

        return true;
    }
}

[System.Serializable]
public class DirectionArray
{
    public Direction[] directions;
}