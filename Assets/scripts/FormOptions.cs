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

    int format = Mathf.RoundToInt(SoundManager.Instance.MusicVolume * 100);
    MenuIems[0].text = string.Format("MUSIC: {0}", format);

    format = Mathf.RoundToInt(SoundManager.Instance.SoundVolume * 100);
    MenuIems[1].text = string.Format("SOUND: {0}", format);
  }

  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    _itemIndex = 0;
  }

  public override void Close()
  {
    MenuIems[_itemIndex].fontSize = DefaultFontSize;
    MenuIems[_itemIndex].color = Color.white;

    base.Close();
  }

  int _musicIndex = 0;
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
      if (_itemIndex == 2)
      {
        SoundManager.Instance.PlayMusicTrack(GlobalConstants.MusicTracks[_musicIndex]);
      }
    }

    HandleMenuItem();

    _itemIndex = Mathf.Clamp(_itemIndex, 0, MenuIems.Count - 1);

    MenuIems[_itemIndex].color = _selectedColor;

    AnimateFont();
  }

  void HandleMenuItem()
  {
    switch (_itemIndex)
    {
      case 0:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
          SoundManager.Instance.MusicVolume -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          SoundManager.Instance.MusicVolume += 0.1f;
        }

        SoundManager.Instance.MusicVolume = Mathf.Clamp(SoundManager.Instance.MusicVolume, 0.0f, 1.0f);

        int format = Mathf.RoundToInt(SoundManager.Instance.MusicVolume * 100);

        SoundManager.Instance.CurrentMusicTrack.volume = SoundManager.Instance.MusicVolume;

        MenuIems[_itemIndex].text = string.Format("MUSIC: {0}", format);
        break;

      case 1:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
          SoundManager.Instance.SoundVolume -= 0.1f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          SoundManager.Instance.SoundVolume += 0.1f;
        }

        SoundManager.Instance.SoundVolume = Mathf.Clamp(SoundManager.Instance.SoundVolume, 0.0f, 1.0f);

        format = Mathf.RoundToInt(SoundManager.Instance.SoundVolume * 100);

        MenuIems[_itemIndex].text = string.Format("SOUND: {0}", format);
        break;
      
      case 2:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
          _musicIndex--;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          _musicIndex++;
        }

        if (_musicIndex < 0)
        {
          _musicIndex = GlobalConstants.MusicTracks.Count - 1;
        }
        else if (_musicIndex > GlobalConstants.MusicTracks.Count - 1)
        {
          _musicIndex = 0;
        }

        MenuIems[_itemIndex].text = string.Format("MUSIC TEST: {0}", _musicIndex);
        break;
    }
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

    _fontSize = Mathf.Clamp(_fontSize, DefaultFontSize, FontSizeMax);

    if (_fontSize == FontSizeMax)
    {
      _sizeGrow = false;
    }
    else if (_fontSize == DefaultFontSize)
    {
      _sizeGrow = true;
    }

    MenuIems[_itemIndex].fontSize = _fontSize;
  }
}
