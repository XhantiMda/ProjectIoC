using System;

namespace ProjectIoC
{
    public interface IContainer
    {
        void Register(Type lookupType, string key, Type type);
        void RegisterSingleton<T>();
        void RegisterSingleton(Type lookupType, string key, Type type);
        void RegisterPerRequest<T>();
        T GetInstance<T>(string key = null);
        object GetInstance(Type type, string key = null);
    }
}
