using UnityEngine;

public class DropGroup
{
    readonly DropManager _mgr;
    readonly Transform _anchor;
    readonly int _total; // 同時スポーン総数
    int _kills;          // 撃破数
    bool _done;

    public DropGroup(DropManager mgr, Transform anchor, int totalCount)
    {
        _mgr = mgr;
        _anchor = anchor;
        _total = Mathf.Max(0, totalCount);
        _done = false;
        _kills = 0;
    }

    // byPlayer: 撃破true / 逃亡false
    public void NotifyMemberDead(bool byPlayer, Vector3 lastPos)
    {
        //if (_done || !byPlayer) return;   // AllKilled：逃亡はノーカン

        _kills++;
        Debug.Log($"DropGroup: _total={_total}, _kills={_kills}");
        if (_kills >= _total)
        {
            _done = true;
            var pos = _anchor ? _anchor.position : lastPos;
            _mgr.SpawnDrop(pos);
            Debug.Log($"DropGroup: AllKilled at {pos}");
        }
    }
}
