using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void DebugGameObject(this GameObject gameObject)
    {
        Debug.Log(gameObject);
    }

    // RectTransform 참조를 저장하는 딕셔너리
    // static으로 선언하여 프로그램이 시작할 때 한번만 메모리에 할당 되게 함. 또한 static 이므로 모든 인스턴스에서 데이터를 공유함.
    // readonly를 통해 재할당이 불가하도록함 = 딕셔너리가 프로그램 실행 도중 변경되지 않음을 의미
    private static readonly Dictionary<GameObject, RectTransform> _rectTransformDic = new Dictionary<GameObject, RectTransform>();

    public static RectTransform rectTransform(this GameObject gameObject)
    {
        // 캐시된 RectTransform이 있는지 확인하고 반환
        if (_rectTransformDic.TryGetValue(gameObject, out RectTransform cachedRectTransform))
        {
            return cachedRectTransform;
        }

        // 캐시된 RectTransform이 없는 경우 GetComponent로 찾아서 캐시하고 반환
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        _rectTransformDic.Add(gameObject, rectTransform);
        return rectTransform;
    }

    public static Tweener DOAnchorPos(this GameObject gameObject, Vector2 endValue, float duration)
    {
        RectTransform rectTransform = gameObject.rectTransform();
        if (rectTransform != null)
        {
            return rectTransform.DOAnchorPos(endValue, duration);
        }

        // rectTransfor을 쓰는 오브젝트가 아니면, 그냥 DOMove로 이동
        return gameObject.transform.DOMove(endValue, duration);
    }
}
