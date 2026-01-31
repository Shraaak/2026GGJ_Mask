using UnityEngine;

public class TableCover : Cover
{
    void Reset()
    {
        coverType = CoverType.Table;
    }

    public override bool CanEnter()
    {
        return !isOccupied;
    }
}