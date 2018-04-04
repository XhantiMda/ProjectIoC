using ProjectIoC.Enums;
using System;

namespace ProjectIoC.Models
{
    internal class LookupObject
    {
        public LookupObject(Type registeredAs, Type instanceType, RegistrationType registrationType = RegistrationType.PerRequest)
        {
            RegisteredAs = registeredAs;
            InstanceType = instanceType;
            RegistrationType = registrationType;
        }

        public Type RegisteredAs { get; }
        public Type InstanceType { get; }
        public RegistrationType RegistrationType { get; }
        public object Instance { get; set; } 
        public bool IsIstantiated { get; set; }
    }
}