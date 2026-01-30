using UnityEngine;

public class TableCover : Cover
{
    void Reset()
    {
        coverType = CoverType.Table;
    }

    public override void OnEnter()
    {
        // 桌子一般不需要动画
        // 你也可以在这放个音效
    }
}