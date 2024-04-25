using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMousPos : MonoBehaviour
{
    public RectTransform rectTransform;
    private RectTransform curRect;
    public Vector2 mousePos;

    private void Awake()
    {
        curRect = transform.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition,
           null, out mousePos);

            curRect.anchoredPosition = mousePos;
        }
    }
}
