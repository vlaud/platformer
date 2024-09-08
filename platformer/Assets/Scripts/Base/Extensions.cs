using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void DebugGameObject(this GameObject gameObject)
    {
        Debug.Log(gameObject);
    }

    // RectTransform ������ �����ϴ� ��ųʸ�
    // static���� �����Ͽ� ���α׷��� ������ �� �ѹ��� �޸𸮿� �Ҵ� �ǰ� ��. ���� static �̹Ƿ� ��� �ν��Ͻ����� �����͸� ������.
    // readonly�� ���� ���Ҵ��� �Ұ��ϵ����� = ��ųʸ��� ���α׷� ���� ���� ������� ������ �ǹ�
    private static readonly Dictionary<GameObject, RectTransform> _rectTransformDic = new Dictionary<GameObject, RectTransform>();

    public static RectTransform rectTransform(this GameObject gameObject)
    {
        // ĳ�õ� RectTransform�� �ִ��� Ȯ���ϰ� ��ȯ
        if (_rectTransformDic.TryGetValue(gameObject, out RectTransform cachedRectTransform))
        {
            return cachedRectTransform;
        }

        // ĳ�õ� RectTransform�� ���� ��� GetComponent�� ã�Ƽ� ĳ���ϰ� ��ȯ
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

        // rectTransfor�� ���� ������Ʈ�� �ƴϸ�, �׳� DOMove�� �̵�
        return gameObject.transform.DOMove(endValue, duration);
    }
}
