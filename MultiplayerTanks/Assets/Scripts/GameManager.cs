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
    public Text MessageText;
    public List<Player> Players = new List<Player>();
    public Text[] LabelTexts = new Text[4];
    public Text[] ScoreTexts = new Text[4];

    [SyncVar]
    private int playersCount;


    public void AddPlayer(PlayerSetup setup)
    {
        if (playersCount < MaximumPlayers)
        {
            setup.PlayerColor = PlayerColors[playersCount];
            setup.PlayerNumber = ++playersCount;

            Players.Add(new Player
            {
                Setup = setup,
                Controller = setup.GetComponent<PlayerController>(),
                LabelText = LabelTexts[setup.PlayerNumber - 1],
                ScoreText = ScoreTexts[setup.PlayerNumber - 1]
            });
        }
    }

    public void UpdateScoreboard()
    {
        if (isServer)
        {
            var names = Players.Select(p => p.Setup.PlayerNameText.text).ToArray();
            var scores = Players.Select(p => p.Controller.Score).ToArray();
            RpcUpdateScoreboard(names, scores);
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
    }

    private IEnumerator EnterLobby()
    {
        if (MessageText != null && !MessageText.IsActive())
        {
            MessageText.gameObject.SetActive(true);
            MessageText.text = "waiting for players";
        }

        while (playersCount < MinimumPlayers)
        {
            DisablePlayers();
            yield return null;
        }
    }

    private IEnumerator PlayGame()
    {
        EnablePlayers();
        UpdateScoreboard();

        if (MessageText != null)
        {
            MessageText.gameObject.SetActive(false);
        }

        yield return null;
    }

    private IEnumerator EndGame()
    {
        yield return null;
    }

    private static void EnablePlayers()
    {
        SetPlayerState(true);
    }

    private static void DisablePlayers()
    {
        SetPlayerState(false);
    }

    private static void SetPlayerState(bool state)
    {
        PlayerController[] allPlayers = GameObject.FindObjectsOfType<PlayerController>();
        foreach (var p in allPlayers)
        {
            p.enabled = state;
        }
    }

    [ClientRpc]
    private void RpcUpdateScoreboard(string[] playerNames, int[] playerScores)
    {
        for (var i = 0; i < playersCount; i++)
        {
            if (playerNames[i] != null)
            {
                LabelTexts[i].text = playerNames[i];
            }
            ScoreTexts[i].text = playerScores[i].ToString();
        }

        for (var j = MaximumPlayers - 1; j >= playersCount; j--)
        {
            LabelTexts[j].text = string.Empty;
            ScoreTexts[j].text = string.Empty;
        }
    }
}
