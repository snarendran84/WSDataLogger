using Castle.Core.Configuration;
using DataLoggerWS.core;
using DataLoggerWS.util;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Reflection;

namespace DataLoggerTest
{
    [TestClass]
    public class DirectoryMonitorTests
    {
        [TestMethod]
        public void when_processFile_method_is_invoked_it_shouldnt_return_an_exception()
        {
            var fileWrapper = new Mock<IFileWrapper>();
            fileWrapper.Setup(r => r.ReadAllLines(It.IsAny<string>())).Returns(new[] { "abc", "bcde" });
            fileWrapper.Setup(r => r.Exists(It.IsAny<string>())).Returns(true);
            fileWrapper.Setup(r => r.Move(It.IsAny<string>(), It.IsAny<string>())).Verifiable();
            fileWrapper.Setup(r => r.Delete(It.IsAny<string>())).Verifiable();

            var mockConf = new Mock<IConfiguration>();
            mockConf.Setup(a => a.GetHashCode()).Returns(0);

            Type classType = typeof(DirectoryMonitor);
            object[] arguments =  { @"c:\temp\" };

            var directoryMonitor = Activator.CreateInstance(classType, arguments);

            MethodInfo processFileMethodInfo = classType.GetMethod("processFile", BindingFlags.NonPublic | BindingFlags.Instance);

            int result = (int) processFileMethodInfo.Invoke(directoryMonitor, arguments);
        }
    }
}
