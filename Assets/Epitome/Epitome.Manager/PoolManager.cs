using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Epitome
{
    public interface PoolUnit
    {
        Pool_UnitState State();
        void setParentList(object parentList);
        void restore();
    }
    public enum Pool_Type
    {
        Idle,
        Work
    }
    public class Pool_UnitState
    {
        public Pool_Type InPool
        {
            get;
            set;
        }
    }

    public abstract class PoolUnitList<T> where T : class, PoolUnit
    {
        protected object template;
        protected List<T> idleList;
        protected List<T> workList;

        protected int createdNum = 0;

        public PoolUnitList()
        { }


        public virtual T takeUnit<UT>() where UT : T
        {
            T unit;
            if (idleList.Count > 0)
            {
                unit = idleList[0];
                idleList.RemoveAt(0);
            }
            else
            {
                unit = createNewUnit<UT>();
                unit.setParentList(this);
                createdNum++;
            }

            workList.Add(unit);
            unit.State().InPool = Pool_Type.Work;
            OnUnitChangePool(unit);
            return unit;
        }

        public virtual void RestoreUnit(T unit)
        {
            workList.Remove(unit);
            idleList.Add(unit);
            unit.State().InPool = Pool_Type.Idle;
            OnUnitChangePool(unit);
        }

        public void Settemplate(object template)
        {
            this.template = template;
        }

        protected abstract void OnUnitChangePool(T unit);
        protected abstract T createNewUnit<UT>() where UT : T;
    }

    //public abstract class Pool_Base<UnitType, UnitList> : BaseBehavior
    //{ }

    public class Pools : IEnumerable<KeyValuePair<string, Prefabs>>
    {
        public Dictionary<string, Prefabs> poolDict = null;

        public Prefabs prefabs;

        public Prefabs this[string key]
        {
            get
            {
                return null;
            }
            set { poolDict[key] = value; }
        }

        public IEnumerator<KeyValuePair<string, Prefabs>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }

    public class Prefabs : IEnumerable<KeyValuePair<string, Transform>>, PoolUnit
    {
        public Dictionary<string, Transform> poolDict = null;

        public Transform this[string key]
        {
            get
            {
                return null;
            }
            set { poolDict[key] = value; }
        }

        public IEnumerator<KeyValuePair<string, Transform>> GetEnumerator()
        {
            throw new System.NotImplementedException();
        }

        public void restore()
        {
            throw new System.NotImplementedException();
        }

        public void setParentList(object parentList)
        {
            throw new System.NotImplementedException();
        }

        public Pool_UnitState State()
        {
            throw new System.NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new System.NotImplementedException();
        }
    }

    public class PoolManager : Singleton<PoolManager>
    {
        public Dictionary<string, Pools> PoolDict;

        public T Take<T>()
        {
            Pools df = PoolDict["ZIDANG"];
            Transform tran = df.prefabs[""];
            return default(T);
        }

        public void Restore<T>(T t)
        {
            
        }
    }
}
