using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public delegate void CustomEvent();
public delegate void ChangePhaseEvent(bool state);

public class GameManager : MonoBehaviour
{
    public event CustomEvent onTurn;
    public event ChangePhaseEvent onPhaseChange;

    public static GameManager Instance;

    public bool isMoving;
    public int currentLevel;

    public Animation curtain;
    public GameObject victoryCurtain;

    public bool loadNextLevel = false;

    private void Awake()
    {
        Instance = this;
    }

    public void ChangePhase()
    {
        isMoving = !isMoving;
        if (onPhaseChange != null)
            onPhaseChange(isMoving);
    }

    public void SpendTurns(int turnsAmount)
    {
        for (int i = 0; i < turnsAmount; i++)
        {
            if(onTurn != null)
            {
                onTurn();
            }
        }
    }

    public void GameOver()
    {
        // Reset the level
        Loading();
        // Remove curtains
    }

    public void Victory()
    {
        // Close curtains
        // Check if there are any levels left
        // Load new level
        currentLevel++;
        Loading();
        // Remove curtains
        loadNextLevel = false;
    }

    public void LoadLevel()
    {
        if(loadNextLevel)
        {
            Victory();
        }
        else
        {
            GameOver();
        }
    }

    public void ExitToMainMenu()
    {
        currentLevel = 0;
        ShowCurtain(false);
    }

    public void ShowVictoryCurtain()
    {
        victoryCurtain.SetActive(true);
    }

    public void ShowCurtain(bool loadNext)
    {
        loadNextLevel = loadNext;
        curtain.gameObject.SetActive(true);
        curtain.Play("SceneFadeOut");
    }

    void Loading()
    {
        if (currentLevel < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(currentLevel);
        else
        {
            // No more levels left
        }
    }

    public void SubscribeForTurn(CustomEvent evt)
    {
        onTurn += evt;  
    }

    public void Unsubscribe(CustomEvent evt)
    {
        if(onTurn != null)
            onTurn -= evt;
    }
}
