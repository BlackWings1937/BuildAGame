using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour {

    public LineRenderer myLr_;

    public Vector3[] myPoes_;

    public float addIndex = 0;
	// Use this for initialization
	void Start () {
        myPoes_ = new Vector3[2];
        myPoes_[0] = new Vector3(0,0,0);
        myPoes_[1] = new Vector3(0,0,0);
        if (!myLr_) {
            myLr_ = GetComponent<LineRenderer>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        myLr_.SetPositions(myPoes_);
        myPoes_[1].y = myPoes_[1].y + 0.01f;
        if (myPoes_[1].y > 10) {
            myPoes_[1].y = 0;
        }

    }
}
