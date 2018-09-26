using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityStandardAssets.Utility;

/// <summary>
/// Tracks player's name, id, color etc. for networking purposes.
/// </summary>
public class PlayerSetup : NetworkBehaviour
{
    public Color PlayerColor;
    public string BaseName = "Player  ";
    public int PlayerNumber = 1;
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

        SetLocalPlayerColor();
        SetLocalPlayerName();
        SetupFollowTarget();
    }

    private void SetupFollowTarget()
    {
        var followTarget = Camera.main.GetComponent<FollowTarget>();
        if (followTarget != null)
        {
            followTarget.target = this.transform;
        }
    }

    private void SetLocalPlayerColor()
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();
        foreach (var meshRenderer in meshes)
        {
            meshRenderer.material.color = PlayerColor;
        }
    }

    private void SetLocalPlayerName()
    {
        if (PlayerNameText != null)
        {
            PlayerNameText.enabled = true;
            PlayerNameText.text = BaseName + PlayerNumber;
        }
    }
}
