using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProjectIoC;
using System;

namespace ProjectIoCTests
{
    [TestClass]
    public class RegistrationTests
    {
        private IContainer _container;

        [TestInitialize]
        public void Init()
        {
            _container = new Container();
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Register_ShouldThrowAnExceptionWhenRegisteringDuplicates_Test()
        {
            _container.Register(typeof(IMockUser), nameof(MockUser), typeof(MockUser));
            _container.Register(typeof(IMockUser), nameof(MockUser), typeof(MockUser));
        }

        [TestMethod]
        public void Get_GivenRegisteredTypeShouldReturnInstance_Test()
        {
            _container.Register(typeof(IMockUser), nameof(MockUser), typeof(MockUser));

            var instance = _container.GetInstance<IMockUser>();

            Assert.IsNotNull(instance);
        }

        [TestMethod]
        public void Get_GivenInsttiatedTypeShouldReturnTheSameInstance_Test()
        {
            _container.RegisterSingleton(typeof(IMockUser), nameof(MockUser), typeof(MockUser));

            var instance = _container.GetInstance<IMockUser>();

            var secondInstance = _container.GetInstance<IMockUser>();

            Assert.AreEqual(instance.GetHashCode(), secondInstance.GetHashCode());
        }
    }

    public class MockUser : IMockUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public interface IMockUser
    {
        int Id { get; set; } 
        string Name { get; set; }
    }
}
