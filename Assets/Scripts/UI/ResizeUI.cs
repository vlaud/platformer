using UnityEngine;
using UnityEngine.EventSystems;

public class ResizeUI : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    private RectTransform SetTransform;

    private RectTransform rectTransform;
    private RectTransform curRect;

    private Vector2 curPos;
    private Vector2 prevPos;
    private Vector2 dirPos = Vector2.one;

    private Vector2 size;

    private Vector2 curSize;
    private Vector2 prevSize;

    private Vector2 sizeGap;
    private Vector2 sizeDelta;
    private Vector2 prevRectAnchoredPos;

    private Vector2 moveAmount;

    public bool isSide = false;

    private void Awake()
    {
        rectTransform = transform.parent.GetComponent<RectTransform>();
        curRect = transform.GetComponent<RectTransform>();
        SetTransform = UIManager.Inst.SetTransform;

        dirPos.x = curRect.anchoredPosition.x < 0f ? 1f : curRect.anchoredPosition.x > 0f ? -1f : 0f;
        dirPos.y = curRect.anchoredPosition.y < 0f ? 1f : curRect.anchoredPosition.y > 0f ? -1f : 0f;

        size = transform.GetComponent<RectTransform>().sizeDelta;

        SetSideSize();
        isSide = dirPos.x == 0f || dirPos.y == 0f;
    }

    void SetSideSize()
    {
        if (dirPos.x == 0f)
        {
            curRect.sizeDelta = new Vector2(rectTransform.sizeDelta.x - 2f * size.x, size.y);
        }
        if (dirPos.y == 0f)
        {
            curRect.sizeDelta = new Vector2(size.x, rectTransform.sizeDelta.y - 2f * size.y);
        }
    }

    void SetMinMaxPos(bool isPositive, out float min, out float max, float minAmount, float maxAmount)
    {
        if (isPositive)
        {
            min = minAmount;
            max = maxAmount;
        }
        else
        {
            min = -maxAmount;
            max = -minAmount;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null)
            return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(SetTransform, eventData.position,
            eventData.pressEventCamera, out curPos);

        sizeDelta.x = (curPos.x - prevPos.x) * dirPos.x;
        sizeDelta.y = (curPos.y - prevPos.y) * dirPos.y;

        Vector2 temp = prevSize + sizeDelta;
        temp.x = Mathf.Clamp(temp.x, UIManager.Inst.minSize.x, UIManager.Inst.maxSize.x);
        temp.y = Mathf.Clamp(temp.y, UIManager.Inst.minSize.y, UIManager.Inst.maxSize.y);

        rectTransform.sizeDelta = temp;
        curSize = rectTransform.sizeDelta;

        sizeGap = curSize - prevSize;
        moveAmount.x = prevRectAnchoredPos.x + sizeGap.x * 0.5f * dirPos.x;
        moveAmount.y = prevRectAnchoredPos.y + sizeGap.y * 0.5f * dirPos.y;


        rectTransform.anchoredPosition = moveAmount;

        foreach (var side in UIManager.Inst.Sides)
        {
            side.SetSideSize();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        sizeDelta = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        rectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(SetTransform, eventData.position,
            eventData.pressEventCamera, out prevPos);

        prevSize = rectTransform.sizeDelta;
        prevRectAnchoredPos = rectTransform.anchoredPosition;
    }
}
