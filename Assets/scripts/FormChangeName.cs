using UnityEngine.UI;

public class FormChangeName : FormBase
{
  public InputField InputObject;

  public override void Init()
  {
    InputObject.text = GameStats.Instance.PlayerName;

    InputObject.Select();
    InputObject.ActivateInputField();
  }

  public void InputFieldHandler()
  {
    GameStats.Instance.PlayerName = InputObject.text;
    GameStats.Instance.GameConfig.WriteConfig();
  }
}
