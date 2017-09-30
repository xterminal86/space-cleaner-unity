using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FormExit : FormBase 
{
  public override void Select(FormBase parentForm)
  {
    base.Select(parentForm);

    GameStats.Instance.GameConfig.WriteConfig();

    Application.Quit();
  }
}
