using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneViewChildView : MonoBehaviour
{
    protected SceneView mySceneView_ = null;
    public SceneView MySceneView {
        get { return mySceneView_; }
        set { mySceneView_ = value; }
    }
}
