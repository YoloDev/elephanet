namespace Elephanet
{
    public interface IDocumentStore
    {
        IDocumentSession OpenSession();
        string ConnectionString { get; }
        IStoreConventions Conventions { get; }
        IStoreInfo StoreInfo { get; }
        void Empty();
    }
}
