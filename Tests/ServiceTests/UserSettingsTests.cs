using Entities;
using Interfaces.Repositories;
using Interfaces.Services;
using Moq;
using Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Tests.ServiceTests
{
    public class UserSettingsTests
    {
        [Fact]
        public async Task SaveTest()
        {
            // Arrange
            var mock = new Mock<IFileSystemRepository>();
            mock.Setup(r => r.SaveJsonToFile(It.IsAny<string>(), It.IsAny<string>()));

            var userSettingsService = new UserSettingsService(mock.Object);

            var userSettings = new UserSettings();
            userSettings.FactionName = "Test Faction";
            userSettings.OffLimits = new List<string> { "system one", "system two", "system three" };

            // Act
            var task = userSettingsService.Save(userSettings);
            await task;

            // Assert
            Assert.True(task.Status != TaskStatus.Faulted, "FileSystemRepository.Save failed to run: " + task.Exception);
        }

        [Fact]
        public async Task LoadTest()
        {
            // Arrange
            var mock = new Mock<IFileSystemRepository>();
            mock.Setup(r => r.RetrieveJsonFromFile(It.IsAny<string>())).Returns(@"{""FactionName"":""Alliance Rapid-reaction Corps"",""OffLimits"":[""Biria"",""Bruthanvan"",""CD-45 7854""]}");

            var userSettingsService = new UserSettingsService(mock.Object);

            var expected = new UserSettings();
            expected.FactionName = "Alliance Rapid-reaction Corps";
            expected.OffLimits = new List<string> { "Biria", "Bruthanvan", "CD-45 7854" };

            //Act
            var task = userSettingsService.Load();
            var actual = await task;

            //Assert
            Assert.True(task.Status != TaskStatus.Faulted, "FileSystemRepository.Save failed to run: " + task.Exception);
            Assert.Equal(expected.FactionName, actual.FactionName);
            Assert.Equal(expected.OffLimits, actual.OffLimits);
        }
    }
}
