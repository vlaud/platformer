using UnityEngine;
using DG.Tweening;
public class UIAction : IUIAction
{
    public void Activate(IUIScriptablesInfos info)
    {
        Vector2 showPos = info.GetShowPos();
        RectTransform curBlock = info.GetBlock();
        float MenuMoveTime = info.GetMenuMoveTime();

        SetBlockState(ref curBlock, true);

        info.GetRectTransform().DOAnchorPos(showPos, MenuMoveTime, false)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => SetBlockState(ref curBlock, false));
    }

    public void Deactivate(IUIScriptablesInfos info)
    {
        Vector2 hidePos = info.GetHidePos();
        RectTransform curBlock = info.GetBlock();
        float MenuMoveTime = info.GetMenuMoveTime();

        SetBlockState(ref curBlock, true);
        info.GetRectTransform().DOAnchorPos(hidePos, MenuMoveTime, false)
            .SetEase(Ease.OutQuad);
    }

    private void SetBlockState(ref RectTransform block, bool v)
    {
        block.gameObject.SetActive(v);
    }
}