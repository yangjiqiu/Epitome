using System;
using System.Collections.Generic;

namespace Epitome
{
    public class ModuleManager : Singleton<ModuleManager>
    {
        protected ModuleManager() { }

        Dictionary<string, ModuleBase> moduleDict;

        public override void OnSingletonInit()
        {
            moduleDict = new Dictionary<string, ModuleBase>();

            base.OnSingletonInit();
        }

        public ModuleBase Get(string type)
        {
            if (moduleDict.ContainsKey(type))
            {
                return moduleDict[type];
            }

            return null;
        }

        public T Get<T>() where T : ModuleBase
        {
            Type type = typeof(T);

            if (moduleDict.ContainsKey(type.ToString()))
            {
                return moduleDict[type.ToString()] as T;
            }

            return default(T);
        }

        protected void AddModule(Type moduleType)
        {
            ModuleBase module = Activator.CreateInstance(moduleType) as ModuleBase;
            module.Load();
        }

        public virtual void RegisterAllModules() { }

        public void Register(string key, ModuleBase module)
        {
            if (!moduleDict.ContainsKey(key))
            {
                moduleDict.Add(key, module);
            }
        }

        public void Register(ModuleBase module)
        {
            Register(module.GetType().ToString(), module);
        }

        public void UnRegister(string key)
        {
            if (moduleDict.ContainsKey(key))
            {
                ModuleBase module = moduleDict[key];
                module.Release();
                moduleDict.Remove(key);
                module = null;
            }
        }

        public void UnRegister(ModuleBase module)
        {
            UnRegister(module.GetType().ToString());
        }

        public void UnRegisterAll()
        {
            List<string> keyList = new List<string>(moduleDict.Keys);
            for (int i = 0; i < keyList.Count; i++)
            {
                UnRegister(keyList[i]);
            }
            moduleDict.Clear();
        }
    }
}

