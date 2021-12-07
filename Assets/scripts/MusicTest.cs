using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicTest : MonoBehaviour
{
  public Text TrackName;
  public InputField SamplesStart;
  public InputField SamplesEnd;

  // city-lights 0 000 000 - 2 560 300
  int _loopStartSamples = 2560300;

  public AudioSource MusicTrack;

  /*
  private void Start()
  {
    GameObject loopSegment = new GameObject("Loop Fragment");
    AudioSource loop = loopSegment.AddComponent<AudioSource>();
    loop.playOnAwake = false;
    float[] samples = new float[(MusicTrack.clip.samples - _loopStartSamples) * MusicTrack.clip.channels];
    MusicTrack.clip.GetData(samples, _loopStartSamples);
    loop.clip = AudioClip.Create(MusicTrack.clip.name + "-loop", MusicTrack.clip.samples - _loopStartSamples, MusicTrack.clip.channels, MusicTrack.clip.frequency, false);
    loop.clip.SetData(samples, 0);
    loop.loop = true;
    loop.PlayScheduled(AudioSettings.dspTime + MusicTrack.clip.length);
    MusicTrack.Play();
  }
  */

  int _trackIndex = 0;
  string _trackKey = string.Empty;
  void Awake()
  {
    SoundManager.Instance.Initialize();

    _trackKey = GlobalConstants.MusicTracks[_trackIndex].Key;

    SamplesStart.text = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].X.ToString();
    SamplesEnd.text = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].Y.ToString();

    TrackName.text = _trackKey;

    SoundManager.Instance.PlayMusicTrack(_trackKey);

    SoundManager.Instance.SetMusicTrackVolume(1.0f);
  }

  public void PreviousHandler()
  {
    _trackIndex--;

    if (_trackIndex < 0)
    {
      _trackIndex = GlobalConstants.MusicTracks.Count - 1;
    }

    _trackKey = GlobalConstants.MusicTracks[_trackIndex].Key;
    SamplesStart.text = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].X.ToString();
    SamplesEnd.text = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].Y.ToString();

    TrackName.text = _trackKey;
  }

  public void NextHandler()
  {
    _trackIndex++;

    if (_trackIndex > GlobalConstants.MusicTracks.Count - 1)
    {
      _trackIndex = 0;
    }

    _trackKey = GlobalConstants.MusicTracks[_trackIndex].Key;
    SamplesStart.text = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].X.ToString();
    SamplesEnd.text = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].Y.ToString();

    TrackName.text = _trackKey;
  }

  int _endLoop = 0;
  public void EditEndHandler()
  {
    _endLoop = int.Parse(SamplesEnd.text);

    GlobalConstants.MusicTrackLoopPointsByName[_trackKey].Y = _endLoop;
  }

  public void PlayHandler()
  {
    SoundManager.Instance.PlayMusicTrack(_trackKey);

    int playFrom = GlobalConstants.MusicTrackLoopPointsByName[_trackKey].Y - 100000;
    SoundManager.Instance.CurrentMusicTrack.timeSamples = playFrom;
  }
}
