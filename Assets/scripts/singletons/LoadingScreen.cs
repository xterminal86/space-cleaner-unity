public class LoadingScreen : MonoSingleton<LoadingScreen>
{
  public void Show()
  {
    gameObject.SetActive(true);
  }

  public void Hide()
  {
    gameObject.SetActive(false);
  }
}
