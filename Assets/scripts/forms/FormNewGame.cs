using UnityEngine.SceneManagement;

public class FormNewGame : FormBase
{
  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    LoadingScreen.Instance.Show();

    SceneManager.LoadScene("main");
  }
}
