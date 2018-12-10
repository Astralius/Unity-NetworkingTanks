using UnityEngine.UI;

/// <summary>
/// Represents a single player and his related items.
/// </summary>
public class Player
{
    /// <summary>
    /// Controller for this player.
    /// </summary>
    public PlayerController Controller;

    /// <summary>
    /// Setup data for this player.
    /// </summary>
    public PlayerSetup Setup;

    /// <summary>
    /// UI Label for this player's name.
    /// </summary>
    public Text LabelText;

    /// <summary>
    /// Score text for this player.
    /// </summary>
    public Text ScoreText;
}