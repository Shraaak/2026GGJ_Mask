using UnityEngine;

public class CabinetCover : Cover
{
    [Header("Cabinet")]
    public Animator cabinetAnimator;
    public string openTrigger = "OpenCabinet";

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
        {
            print("触发动画");
            cabinetAnimator.SetTrigger(openTrigger);
        }
            
    }

    public override bool CanEnter()
    {
        return opened && !isOccupied;
    }

    public override void OnEnter()
    {
        // 可加音效 / 事件
    }

    public override void OnExit()
    {
        // 48h 不建议关门
    }
}