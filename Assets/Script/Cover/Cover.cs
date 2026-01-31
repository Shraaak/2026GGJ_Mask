using UnityEngine;

public enum CoverType
{
    Table,
    Cabinet
}

public class Cover : MonoBehaviour
{
    [Header("Base")]
    public CoverType coverType;

    [Tooltip("玩家进入掩体后的位置")]
    public Transform hidePoint;

    [HideInInspector]
    public bool isOccupied = false;

    public virtual bool CanEnter()
    {
        return !isOccupied;
    }

    // 给 PlayerCoverController 调用
    public virtual void OnOpen() { }
    public virtual void OnEnter() { }
    public virtual void OnExit() { }
}