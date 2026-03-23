using System.Collections.Generic;
using UnityEngine;

namespace LevelCreator.TargetTemplate
{
    public abstract partial class Target : MonoBehaviour
    {
        private static readonly List<Target> targets = new List<Target>();
        public bool InFront(Target data)
        {
            return (data.transform.position.x > transform.position.x) == FaceRight;
        }
        public bool HasEnemy()
        {
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                if (i.Value.Count > 0) return true;
            }
            return false;
        }
        public Target GetNearestEnemy(float range = 99999f, bool requireInFront = false)
        {
            float DMin = range * range; // 使用距离平方进行比较
            Target r = null;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    var mSqr = (transform.position - j.transform.position).sqrMagnitude;
                    if (mSqr < DMin)
                    {
                        r = j;
                        DMin = mSqr;
                    }
                }
            }
            return r;
        }
        public Target GetNearestPartner(float range = 99999f, bool requireInFront = false)
        {
            float DMin = range * range;
            Target r = null;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (j.ObjectId == ObjectId) continue;
                    if (requireInFront && !InFront(j)) continue;
                    var mSqr = (transform.position - j.transform.position).sqrMagnitude;
                    if (mSqr < DMin)
                    {
                        r = j;
                        DMin = mSqr;
                    }
                }
            }
            return r;
        }
        public List<Target> GetEnemyInRange(float range = 99999f, bool requireInFront = false)
        {
            targets.Clear();
            float rangeSqr = range * range;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    if ((transform.position - j.transform.position).sqrMagnitude <= rangeSqr)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public List<Target> GetPartnerInRange(float range = 99999f, bool requireInFront = false)
        {
            targets.Clear();
            float rangeSqr = range * range;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (j.ObjectId == ObjectId) continue;
                    if (requireInFront && !InFront(j)) continue;
                    if ((transform.position - j.transform.position).sqrMagnitude <= rangeSqr)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public List<Target> GetEnemyInRect(float halfx, float halfy, bool requireInFront = false)
        {
            targets.Clear();
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    if (Mathf.Abs(j.transform.position.x - transform.position.x) <= halfx &&
                        Mathf.Abs(j.transform.position.y - transform.position.y) <= halfy)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public List<Target> GetPartnerInRect(float halfx, float halfy, bool requireInFront = false)
        {
            targets.Clear();
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (j.ObjectId == ObjectId) continue;
                    if (requireInFront && !InFront(j)) continue;
                    if (Mathf.Abs(j.transform.position.x - transform.position.x) <= halfx &&
                        Mathf.Abs(j.transform.position.y - transform.position.y) <= halfy)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public Target GetNearestEnemy(Vector3 pos, float range = 99999f, bool requireInFront = false)
        {
            float DMin = range * range; // 使用距离平方进行比较
            Target r = null;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    var mSqr = (pos - j.transform.position).sqrMagnitude;
                    if (mSqr < DMin)
                    {
                        r = j;
                        DMin = mSqr;
                    }
                }
            }
            return r;
        }
        public Target GetNearestPartner(Vector3 pos, float range = 99999f, bool requireInFront = false)
        {
            float DMin = range * range;
            Target r = null;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (j.ObjectId == ObjectId) continue;
                    if (requireInFront && !InFront(j)) continue;
                    var mSqr = (pos - j.transform.position).sqrMagnitude;
                    if (mSqr < DMin)
                    {
                        r = j;
                        DMin = mSqr;
                    }
                }
            }
            return r;
        }
        public List<Target> GetEnemyInRange(Vector3 pos, float range = 99999f, bool requireInFront = false)
        {
            targets.Clear();
            float rangeSqr = range * range;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    if ((pos - j.transform.position).sqrMagnitude <= rangeSqr)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public List<Target> GetPartnerInRange(Vector3 pos, float range = 99999f, bool requireInFront = false)
        {
            targets.Clear();
            float rangeSqr = range * range;
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (j.ObjectId == ObjectId) continue;
                    if (requireInFront && !InFront(j)) continue;
                    if ((pos - j.transform.position).sqrMagnitude <= rangeSqr)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public List<Target> GetEnemyInRect(Vector3 pos, float halfx, float halfy, bool requireInFront = false)
        {
            targets.Clear();
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key == Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (requireInFront && !InFront(j)) continue;
                    if (Mathf.Abs(j.transform.position.x - pos.x) <= halfx &&
                        Mathf.Abs(j.transform.position.y - pos.y) <= halfy)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }
        public List<Target> GetPartnerInRect(Vector3 pos, float halfx, float halfy, bool requireInFront = false)
        {
            targets.Clear();
            foreach (var i in Tool.SceneController.Targets)
            {
                if (i.Key != Camp) continue;
                foreach (var j in i.Value.Values)
                {
                    if (j.ObjectId == ObjectId) continue;
                    if (requireInFront && !InFront(j)) continue;
                    if (Mathf.Abs(j.transform.position.x - pos.x) <= halfx &&
                        Mathf.Abs(j.transform.position.y - pos.y) <= halfy)
                    {
                        targets.Add(j);
                    }
                }
            }
            return targets;
        }

        public enum XLimit { Front, Back }
        public enum YLimit { Highter, Lower }
        public List<Target> XFilter(List<Target> targets, XLimit x)
        {
            switch (x)
            {
                case XLimit.Front:
                    for (int i = targets.Count - 1; i >= 0; i--)
                    {
                        if (!InFront(targets[i])) targets.RemoveAt(i);
                    }
                    break;
                case XLimit.Back:
                    for (int i = targets.Count - 1; i >= 0; i--)
                    {
                        if (InFront(targets[i])) targets.RemoveAt(i);
                    }
                    break;
            }
            return targets;
        }
        public List<Target> YFilter(List<Target> targets, YLimit y)
        {
            switch (y)
            {
                case YLimit.Highter:
                    for (int i = targets.Count - 1; i >= 0; i--)
                    {
                        if (targets[i].transform.position.y < transform.position.y) targets.RemoveAt(i);
                    }
                    break;
                case YLimit.Lower:
                    for (int i = targets.Count - 1; i >= 0; i--)
                    {
                        if (targets[i].transform.position.y > transform.position.y) targets.RemoveAt(i);
                    }
                    break;
            }
            return targets;
        }
    }
}