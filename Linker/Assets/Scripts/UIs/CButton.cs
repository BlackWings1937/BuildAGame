using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CButton : Button
{
    
    public override void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("----------------------------------------------OnPointerDown");
        base.OnPointerDown(eventData);
    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("----------------------------------------------OnPointerUp");
        base.OnPointerUp(eventData);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
