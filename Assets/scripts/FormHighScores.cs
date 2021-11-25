using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FormHighScores : FormBase
{
  public List<Text> MenuIems;
  public int DefaultFontSize = 24;
  public int FontSizeMax = 34;

  public Text TextObject;

  int _itemIndex = 0, _fontSize = 1;

  public override void Init()
  {
    _itemIndex = 0;
    _fontSize = DefaultFontSize;
  }

  StringBuilder sb = new StringBuilder();
  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    RefreshHighscoreTable();
  }

  void RefreshHighscoreTable()
  {
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

  public override void SelectMenuItem(int itemIndex)
  {
    (ChildForms[0] as FormYesNo).SetActions(() =>
    {
      SoundManager.Instance.PlaySound("cash-register", 1.0f, 1.0f, false);
      GameStats.Instance.ClearHighScores();
      RefreshHighscoreTable();
    }, () =>
    {
      SoundManager.Instance.PlaySound(GlobalConstants.MenuBackSound);
    });

    ChildForms[0].Select(this);
  }

  Color _selectedColor = new Color(1.0f, 0.0f, 1.0f);
  public override void Process()
  {
    if (Input.GetKeyDown(KeyCode.Return))
    {
      (ChildForms[0] as FormYesNo).SetActions(() =>
      {
        SoundManager.Instance.PlaySound("cash-register", 1.0f, 1.0f, false);
        GameStats.Instance.ClearHighScores();
        RefreshHighscoreTable();
      }, () =>
      {
        SoundManager.Instance.PlaySound(GlobalConstants.MenuBackSound);
      });

      ChildForms[0].Select(this);
    }

    MenuIems[_itemIndex].color = _selectedColor;

    AnimateFont();
  }

  bool _sizeGrow = false;
  void AnimateFont()
  {
    if (_sizeGrow)
    {
      _fontSize++;
    }
    else
    {
      _fontSize--;
    }

    if (_fontSize > FontSizeMax)
    {
      _sizeGrow = false;
    }
    else if (_fontSize < DefaultFontSize)
    {
      _sizeGrow = true;
    }

    MenuIems[_itemIndex].fontSize = _fontSize;
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
