using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : AppChildController {

    // ----- 对外接口 -----
    public void BackToPackageEditView() {
        DisposeController();
    }
}
