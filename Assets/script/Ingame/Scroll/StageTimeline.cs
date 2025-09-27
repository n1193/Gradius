using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Stage/Timeline")]
public class StageTimeline : ScriptableObject
{
    [System.Serializable]
    public class SpawnEvent
    {
        public GameObject prefab;
        public float y;
        public int triggerX;
        public int count = 1;
        public float interval = 0f;
        public float spacingY = 0f;

        public bool forceDropUpgrade = false;
    }

    public List<SpawnEvent> events = new();
}
