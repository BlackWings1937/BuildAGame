﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcOtionsTrain : MonoBehaviour
{

    // ----- 私有成员 -----
    [SerializeField]
    private GameObject prefabNpcOptionItems_ = null;


    private List<BtnAdaptText> listOfItems_ = new List<BtnAdaptText>();

    private NpcOptions data_ = null;

    // ----- 私有方法 -----
    private void clearItems() {
        var count = listOfItems_.Count;
        for (int i = 0;i<count;++i) {
            GameObject.Destroy(listOfItems_[i].gameObject);
        }
        listOfItems_.Clear();
    }

    private void reCaculateSelfHeight() {
        var height = 0.0f;

        var count = listOfItems_.Count;
        for (int i = 0;i<count;++i) {
            var itemSize = (listOfItems_[i].transform as RectTransform).sizeDelta;
            if (itemSize.y > height) {
                height = itemSize.y;
            }
        }
        var size = (transform as RectTransform).sizeDelta;
        (transform as RectTransform).sizeDelta = new Vector2(size.x,height);
    }
    private void reCaculateSelfWidth() {
        var width = 0.0f;
        var horizontalPadding = GetComponent<HorizontalLayoutGroup>();
        var spacing = horizontalPadding.spacing;
        var leftPadding = horizontalPadding.padding.left;
        var rightPadding = horizontalPadding.padding.right;
        width = width + leftPadding + rightPadding + Mathf.Max(0, spacing * (listOfItems_.Count - 1));

        var count = listOfItems_.Count;
        for (int i = 0;i<count;++i) {
            var itemSize = (listOfItems_[i].transform as RectTransform).sizeDelta;
            width += itemSize.x;
        }
        var size = (transform as RectTransform).sizeDelta;
        (transform as RectTransform).sizeDelta = new Vector2(width,size.y);
    }
    private void reCaculateSelfSize() {
        reCaculateSelfHeight();
        reCaculateSelfWidth();
    }

    // ----- 对外接口 -----
    public void UpdateByPevAndNpcNameAndOptionLIst(PlotEditView pev,NpcOptions data) {
        data_ = data;
        if (data_!= null && prefabNpcOptionItems_ != null) {
            clearItems();
            var npcNameItem = GameObject.Instantiate(prefabNpcOptionItems_) as GameObject;
            npcNameItem.GetComponent<BtnAdaptText>().SetText(data_.NpcName);
            npcNameItem.GetComponent<Button>().onClick.AddListener(()=> {
                if (pev!=null) {
                    pev.OnBtnClickAtNpcItem(npcNameItem.transform as RectTransform, data_);
                }
            });
            npcNameItem.transform.SetParent(this.transform,false);
            listOfItems_.Add(npcNameItem.GetComponent<BtnAdaptText>());


            var count = data_.listOfOptions.Count;
            for (int i = 0;i<count;++i) {
                var option = data_.listOfOptions[i];
                var opItem = GameObject.Instantiate(prefabNpcOptionItems_) as GameObject;
                var batItem = opItem.GetComponent<BtnAdaptText>();
                batItem.SetText(NpcOption.EToS(option.MyState) +  option.ExData);
                var btn = opItem.GetComponent<Button>();
                btn.onClick.AddListener(()=> {
                    if (pev!=null) {
                        pev.OnBtnClickAtOptionItem(btn.transform as RectTransform, option);
                    }
                });
                opItem.transform.SetParent(this.transform,false);
                listOfItems_.Add(batItem);
            }

            reCaculateSelfSize();
        }
    }
}