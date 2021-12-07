using UnityEngine;
using UnityEngine.UI;

public class HighScoreItem : MonoBehaviour
{
  public Text NoText;
  public Text NameText;
  public Text ScoreText;
  public Text UfoLameCountText;
  public Text UfoEMPCountText;
  public Text UfoEliteCountText;
  public Text PhaseCountText;

  string _pad20 = new string('-', 20);
  string _pad10 = new string('-', 10);
  string _noKills = "x0";

  public void ResetValues()
  {
    NameText.text          = _pad20;
    ScoreText.text         = _pad10;
    PhaseCountText.text    = _pad10;
    UfoLameCountText.text  = _noKills;
    UfoEMPCountText.text   = _noKills;
    UfoEliteCountText.text = _noKills;
  }

  public void SetValues(HighscoreEntry e)
  {
    NameText.text          = string.IsNullOrEmpty(e.PlayerName) ? _pad20 : e.PlayerName.PadRight(20, '.');
    ScoreText.text         = (e.Score == -1) ? _pad10 : e.Score.ToString().PadRight(10, '.');
    PhaseCountText.text    = (e.Phase == -1) ? _pad10 : e.Phase.ToString().PadRight(10, '.');
    UfoLameCountText.text  = (e.UfoLameCount == -1)  ? _noKills : string.Format("x{0}", e.UfoLameCount);
    UfoEMPCountText.text   = (e.UfoEmpCount == -1)   ? _noKills : string.Format("x{0}", e.UfoEmpCount);
    UfoEliteCountText.text = (e.UfoEliteCount == -1) ? _noKills : string.Format("x{0}", e.UfoEliteCount);
  }
}
