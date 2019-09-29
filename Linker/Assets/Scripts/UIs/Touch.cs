/*
 * 模拟每一次触碰的touch 对象
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate List<TouchObject> GetTouchObject();

public class Touch 
{
    private TouchObject swallowTouchObj_ = null;
    private GetTouchObject getTouchObjects_ = null;
    public void SetGetTouchObjects(GetTouchObject cb) { getTouchObjects_ = cb; }

    public void OnTouchBegan(Vector2 mousePos) {
        if (swallowTouchObj_ == null) {
            var worldPos = TransformUtils.ScreenPosToWorldPos(mousePos);
            var listOfTouchObjects = getTouchObjects_();
            var count = listOfTouchObjects.Count;
            for (int i = count-1; 0<=i;--i) {
                if (listOfTouchObjects[i].gameObject.activeSelf == false) { continue; }
                var result = listOfTouchObjects[i].OnTouchBegan(worldPos);
                if (result)
                {
                    swallowTouchObj_ = listOfTouchObjects[i];
                    break;
                }
                else {
                   // listOfTouchObjects.RemoveAt(i);
                }
            }
        } 
    }
    public void OnTouchMoved(Vector2 mousePos) {
        var worldPos = TransformUtils.ScreenPosToWorldPos(mousePos);

        if (swallowTouchObj_ == null) {
            var listOfTouchObjects = getTouchObjects_();
            var count = listOfTouchObjects.Count;
            for (int i = count - 1; 0 <= i; --i)
            {
                if (listOfTouchObjects[i].gameObject.activeSelf == false) { continue; }
                var result = listOfTouchObjects[i].OnTouchMoved(worldPos);
                if (result)
                {
                    swallowTouchObj_ = listOfTouchObjects[i];
                    break;
                }
            }
        } else {
            swallowTouchObj_.OnTouchMoved(worldPos);
        }
    }
    public void OnTouchEnded(Vector2 mousePos) {

        var worldPos = TransformUtils.ScreenPosToWorldPos(mousePos);

        if (swallowTouchObj_ == null) {
            var listOfTouchObjects = getTouchObjects_();
            var count = listOfTouchObjects.Count;
            for (int i = count - 1; 0 <= i; --i)
            {
                if (listOfTouchObjects[i].gameObject.activeSelf == false) { continue; }
                var result = listOfTouchObjects[i].OnTouchEnded(worldPos);
                if (result)
                {
                    swallowTouchObj_ = listOfTouchObjects[i];
                    break;
                }
            }
        } else {
            swallowTouchObj_.OnTouchEnded(worldPos);
        }
    }

}
