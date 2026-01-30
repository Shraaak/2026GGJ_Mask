using UnityEngine;

public class CabinetCover : Cover
{
    [Header("Cabinet")]
    public Animator cabinetAnimator;
    public string openTrigger = "Open";

    private bool opened = false;

    void Reset()
    {
        coverType = CoverType.Cabinet;
    }

    public override void OnOpen()
    {
        if (opened) return;

        opened = true;

        if (cabinetAnimator != null)
            cabinetAnimator.SetTrigger(openTrigger);
    }

    public override void OnEnter()
    {
        // 进入柜子时也可以加音效 / 事件
    }

    public override void OnExit()
    {
        // 48h 期间不建议关门，避免状态复杂
        // 如果你要关门，这里再加 Close Trigger
    }

    public bool IsOpened()
    {
        return opened;
    }
}