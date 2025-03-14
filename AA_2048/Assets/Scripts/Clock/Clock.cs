using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clock : MonoBehaviour
{
    public Image imgHour;
    public Image imgMinute;
    public Image imgSecond;

    private const float degreeDiff = 90; //默认需要加90度才会归零
    private const float hoursToDegrees = -30f; //1小时旋转的角度
    private const float minutesToDegrees = -6f; //1分钟旋转的角度
    private const float secondsToDegrees = -6f; //1秒钟旋转的角度

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        var timeSpan = DateTime.Now.TimeOfDay;
        imgHour.transform.localRotation =
            Quaternion.Euler(0f, 0f, (float)(hoursToDegrees * timeSpan.TotalHours) + degreeDiff);
        imgMinute.transform.localRotation =
            Quaternion.Euler(0, 0, (float)(minutesToDegrees * timeSpan.TotalMinutes) + degreeDiff);
        imgSecond.transform.localRotation =
            Quaternion.Euler(0, 0, (float)(secondsToDegrees * timeSpan.TotalSeconds) + degreeDiff);
    }
}