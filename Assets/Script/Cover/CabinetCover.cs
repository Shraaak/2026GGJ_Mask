using UnityEngine;

public class CabinetCover : Cover
{

    private bool opened = false;

    void Reset()
    {
        coverType = CoverType.Cabinet;
    }

    public override void OnOpen()
    {
        if (opened) return;

        opened = true;
            
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