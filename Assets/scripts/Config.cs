using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class Config
{
  public JSONNode DataAsJson;

  public Config()
  {    
    DataAsJson = new JSONObject();
  }

  public void ReadConfig()
  {
    if (PlayerPrefs.HasKey(GlobalConstants.PlayerPrefsConfigDataKey))
    {
      string config = PlayerPrefs.GetString(GlobalConstants.PlayerPrefsConfigDataKey);
      DataAsJson = JSON.Parse(config);

      Debug.Log("Config loaded:\n" + DataAsJson.ToString(4));

      GameStats.Instance.FillHighscores();
    }
    else
    {
      CreateDefaultConfig();
    }
  }

  void CreateDefaultConfig()
  {
    string entryKey = string.Empty;

    DataAsJson[GlobalConstants.PlayerPrefsPlayerNameKey] = GameStats.Instance.PlayerName;
    DataAsJson[GlobalConstants.PlayerPrefsSoundVolumeKey] = "100";
    DataAsJson[GlobalConstants.PlayerPrefsMusicVolumeKey] = "100";

    for (int i = 0; i < 10; i++)
    {
      HighscoreEntry e = new HighscoreEntry();

      //e.PlayerName = string.Format("Player #{0}", i + 1);
      //e.RandomizeEntry();

      entryKey = string.Format("entry-{0}", i);

      DataAsJson[entryKey] = e.GetJson();
    }

    GameStats.Instance.FillHighscores();
    WriteConfig();
  }

  public void WriteConfig()
  {
    WriteJson();

    Debug.Log("Writing config:\n" + DataAsJson.ToString(4));

    PlayerPrefs.SetString(GlobalConstants.PlayerPrefsConfigDataKey, DataAsJson.ToString());
  }

  void WriteJson()
  {
    DataAsJson[GlobalConstants.PlayerPrefsPlayerNameKey] = GameStats.Instance.PlayerName;
    DataAsJson[GlobalConstants.PlayerPrefsSoundVolumeKey] = SoundManager.Instance.SoundVolumePercent.ToString();
    DataAsJson[GlobalConstants.PlayerPrefsMusicVolumeKey] = SoundManager.Instance.MusicVolumePercent.ToString();

    for (int i = 0; i < 10; i++)
    {      
      string entryKey = string.Format("entry-{0}", i);

      var e = GameStats.Instance.HighscoresSorted[i];

      DataAsJson[entryKey] = e.GetJson();
    }
  }
}

public class HighscoreEntry
{
  public string PlayerName = string.Empty;
  public int Score = -1;
  public int Phase = -1;

  public JSONNode DataAsJson = new JSONObject();

  public string GetJson()
  {
    DataAsJson[GlobalConstants.HighscoreEntryPlayerNameKey] = PlayerName;
    DataAsJson[GlobalConstants.HighscoreEntryPlayerScoreKey] = Score.ToString();
    DataAsJson[GlobalConstants.HighscoreEntryPlayerPhaseKey] = Phase.ToString();

    return DataAsJson.ToString();
  }

  public void RandomizeEntry()
  {    
    Score = Random.Range(1, 1000);
    Phase = Random.Range(1, 1000);
  }

  public override string ToString()
  {
    return string.Format("name=[{0}] score=[{1}] phase=[{2}]", PlayerName, Score, Phase);
  }
}
