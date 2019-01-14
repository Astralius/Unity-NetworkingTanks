using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : NetworkBehaviour
{
    #region Singleton
    
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            return instance ?? 
                   (instance = FindObjectOfType<GameManager>()) ?? 
                   (instance = new GameObject().AddComponent<GameManager>());
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    #endregion

    public Color[] PlayerColors = {Color.red, Color.blue, Color.green, Color.magenta};
    public int MinimumPlayers = 2;
    public int MaximumPlayers = 4;
    public int CountdownSeconds = 3;
    public int MaximumScore = 3;
    public Text MessageText;
    public List<Player> Players = new List<Player>();
    public Text[] LabelTexts = new Text[4];
    public Text[] ScoreTexts = new Text[4];

    [SyncVar]
    private int playersCount;
    [SyncVar]
    private bool gameOver = false;
    private Player winner;


    public void AddPlayer(PlayerSetup setup)
    {
        if (isServer && playersCount < MaximumPlayers)
        {
            setup.PlayerColor = PlayerColors[playersCount];
            setup.PlayerNumber = ++playersCount;

            var newPlayer = new Player
            {
                Setup = setup,
                Controller = setup.GetComponent<PlayerController>(),
                LabelText = LabelTexts[setup.PlayerNumber - 1],
                ScoreText = ScoreTexts[setup.PlayerNumber - 1]
            };

            Players.Add(newPlayer);
        }
    }

    public void UpdateScoreboard()
    {
        if (isServer)
        {
            winner = GetWinner();
            gameOver = winner != null;

            var names = Players.Select(p => p.Setup.PlayerNameText.text).ToArray();
            var scores = Players.Select(p => p.Controller.Score).ToArray();
            RpcUpdateScoreboard(names, scores);
        }
    }

    public void UpdateMessage(string message)
    {
        if (isServer)
        {
            RpcUpdateMessage(message);
        }
    }

    private void Start()
    {
        StartCoroutine(GameLoopRoutine());
    }

    private IEnumerator GameLoopRoutine()
    {
        yield return StartCoroutine(EnterLobby());
        yield return StartCoroutine(PlayGame());
        yield return StartCoroutine(EndGame());
        yield return StartCoroutine(RestartGame());
    }

    private IEnumerator EnterLobby()
    {
        do
        {
            UpdateMessage("Waiting for players..");
            yield return new WaitForSeconds(Time.deltaTime);
        } while (playersCount < MinimumPlayers);
    }

    private IEnumerator PlayGame()
    {
        StartCoroutine(Countdown());               
        UpdateScoreboard();

        while (!gameOver)
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    private IEnumerator Countdown()
    {
        for (var i = CountdownSeconds; i >= 0; i--)
        {
            yield return new WaitForSeconds(1f);
            UpdateMessage(i > 0 ? i.ToString() : "Fight!");        
        }

        yield return new WaitForSeconds(2f);
        UpdateMessage(string.Empty);
        EnablePlayers();
    }

    private IEnumerator EndGame()
    {
        DisablePlayers();
        UpdateMessage("GAME OVER \n " + winner.Setup.PlayerNameText.text + " wins!");
        yield return new WaitForSeconds(3f);
    }

    private IEnumerator RestartGame()
    {
        UpdateMessage("Restarting...");
        Reset();
        yield return new WaitForSeconds(3f);     
        StartCoroutine(GameLoopRoutine());
    }

    [ClientRpc]
    private void RpcSetPlayersState(bool state)
    {   
        FindObjectsOfType<PlayerController>()
           .ToList()
           .ForEach(controller => controller.enabled = state);
    }

    [ClientRpc]
    private void RpcUpdateScoreboard(string[] playerNames, int[] playerScores)
    {
        for (var i = 0; i < playersCount; i++)
        {
            if (!string.IsNullOrEmpty(playerNames[i]))
            {
                LabelTexts[i].text = playerNames[i];
            }
            ScoreTexts[i].text = playerScores[i].ToString();
        }

        for (var j = playersCount; j < MaximumPlayers; j++)
        {
            LabelTexts[j].text = string.Empty;
            ScoreTexts[j].text = string.Empty;
        }
    }

    [ClientRpc]
    private void RpcUpdateMessage(string message)
    {
        if (MessageText != null && message != null)
        {
            MessageText.gameObject.SetActive(true);
            MessageText.text = message;
        }

    }

    [ClientRpc]
    private void RpcReset()
    {
        Players.ForEach(p => p.Controller.Score = 0);
    }

    private void EnablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayersState(true);
        }
    }

    private void DisablePlayers()
    {
        if (isServer)
        {
            RpcSetPlayersState(false);
        }
    }

    private Player GetWinner()
    {
        return isServer
            ? Players.FirstOrDefault(player => player.Controller.Score >= MaximumScore)
            : null;
    }
    
    private void Reset()
    {
        if (isServer)
        {
            RpcReset();
            UpdateScoreboard();
            winner = null;
            gameOver = false;
        }
    } 
}
