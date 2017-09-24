using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FormOptions : FormBase 
{
  public List<Text> MenuIems;
  public int DefaultFontSize = 24;
  public int FontSizeMax = 34;

  int _itemIndex = 0, _fontSize = 1;

  Color _selectedColor = new Color(1.0f, 0.0f, 1.0f);
  public override void Init()
  {
    _itemIndex = 0;
    _fontSize = DefaultFontSize;
    MenuIems[_itemIndex].color = _selectedColor;
  }

  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    _itemIndex = 0;
  }

  public override void Close()
  {
    MenuIems[_itemIndex].color = Color.white;

    base.Close();
  }

  public override void Process()
  {           
    if (Input.GetKeyDown(KeyCode.DownArrow))
    {
      SoundManager.Instance.PlaySound(GlobalConstants.MenuMoveSound);

      MenuIems[_itemIndex].fontSize = DefaultFontSize;
      MenuIems[_itemIndex].color = Color.white;
      _itemIndex++;
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow))
    {
      SoundManager.Instance.PlaySound(GlobalConstants.MenuMoveSound);

      MenuIems[_itemIndex].fontSize = DefaultFontSize;
      MenuIems[_itemIndex].color = Color.white;
      _itemIndex--;
    }
    else if (Input.GetKeyDown(KeyCode.Return))
    {
      Debug.Log("Selected index " + _itemIndex);
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
