using System.Collections;
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

    [SyncVar]
    private int playersCount;

    public void AddPlayer(PlayerSetup setup)
    {
        if (playersCount < MaximumPlayers)
        {
            setup.PlayerColor = PlayerColors[playersCount];
            setup.PlayerNumber = ++playersCount;
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

}
