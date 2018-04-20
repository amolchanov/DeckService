namespace DeckService.Repository
{
    public interface IStorageRepositoryFactory<T>  where T : class
    {
        IStorageRepository<T> GetStorageRepository(bool isDevEnvironment);
    }
}
