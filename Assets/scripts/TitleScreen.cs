using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour 
{
  public Font MyFont;

  public Transform BackgroundStarsHolder;
  public Transform AsteroidsHolder;

  public GameObject BackgroundStarPrefab;
  public GameObject AsteroidPrefab;

  List<BackgroundStar> _stars = new List<BackgroundStar>();

  float[] _screenRect;
  public float[] ScreenRect
  {
    get { return _screenRect; }
  }

  void Awake()
  {
    // In new version of Unity without this framerate seems to be limited to 30 FPS on Android
    Application.targetFrameRate = 60;

    ScreenshotTaker.Instance.Initialize();
    GameStats.Instance.Initialize();
    LoadingScreen.Instance.Initialize();
    SoundManager.Instance.Initialize();
    //FPSCounter.Instance.Initialize();

    Vector3 dimensions = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f, Camera.main.nearClipPlane));

    //Debug.Log(_dimensions);

    // 90 is pixels per unit in import settings of the star texture

    int starsNumberHorizontal = Screen.width / 180;
    int starsNumberVertical = Screen.height / 180;

    int totalStars = starsNumberHorizontal * starsNumberVertical;

    float[] screenDimensions = new float[4];

    screenDimensions[0] = dimensions.x;
    screenDimensions[1] = dimensions.y;
    screenDimensions[2] = -dimensions.x;
    screenDimensions[3] = -dimensions.y + 1.0f;

    _screenRect = screenDimensions;

    _stars.Clear();

    for (int i = 0; i < totalStars; i++)
    {      
      GameObject go = Instantiate(BackgroundStarPrefab, Vector3.zero, Quaternion.identity, BackgroundStarsHolder);
      BackgroundStar bs = go.GetComponentInChildren<BackgroundStar>();
      bs.Init(screenDimensions);
      _stars.Add(bs);
    }

    int asteroidsH = Screen.width / 180;
    int asteroidsV = Screen.height / 180;

    int totalAsteroids = asteroidsH * asteroidsV;

    for (int i = 0; i < totalAsteroids; i++)
    {
      GameObject go = Instantiate(AsteroidPrefab, Vector3.zero, Quaternion.identity, AsteroidsHolder);
      Asteroid a = go.GetComponent<Asteroid>();
      int breakdownLevel = Random.Range(1, GlobalConstants.AsteroidMaxBreakdownLevel);
      float x = Random.Range(dimensions.x, -dimensions.x);
      float y = Random.Range(dimensions.y, -dimensions.y);
      a.Init(new Vector2(x, y), breakdownLevel, GlobalConstants.GetRandomDir());
    }

    MyFont.material.mainTexture.filterMode = FilterMode.Trilinear;
  }

  void OnEnable()
  {
    SceneManager.sceneLoaded += SceneLoadedHandler;
  }

  void OnDisable()
  {
    SceneManager.sceneLoaded -= SceneLoadedHandler;
  }

  void SceneLoadedHandler(Scene scene, LoadSceneMode mode)
  {
    System.GC.Collect();

    LoadingScreen.Instance.Hide();
  }
}
