using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicTest : MonoBehaviour 
{
  public Text TrackName;
  public InputField SamplesStart;
  public InputField SamplesEnd;

  int _trackIndex = 0;
  string _trackName = string.Empty;
  void Awake()
  {
    SoundManager.Instance.Initialize();

    _trackName = GlobalConstants.MusicTracks[_trackIndex];

    SamplesStart.text = GlobalConstants.MusicTrackLoopPointsByName[_trackName].X.ToString();
    SamplesEnd.text = GlobalConstants.MusicTrackLoopPointsByName[_trackName].Y.ToString();

    TrackName.text = _trackName;

    SoundManager.Instance.PlayMusicTrack(_trackName);

    SoundManager.Instance.SetMusicTrackVolume(1.0f);
  }

  public void PreviousHandler()
  {
    _trackIndex--;

    if (_trackIndex < 0)
    {
      _trackIndex = GlobalConstants.MusicTracks.Count - 1;
    }

    _trackName = GlobalConstants.MusicTracks[_trackIndex];
    SamplesStart.text = GlobalConstants.MusicTrackLoopPointsByName[_trackName].X.ToString();
    SamplesEnd.text = GlobalConstants.MusicTrackLoopPointsByName[_trackName].Y.ToString();

    TrackName.text = _trackName;
  }

  public void NextHandler()
  {
    _trackIndex++;

    if (_trackIndex > GlobalConstants.MusicTracks.Count - 1)
    {
      _trackIndex = 0;
    }

    _trackName = GlobalConstants.MusicTracks[_trackIndex];
    SamplesStart.text = GlobalConstants.MusicTrackLoopPointsByName[_trackName].X.ToString();
    SamplesEnd.text = GlobalConstants.MusicTrackLoopPointsByName[_trackName].Y.ToString();

    TrackName.text = _trackName;
  }

  int _endLoop = 0;
  public void EditEndHandler()
  {
    _endLoop = int.Parse(SamplesEnd.text);

    GlobalConstants.MusicTrackLoopPointsByName[_trackName].Y = _endLoop;
  }

  public void PlayHandler()
  {
    SoundManager.Instance.PlayMusicTrack(_trackName);

    int playFrom = GlobalConstants.MusicTrackLoopPointsByName[_trackName].Y - 100000;
    SoundManager.Instance.CurrentMusicTrack.timeSamples = playFrom;
  }
}
