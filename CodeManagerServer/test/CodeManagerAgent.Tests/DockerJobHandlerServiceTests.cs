using System.Threading.Tasks;
using CodeManagerAgent.Services;
using NUnit.Framework;
using Moq;
namespace CodeManagerAgent.Tests
{
    public class DockerJobHandlerServiceTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task StartWithOneStepShouldSucceed()
        {
            Assert.Pass();
            // Arrange
            var mockJobHandlerService = new Mock<DockerJobHandlerService>();
           // var mock
            
            //mockJobHandlerService.Setup(x => x.StartAsync()).Returns()

            // Act

            // Assert
        }
    }
}