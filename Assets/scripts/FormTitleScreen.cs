using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormTitleScreen : FormBase 
{
  public List<Text> MenuIems;
  public int DefaultFontSize = 24;
  public int FontSizeMax = 34;

  int _itemIndex = 0, _fontSize = 1;

  Color _selectedColor = new Color(1.0f, 0.0f, 1.0f);
  public override void Init()
  {
    SoundManager.Instance.PlayMusicTrack(GlobalConstants.MusicTracks[0]);

    _itemIndex = 0;
    _fontSize = DefaultFontSize;
    MenuIems[_itemIndex].color = _selectedColor;
  }

  public override void SelectMenuItem(int itemIndex)
  {
    if (_itemIndex == itemIndex)
    {
      ChildForms[_itemIndex].Select(this);
    }
    else
    {
      SoundManager.Instance.PlaySound(GlobalConstants.MenuMoveSound);

      _fontSize = DefaultFontSize;
      MenuIems[_itemIndex].fontSize = DefaultFontSize;
      MenuIems[_itemIndex].color = Color.white;
      _itemIndex = itemIndex;
    }
  }

  public override void Process()
  {           
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
