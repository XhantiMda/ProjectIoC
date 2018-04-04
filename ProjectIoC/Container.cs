using ProjectIoC.Enums;
using ProjectIoC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectIoC
{
    public class Container : IContainer
    {
        private readonly Dictionary<string, LookupObject> _instanceCollection;

        public Container()
        {
            _instanceCollection = new Dictionary<string, LookupObject>();
        }

        /// <summary>
        /// Returns an instance of a registered type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetInstance<T>(string key = null)
        {
            return (T)GetInstance(typeof(T), key);
        }

        /// <summary>
        /// Registers type in the container.
        /// </summary>
        /// <param name="lookupType"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public void Register(Type lookupType, string key, Type type)
        {
            Register(lookupType, key, type, RegistrationType.PerRequest);
        }

        /// <summary>
        /// Registers a type to be created on every request. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterPerRequest<T>()
        {
            Register(typeof(T), nameof(T), typeof(T), RegistrationType.PerRequest);
        }

        /// <summary>
        /// Registers a singleton.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public void RegisterSingleton<T>()
        {
            Register(typeof(T), nameof(T), typeof(T), RegistrationType.Singleton);
        }

        private void Register(Type lookupType, string key, Type type, RegistrationType registrationType)
        {
            if (_instanceCollection.ContainsKey(key))
                throw new InvalidOperationException($"{key} has already been registered");

            _instanceCollection.Add(key, new LookupObject(lookupType, type, registrationType));
        }

        /// <summary>
        /// Creates an instance of the registered object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lookupObject"></param>
        /// <returns></returns>
        private object Resolve(LookupObject lookupObject)
        {
            if (lookupObject.IsIstantiated && lookupObject.RegistrationType == RegistrationType.Singleton)
                return lookupObject.Instance;

            //create an instance of the object 
            var instance = InstanceConstructor(lookupObject.InstanceType);

            //save instance for singletons 
            if (lookupObject.RegistrationType == RegistrationType.Singleton)
            {
                lookupObject.Instance = instance;
                lookupObject.IsIstantiated = true;
            }

            return instance;
        }

        /// <summary>
        /// Registered a Singleton instance of the type.
        /// </summary>
        /// <param name="lookupType"></param>
        /// <param name="key"></param>
        /// <param name="type"></param>
        public void RegisterSingleton(Type lookupType, string key, Type type)
        {
            Register(lookupType, key, type, RegistrationType.Singleton);
        }

        /// <summary>
        /// Gets an instance of a registered type either by key or type.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public object GetInstance(Type type, string key = null)
        {
            //find instance by type 
            if (string.IsNullOrWhiteSpace(key))
            {
                //tries to find an instance for the specified type  
                var lookupObj = _instanceCollection
                    .Where(lookupObjCollection => lookupObjCollection.Value.RegisteredAs.Equals(type))
                    .Select(lookupObjKeyValue => lookupObjKeyValue.Value)
                    .SingleOrDefault();

                //can only retrieve registered types
                if (lookupObj == null)
                    return default(object);

                return Resolve(lookupObj);
            }

            //find instance by key
            if (!_instanceCollection.Keys.Contains(key))
                return default(object);

            return Resolve(_instanceCollection[key]);
        }

        /// <summary>
        /// Creates and returns instance of the specified type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="typeToInstantiate"></param>
        /// <returns></returns>
        private object InstanceConstructor(Type typeToInstantiate)
        {
            //find constructor with the most params
            var constructor = typeToInstantiate
                .GetConstructors()
                .OrderByDescending(ctor => ctor.GetParameters().Count())
                .First();

            //get all constructor params
            var parameters = constructor.GetParameters();

            if (parameters.Count() > 0)
            {
                //list of instantiated parameters
                var instantiatedParams = new List<object>();

                //resolve params
                foreach (var parameter in parameters)
                {
                    instantiatedParams.Add(GetInstance(parameter.ParameterType));
                }

                //create instance with params
                return Activator.CreateInstance(typeToInstantiate, instantiatedParams);
            }

            //create instance without params
            return Activator.CreateInstance(typeToInstantiate);
        }

    }
}
