// Copyright (c) 2021 homuler
//
// Use of this source code is governed by an MIT-style
// license that can be found in the LICENSE file or at
// https://opensource.org/licenses/MIT.

using UnityEngine.UI;

namespace Mediapipe.Unity.UI
{
  public class GlobalConfig : ModalContents
  {
    const string _GlogLogtostederrPath = "Scroll View/Viewport/Contents/GlogLogtostderr/Toggle";
    const string _GlogStderrthresholdPath = "Scroll View/Viewport/Contents/GlogStderrthreshold/Dropdown";
    const string _GlogMinloglevelPath = "Scroll View/Viewport/Contents/GlogMinloglevel/Dropdown";
    const string _GlogVPath = "Scroll View/Viewport/Contents/GlogV/Dropdown";
    const string _GlogLogDirPath = "Scroll View/Viewport/Contents/GlogLogDir/InputField";

    Toggle _glogLogtostderrInput;
    Dropdown _glogStderrthresholdInput;
    Dropdown _glogMinloglevelInput;
    Dropdown _glogVInput;
    InputField _glogLogDirInput;

    void Start()
    {
      InitializeGlogLogtostderr();
      InitializeGlogStderrthreshold();
      InitializeGlogMinloglevel();
      InitializeGlogV();
      InitializeGlogLogDir();
    }

    public void SaveAndExit()
    {
      GlobalConfigManager.GlogLogtostderr = _glogLogtostderrInput.isOn;
      GlobalConfigManager.GlogStderrthreshold = _glogStderrthresholdInput.value;
      GlobalConfigManager.GlogMinloglevel = _glogMinloglevelInput.value;
      GlobalConfigManager.GlogLogDir = _glogLogDirInput.text;
      GlobalConfigManager.GlogV = _glogVInput.value;

      GlobalConfigManager.Commit();
      Exit();
    }

    void InitializeGlogLogtostderr()
    {
      _glogLogtostderrInput = gameObject.transform.Find(_GlogLogtostederrPath).gameObject.GetComponent<Toggle>();
      _glogLogtostderrInput.isOn = GlobalConfigManager.GlogLogtostderr;
    }

    void InitializeGlogStderrthreshold()
    {
      _glogStderrthresholdInput = gameObject.transform.Find(_GlogStderrthresholdPath).gameObject.GetComponent<Dropdown>();
      _glogStderrthresholdInput.value = GlobalConfigManager.GlogStderrthreshold;
    }

    void InitializeGlogMinloglevel()
    {
      _glogMinloglevelInput = gameObject.transform.Find(_GlogMinloglevelPath).gameObject.GetComponent<Dropdown>();
      _glogMinloglevelInput.value = GlobalConfigManager.GlogMinloglevel;
    }

    void InitializeGlogV()
    {
      _glogVInput = gameObject.transform.Find(_GlogVPath).gameObject.GetComponent<Dropdown>();
      _glogVInput.value = GlobalConfigManager.GlogV;
    }

    void InitializeGlogLogDir()
    {
      _glogLogDirInput = gameObject.transform.Find(_GlogLogDirPath).gameObject.GetComponent<InputField>();
      _glogLogDirInput.text = GlobalConfigManager.GlogLogDir;
    }
  }
}
