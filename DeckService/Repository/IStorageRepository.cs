using System.Threading.Tasks;
using Microsoft.Azure.Documents;

namespace DeckService.Repository
{
    public interface IStorageRepository<T> where T : class
    {
        Task Initialize();

        Task<T> GetItemAsync(string id);

        Task<Document> CreateItemAsync(T item);

        Task<Document> UpdateItemAsync(string id, T item);

        Task<Document> DeleteItemAsync(string id);
    }
}
