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
    foreach (var item in MenuIems)
    {
      item.color = Color.white;
    }

    _itemIndex = 0;
    _fontSize = DefaultFontSize;
    MenuIems[_itemIndex].color = _selectedColor;

    MenuIems[1].text = string.Format("MUSIC: {0}", SoundManager.Instance.MusicVolumePercent);
    MenuIems[2].text = string.Format("SOUND: {0}", SoundManager.Instance.SoundVolumePercent);
  }

  public void LeftArrowHandler(int itemIndex)
  {
    switch (itemIndex)
    {
      case 1:
        SoundManager.Instance.MusicVolumePercent -= _musicSoundDelta;
        SoundManager.Instance.MusicVolume = SoundManager.Instance.MusicVolumePercent * 0.01f;

        SoundManager.Instance.MusicVolumePercent = Mathf.Clamp(SoundManager.Instance.MusicVolumePercent, 0, 100);
        SoundManager.Instance.MusicVolume = Mathf.Clamp(SoundManager.Instance.MusicVolume, 0.0f, 1.0f);

        SoundManager.Instance.CurrentMusicTrack.volume = SoundManager.Instance.MusicVolume;
        break;

      case 2:
        SoundManager.Instance.SoundVolumePercent -= _musicSoundDelta;
        SoundManager.Instance.SoundVolume = SoundManager.Instance.SoundVolumePercent * 0.01f;

        SoundManager.Instance.SoundVolumePercent = Mathf.Clamp(SoundManager.Instance.SoundVolumePercent, 0, 100);
        SoundManager.Instance.SoundVolume = Mathf.Clamp(SoundManager.Instance.SoundVolume, 0.0f, 1.0f);
        break;

      case 3:
        _musicIndex--;

        if (_musicIndex < 0)
        {
          _musicIndex = GlobalConstants.MusicTracks.Count - 1;
        }
        else if (_musicIndex > GlobalConstants.MusicTracks.Count - 1)
        {
          _musicIndex = 0;
        }

        MenuIems[3].text = string.Format("MUSIC TEST: {0}", _musicIndex);
        break;
    }
  }

  public void RightArrowHandler(int itemIndex)
  {
    switch (itemIndex)
    {
      case 1:
        SoundManager.Instance.MusicVolumePercent += _musicSoundDelta;
        SoundManager.Instance.MusicVolume = SoundManager.Instance.MusicVolumePercent * 0.01f;

        SoundManager.Instance.MusicVolumePercent = Mathf.Clamp(SoundManager.Instance.MusicVolumePercent, 0, 100);
        SoundManager.Instance.MusicVolume = Mathf.Clamp(SoundManager.Instance.MusicVolume, 0.0f, 1.0f);

        SoundManager.Instance.CurrentMusicTrack.volume = SoundManager.Instance.MusicVolume;
        break;

      case 2:
        SoundManager.Instance.SoundVolumePercent += _musicSoundDelta;
        SoundManager.Instance.SoundVolume = SoundManager.Instance.SoundVolumePercent * 0.01f;

        SoundManager.Instance.SoundVolumePercent = Mathf.Clamp(SoundManager.Instance.SoundVolumePercent, 0, 100);
        SoundManager.Instance.SoundVolume = Mathf.Clamp(SoundManager.Instance.SoundVolume, 0.0f, 1.0f);
        break;

      case 3:
        _musicIndex++;

        if (_musicIndex < 0)
        {
          _musicIndex = GlobalConstants.MusicTracks.Count - 1;
        }
        else if (_musicIndex > GlobalConstants.MusicTracks.Count - 1)
        {
          _musicIndex = 0;
        }

        MenuIems[3].text = string.Format("MUSIC TEST: {0}", _musicIndex);
        break;
    }
  }

  public override void SelectMenuItem(int itemIndex)
  {
    if (_itemIndex == itemIndex)
    {
      if (_itemIndex == 3)
      {
        SoundManager.Instance.PlayMusicTrack(GlobalConstants.MusicTracks[_musicIndex].Key, true);
      }
      else if (_itemIndex == 0)
      {
        ChildForms[0].Select(this);
      }
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

  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    _itemIndex = 0;
  }

  public override void Close(bool dontPlaySound = false)
  {
    GameStats.Instance.GameConfig.WriteConfig();

    MenuIems[_itemIndex].fontSize = DefaultFontSize;
    MenuIems[_itemIndex].color = Color.white;

    base.Close(dontPlaySound);
  }

  void ResetItemText()
  {
    SoundManager.Instance.PlaySound(GlobalConstants.MenuMoveSound);

    _fontSize = DefaultFontSize;
    MenuIems[_itemIndex].fontSize = DefaultFontSize;
    MenuIems[_itemIndex].color = Color.white;
  }

  void ProcessKeyboard()
  {
    if (Input.GetKeyDown(KeyCode.DownArrow) && _itemIndex != MenuIems.Count - 1)
    {
      ResetItemText();

      _itemIndex++;
      _itemIndex = Mathf.Clamp(_itemIndex, 0, MenuIems.Count - 1);
    }
    else if (Input.GetKeyDown(KeyCode.UpArrow) && _itemIndex != 0)
    {
      ResetItemText();

      _itemIndex--;
      _itemIndex = Mathf.Clamp(_itemIndex, 0, MenuIems.Count - 1);
    }
    else if (Input.GetKeyDown(KeyCode.Return))
    {
      SelectMenuItem(_itemIndex);
    }
  }

  int _musicIndex = 0;
  public override void Process()
  {
    ProcessKeyboard();
    HandleMenuItem();

    MenuIems[1].text = string.Format("MUSIC: {0}", SoundManager.Instance.MusicVolumePercent);
    MenuIems[2].text = string.Format("SOUND: {0}", SoundManager.Instance.SoundVolumePercent);

    MenuIems[_itemIndex].color = _selectedColor;

    AnimateFont();
  }

  int _musicSoundDelta = 5;
  void HandleMenuItem()
  {
    switch (_itemIndex)
    {
      case 1:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
          SoundManager.Instance.MusicVolumePercent -= _musicSoundDelta;

          SoundManager.Instance.MusicVolume = SoundManager.Instance.MusicVolumePercent * 0.01f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          SoundManager.Instance.MusicVolumePercent += _musicSoundDelta;

          SoundManager.Instance.MusicVolume = SoundManager.Instance.MusicVolumePercent * 0.01f;
        }

        SoundManager.Instance.MusicVolumePercent = Mathf.Clamp(SoundManager.Instance.MusicVolumePercent, 0, 100);

        SoundManager.Instance.MusicVolume = Mathf.Clamp(SoundManager.Instance.MusicVolume, 0.0f, 1.0f);
        SoundManager.Instance.CurrentMusicTrack.volume = SoundManager.Instance.MusicVolume;

        MenuIems[_itemIndex].text = string.Format("MUSIC: {0}", SoundManager.Instance.MusicVolumePercent);
        break;

      case 2:
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
          SoundManager.Instance.SoundVolumePercent -= _musicSoundDelta;

          SoundManager.Instance.SoundVolume = SoundManager.Instance.SoundVolumePercent * 0.01f;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
          SoundManager.Instance.SoundVolumePercent += _musicSoundDelta;

          SoundManager.Instance.SoundVolume = SoundManager.Instance.SoundVolumePercent * 0.01f;
        }

        SoundManager.Instance.SoundVolumePercent = Mathf.Clamp(SoundManager.Instance.SoundVolumePercent, 0, 100);

        SoundManager.Instance.SoundVolume = Mathf.Clamp(SoundManager.Instance.SoundVolume, 0.0f, 1.0f);
        MenuIems[_itemIndex].text = string.Format("SOUND: {0}", SoundManager.Instance.SoundVolumePercent);
        break;

      case 3:
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
