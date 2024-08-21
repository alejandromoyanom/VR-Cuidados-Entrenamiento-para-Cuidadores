using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ProgressBarManager : MonoBehaviour
{
    public Image fillImage;
    public float animationDuration = 5f;
    public TMP_Text progressText;

    private void Start()
    {
        UpdateProgressValue(1f);
    }

    public void UpdateProgressValue(float targetValue)
    {
        DOTween.To(() => fillImage.fillAmount, x => fillImage.fillAmount = x, targetValue, animationDuration).OnUpdate(() =>
        {
            progressText.text = (int)(fillImage.fillAmount * 100) + "%";
        });
    }
}
