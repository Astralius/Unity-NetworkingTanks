using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

/// <summary>
/// Tracks player's name, id, color etc. for networking purposes.
/// </summary>
public class PlayerSetup : NetworkBehaviour
{
    [SyncVar(hook="UpdatePlayerColor")]
    public Color PlayerColor;
    [SyncVar(hook="UpdatePlayerName")]
    public int PlayerNumber;
    public string BaseName = "Player";
    public Text PlayerNameText;


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (PlayerNameText != null)
        {
            PlayerNameText.enabled = false;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        SetupCameraFollow();
        CmdSetupPlayer();
    }

    [Command]
    private void CmdSetupPlayer()
    {
        GameManager.Instance.AddPlayer(this);
    }

    private void SetupCameraFollow()
    {
        var followTarget = Camera.main.GetComponent<FollowTarget>();
        if (followTarget != null)
        {
            followTarget.target = this.transform;
        }
    }

    private void UpdatePlayerColor(Color color)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshes)
        {
            meshRenderer.material.color = color;
        }
    }

    private void UpdatePlayerName(int playerNumber)
    {
        if (PlayerNameText != null)
        {
            PlayerNameText.enabled = true;
            PlayerNameText.text = BaseName + " " + playerNumber;
        }
    }
}
