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

  public override void Process()
  {           
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      if (_itemIndex == MenuIems.Count - 1) return;

      SoundManager.Instance.PlaySound(GlobalConstants.MenuMoveSound);

      _fontSize = DefaultFontSize;
      MenuIems[_itemIndex].fontSize = DefaultFontSize;
      MenuIems[_itemIndex].color = Color.white;
      _itemIndex++;
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      if (_itemIndex == 0) return;

      SoundManager.Instance.PlaySound(GlobalConstants.MenuMoveSound);

      _fontSize = DefaultFontSize;
      MenuIems[_itemIndex].fontSize = DefaultFontSize;
      MenuIems[_itemIndex].color = Color.white;
      _itemIndex--;
    }
    else if (Input.GetKeyDown(KeyCode.Return))
    {      
      ChildForms[_itemIndex].Select(this);
    }

    _itemIndex = Mathf.Clamp(_itemIndex, 0, MenuIems.Count - 1);

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
