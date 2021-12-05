using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class FormHighScores : FormBase
{
  public List<Text> MenuIems;
  public int DefaultFontSize = 24;
  public int FontSizeMax = 34;

  public List<HighScoreItem> HighScoreItems;

  int _itemIndex = 0, _fontSize = 1;

  public override void Init()
  {
    _itemIndex = 0;
    _fontSize = DefaultFontSize;

    foreach (var item in HighScoreItems)
    {
      item.ResetValues();
    }
  }

  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    RefreshHighscoreTable();
  }

  void RefreshHighscoreTable()
  {
    for (int i = 0; i < GlobalConstants.MaxHighScoreEntries; i++)
    {
      HighscoreEntry e = GameStats.Instance.HighscoresSorted[i];
      HighScoreItems[i].SetValues(e);
    }
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
}
