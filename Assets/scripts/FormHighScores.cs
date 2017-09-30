using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FormHighScores : FormBase 
{
  public Text TextObject;

  StringBuilder sb = new StringBuilder();
	public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    sb.Length = 0;

    int index = 0;
    foreach (var item in GameStats.Instance.HighscoresSorted)
    {       
      string sindex = string.Empty;
      string sname = string.Empty;
      string sscore = string.Empty;
      string sphase = string.Empty;

      if (item.Score == -1)
      {
        sindex = GetProperString((index + 1).ToString(), 2);
        sname = GetProperString("-----", 20);
        sscore = GetProperString("-----", 5);
        sphase = "-----";
      }
      else
      {
        sindex = GetProperString((index + 1).ToString(), 2);
        sname = GetProperString(item.PlayerName, 20);
        sscore = GetProperString(item.Score.ToString(), 5);
        sphase = string.Format("PHASE: {0}", item.Phase);
      }

      sb.AppendFormat("{0}    {1}    {2}    {3}", sindex, sname, sscore, sphase);

      // No newline for last entry
      if (index != 9)
      {
        sb.Append('\n');
      }

      index++;
    }

    TextObject.text = sb.ToString();
  }

  string GetProperString(string initial, int maxChars)
  {
    string format = "{0}";

    int spacesToFill = maxChars - initial.Length;
    if (spacesToFill != 0)
    {
      string spaces = new string(' ', spacesToFill);
      format = "{0}" + spaces;
    }

    return string.Format(format, initial);
  }
}
