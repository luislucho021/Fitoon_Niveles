using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private struct Runner
    {
        public int id;
        public Character character;
        public bool isPlayer;
    }
	[SerializeField] Transform[] spawnPoints;
    [SerializeField] BotRunner botPrefab;
    [SerializeField] int[] numberOfWinners;
    [SerializeField] Countdown countdown;

    static List<Runner> runnerData;
    static List<Runner> winners;
    static List<BaseRunner> runners;
	public static int numberOfRunners;
	public static int numberOfPlayers;
    static int currentRace = 0;
    static int[] s_numberOfWinners;

    static bool classified = false;

    static int playerIndex = 0;

    void Awake()
    {
        if(currentRace != 0)
        {
            playerIndex = 0;
            SpawnBots();
            return;
        }

        ResetStaticValues();
		s_numberOfWinners = numberOfWinners;
		InitializeBots();
        SpawnBots();
    }

	void Start()
	{
        countdown.StartCountdown();
        StartCoroutine(WaitForCountdown());
	}

    void UnfreezeAllRunners()
    {
        for(int i = 0; i < runners.Count; i++)
        {
            runners[i].UnFreeze();
        }
    }

	void SpawnBots()
    {
		for (int i = 0; i < runnerData.Count; i++)
		{
            if (runnerData[i].isPlayer)
            {
                continue;
            }

			BotRunner runner = Instantiate(botPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            runner.SetId(runnerData[i].id);
            runner.LoadCharacter(runnerData[i].character);
            runners.Add(runner);
		}
	}

	public static void ResetStaticValues()
	{
        runners = new List<BaseRunner>();
		runnerData = new List<Runner>();
		winners = new List<Runner>();
		currentRace = 0;
		classified = false;
        playerIndex = 0;
	}

    void InitializeBots()
    {
        for (int i = numberOfPlayers; i < numberOfRunners; i++)
        {
            runnerData.Add
            (
                new Runner
                {
                    id = i,
                    character = CharacterLoader.CreateRandomCharacter(),
                    isPlayer = false,
                }
            );
        }
	}

	public static void GoalReached(BaseRunner baseRunner)
    {
		if (runnerData.Count == 0)
		{
			return;
		}

		if (baseRunner.GetComponent<PlayerController>() != null)
        {
            classified = true;
        }

		Runner runner = new Runner();

		try
        {
			runner = runnerData.Find((r) => r.id == baseRunner.GetId());
        }
        catch(ArgumentNullException)
        {
            Debug.LogError("Runner not found");
            return;
        }

        winners.Add(runner);
        if(winners.Count >= s_numberOfWinners[currentRace])
        {
            runnerData = winners;
            winners = new List<Runner>();
            runners = new List<BaseRunner>();
			currentRace++;

            if(!classified)
            {
                AssessResult();
                return;
            }

            if (currentRace < s_numberOfWinners.Length)
            {
                SceneManager.LoadScene("Classified");//Esto habrá que cambiarlo para networking
            }
            else
            {
                AssessResult();
            }
        }
    }

    public static void AssessResult()//Esto habrá que cambiarlo para networking
    {
        if(runnerData.Count == 0)
        {
            return;
        }

        if(currentRace == s_numberOfWinners.Length)
        {
            if (classified)
            {
                currentRace = 0;
                SceneManager.LoadScene("YouWin");
                return;
            }
        }
        currentRace = 0;
        SceneManager.LoadScene("YouLose");
    }

    public static void AddPlayer(PlayerController player)//Probablemente habrá que cambiarlo para networking
    {
        runners.Add(player);
		if (runnerData.Count == 0)
		{
			return;
		}

		player.SetId(playerIndex);
        runnerData.Add(
            new Runner()
            {
                id = playerIndex++,
                isPlayer = true
            });
    }

    public static void RemoveRunner(BaseRunner runner)
    {
        runnerData.RemoveAt(runnerData.FindIndex((r) => r.id == runner.GetId()));
        runners.RemoveAt(runners.FindIndex((r) => r.GetId() == runner.GetId()));
        try
        {
            winners.RemoveAt(winners.FindIndex((r) => r.id == runner.GetId()));
		}
        catch(Exception) { }
    }

    public IEnumerator WaitForCountdown()
    {
        yield return new WaitUntil(() => countdown.HasFinished());
        UnfreezeAllRunners();
    }
}
