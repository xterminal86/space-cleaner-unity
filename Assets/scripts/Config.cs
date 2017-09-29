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

      Debug.Log("Config loaded:\n" + DataAsJson.ToString());
    }
    else
    {
      CreateDefaultConfig();
    }
  }

  void CreateDefaultConfig()
  {
    string entryKey = string.Empty;

    for (int i = 0; i < 9; i++)
    {
      HighscoreEntry e = new HighscoreEntry();

      entryKey = string.Format("entry-{0}", i);

      DataAsJson[entryKey] = e.GetJson();
    }

    WriteConfig();
  }

  public void WriteConfig()
  {
    Debug.Log("Writing config:\n" + DataAsJson.ToString());

    PlayerPrefs.SetString(GlobalConstants.PlayerPrefsConfigDataKey, DataAsJson.ToString());
  }
}

public class HighscoreEntry
{
  public string PlayerName = string.Empty;
  public int Score = 0;
  public int Phase = 0;

  public JSONNode DataAsJson = new JSONObject();

  public string GetJson()
  {
    DataAsJson[GlobalConstants.HighscoreEntryPlayerNameKey] = PlayerName;
    DataAsJson[GlobalConstants.HighscoreEntryPlayerScoreKey] = Score.ToString();
    DataAsJson[GlobalConstants.HighscoreEntryPlayerPhaseKey] = Phase.ToString();

    return DataAsJson.ToString();
  }
}
