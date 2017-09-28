﻿using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoSingleton<SoundManager> 
{
  [Range(0.0f, 1.0f)]
  public float SoundVolume = 1.0f;
  [Range(0.0f, 1.0f)]
  public float MusicVolume = 1.0f;  

  public AudioSource AudioSourceOneShotPrefab;

  [SerializeField]
  public List<AudioClip> MusicTracks = new List<AudioClip>();
  [SerializeField]
  public List<AudioClip> SoundEffects = new List<AudioClip>();

  public Dictionary<string, int> LastPlayedFootstepSoundIndexOfActor = new Dictionary<string, int>();

  Dictionary<string, AudioSource> _audioSourcesByName = new Dictionary<string, AudioSource>();

  protected override void Init()
  {    
    MakeMusicDatabase();
    MakeSoundsDatabase();
  }

  public void RefreshMediaLists()
  {
    // FIXME:
    // It happened several times: for some reason after git pull there is a null in the sound effects list.
    // Some sound (might be music as well, hadn't tested) doesn't load.
    // Call this method during singleton's initialization.

    MakeMusicDatabase();
    MakeSoundsDatabase();
  }

  void MakeMusicDatabase()
  {
    foreach (var item in MusicTracks)
    {
      if (item == null)
      {
        Debug.LogWarning("Music track didn't load (is null) - rebuild media list in Inspector!");
        continue;
      }

      AudioSource s = (AudioSource)Instantiate(AudioSourceOneShotPrefab);
      s.transform.parent = transform;
    
      s.clip = item;
      s.volume = MusicVolume;
      s.name = item.name;
      s.loop = true;

      _audioSourcesByName.Add(s.name, s);
    }
  }

  void MakeSoundsDatabase()
  {
    foreach (var item in SoundEffects)
    {   
      // It happened several times: for some reason after checkout there is a null in the list
      // if you try to run the game immediately (Debug.Log shows that player-thump.ogg didn't load
      // for some reason). Fixed after pressing "Generate Sounds List" in SoundManager Inspector.
      //
      // For safety same condition is applied to the music list.

      if (item == null)
      {
        Debug.LogWarning("Sound effect didn't load (is null) - rebuild media list in Inspector!");
        continue;
      }

      AudioSource s = (AudioSource)Instantiate(AudioSourceOneShotPrefab);
      s.transform.parent = transform;

      s.clip = item;
      s.volume = SoundVolume;
      s.name = item.name;

      _audioSourcesByName.Add(s.name, s);
    }
  }

  public void PlaySound(string name, Vector3 position, bool is3D, float pitch = 1.0f)
  {
    if (_audioSourcesByName.ContainsKey(name))
    {      
      GameObject go = new GameObject("SFX-3D-" + name);
      go.transform.parent = transform;
      go.transform.position = position;
      AudioSource a = go.AddComponent<AudioSource>();
      a.playOnAwake = false;
      a.spatialBlend = is3D ? 1.0f : 0.0f;
      a.volume = is3D ? SoundVolume : 1.0f;
      a.pitch = pitch;
      a.maxDistance = AudioSourceOneShotPrefab.maxDistance;
      a.minDistance = AudioSourceOneShotPrefab.minDistance;
      a.rolloffMode = AudioRolloffMode.Custom;
      var curve = AudioSourceOneShotPrefab.GetCustomCurve(AudioSourceCurveType.CustomRolloff);
      a.SetCustomCurve(AudioSourceCurveType.CustomRolloff, curve);
      a.clip = _audioSourcesByName[name].clip;
      float length = a.clip.length / pitch + 1.0f;
      a.Play();
      Destroy(go, length);
    }
  }

  public void PlaySound(string name, float volume = 1.0f, float pitch = 1.0f, bool instantiate = true)
  {    
    if (_audioSourcesByName.ContainsKey(name))
    {
      if (instantiate)
      {
        GameObject go = new GameObject("SFX-one-shot");
        go.transform.parent = transform;
        AudioSource a = go.AddComponent<AudioSource>();
        a.playOnAwake = false;
        a.volume = volume * SoundVolume;
        a.clip = _audioSourcesByName[name].clip;    
        a.pitch = pitch;
        float length = a.clip.length + 1.0f;
        a.Play();
        Destroy(go, length);    
      }
      else
      {
        _audioSourcesByName[name].volume = volume * SoundVolume;
        _audioSourcesByName[name].Play();
      }
    }
  }

  string _currentPlayingTrack = string.Empty;
  public void PlayMusicTrack(string trackName)
  { 
    if (_audioSourcesByName.ContainsKey(trackName))
    {
      if (_currentPlayingTrack != string.Empty && _audioSourcesByName[_currentPlayingTrack].isPlaying)
      {        
        _audioSourcesByName[_currentPlayingTrack].Stop();
        _audioSourcesByName[_currentPlayingTrack].timeSamples = 0;
      }

      _audioSourcesByName[trackName].Play();
      _currentPlayingTrack = trackName;
    }
  }

  public void StopAllSounds()
  {
    foreach (var item in _audioSourcesByName)
    {
      item.Value.Stop();
    }
  }

  void Update()
  {    
    if (_currentPlayingTrack == string.Empty)
    {
      return;
    }

    if (_audioSourcesByName[_currentPlayingTrack].timeSamples > (int)GlobalConstants.MusicTrackLoopPointsByName[_currentPlayingTrack].y)
    {
      _audioSourcesByName[_currentPlayingTrack].timeSamples = (int)GlobalConstants.MusicTrackLoopPointsByName[_currentPlayingTrack].x;
    }
  }
}
