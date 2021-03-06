﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class GameStats : MonoSingleton<GameStats> 
{
  [HideInInspector]
  public string PlayerName = "Player";

  public bool DeletePlayerPrefs = false;

  public Config GameConfig = new Config();

  public override void Initialize()
  {
    if (DeletePlayerPrefs)
    {
      PlayerPrefs.DeleteAll();
    }

    GameConfig.ReadConfig();
    PlayerName = GameConfig.DataAsJson[GlobalConstants.PlayerPrefsPlayerNameKey];
  }

  public void ClearHighScores()
  {
    string entryKey = string.Empty;
    for (int i = 0; i < 10; i++)
    {
      HighscoreEntry e = new HighscoreEntry();

      //e.PlayerName = string.Format("Player #{0}", i + 1);
      //e.RandomizeEntry();

      entryKey = string.Format("entry-{0}", i);

      GameConfig.DataAsJson[entryKey] = e.GetJson();
    }

    FillHighscores();
    GameConfig.WriteConfig();
  }

  public void WriteHighScore(int score, int phase)
  {
    int index = -1;
    for (int i = 0; i < _highScoresSorted.Count; i++)
    {
      if (score > _highScoresSorted[i].Score)
      {
        index = i;
        break;
      }
    }

    if (index != -1)
    {
      HighscoreEntry e = new HighscoreEntry();

      e.PlayerName = PlayerName;
      e.Score = score;
      e.Phase = phase;

      _highScoresSorted.Insert(index, e);

      _highScoresSorted.RemoveAt(_highScoresSorted.Count - 1);
    }

    GameConfig.WriteConfig();
  }

  List<HighscoreEntry> _highScoresSorted = new List<HighscoreEntry>();
  public List<HighscoreEntry> HighscoresSorted
  {
    get { return _highScoresSorted; }
  }

  public void FillHighscores()
  {
    _highScoresSorted.Clear();

    for (int i = 0; i < 10; i++)
    {
      _highScoresSorted.Add(GetEntry(i));
    }

    _highScoresSorted.Sort((s1, s2) => s1.Score.CompareTo(s2.Score));

    _highScoresSorted.Reverse();

    /*
    foreach (var item in _highScoresSorted)
    {
      Debug.Log(item.ToString());
    }
    */
  }

  public HighscoreEntry GetEntry(int number)
  {
    string key = string.Format("entry-{0}", number);

    HighscoreEntry e = new HighscoreEntry();

    JSONNode n = JSON.Parse(GameConfig.DataAsJson[key]);

    e.PlayerName = n[GlobalConstants.HighscoreEntryPlayerNameKey];
    e.Score = (int)n[GlobalConstants.HighscoreEntryPlayerScoreKey];
    e.Phase = (int)n[GlobalConstants.HighscoreEntryPlayerPhaseKey];

    return e;
  }
}
