using System.Threading.Tasks;

namespace K8sJanitor.WebApi.Repositories
{
    public class PersistenceRepositoryStub :IPersistenceRepository
    {
        public Task StoreFile(string content, string contentType)
        {
            return Task.CompletedTask;
        }
    }
}