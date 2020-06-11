using Entities;
using Interfaces.Repositories;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.EntityTests
{
    public class UserSettingsTests
    {
        [Fact]
        public async Task SaveTest()
        {
            // Arrange
            var mock = new Mock<IFileSystemRepository>();
            mock.Setup(r => r.SaveJsonToFile(It.IsAny<string>(), It.IsAny<string>()));

            var userSettings = new UserSettings(mock.Object);
            userSettings.FactionName = "Test Faction";
            userSettings.OffLimits = new List<string> { "system one", "system two", "system three" };

            // Act
            var task = userSettings.Save();
            task.Wait();

            // Assert
            Assert.True(task.Status != TaskStatus.Faulted, "FileSystemRepository.Save failed to run: " + task.Exception);
        }

        [Fact]
        public async Task LoadTest()
        {
            // Arrange
            var mock = new Mock<IFileSystemRepository>();
            mock.Setup(r => r.RetrieveJsonFromFile(It.IsAny<string>())).Returns(@"{""FactionName"":""Alliance Rapid-reaction Corps"",""OffLimits"":[""Biria"",""Bruthanvan"",""CD-45 7854""]}");

            var userSettings = new UserSettings(mock.Object);
            var expectedName = "Alliance Rapid-reaction Corps";
            var expectedOffLimits = new List<string> { "Biria", "Bruthanvan", "CD-45 7854" };

            //Act
            var task = userSettings.Load();
            task.Wait();

            //Assert
            Assert.True(task.Status != TaskStatus.Faulted, "FileSystemRepository.Save failed to run: " + task.Exception);
            Assert.Equal(userSettings.FactionName, expectedName);
            Assert.Equal(expectedOffLimits, userSettings.OffLimits);
        }
    }
}
