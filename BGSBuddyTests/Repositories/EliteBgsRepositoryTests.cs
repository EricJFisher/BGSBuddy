using Repositories;
using Xunit;

namespace BGSBuddyTests.Repositories
{
    public class EliteBgsRepositoryTests
    {
        private EliteBgsRepository repository = new EliteBgsRepository();

        [Fact]
        public async void FetchFaction()
        {
            await repository.FetchFaction("Alliance Rapid-reaction Corps");
        }

        [Fact]
        public async void FetchStation()
        {
            await repository.FetchStation("Ridley Scott");
        }

        [Fact]
        public async void FetchSystem()
        {
            await repository.FetchSystem("Zaonce");
        }
    }
}
