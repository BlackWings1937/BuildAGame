/*
 * 模拟每一次触碰的touch 对象
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate List<TouchObject> GetTouchObject();

public class Touch : MonoBehaviour
{
    private TouchObject swallowTouchObj_ = null;
    private GetTouchObject getTouchObjects_ = null;
    public void SetGetTouchObjects(GetTouchObject cb) { getTouchObjects_ = cb; }

    public void OnTouchBegan(Vector2 mousePos) {

    }
    public void OnTouchMoved(Vector2 mousePos) {

    }
    public void OnTouchEnded(Vector2 mousePos) {

    }
}
