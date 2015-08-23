using Elephanet.Serialization;
namespace Elephanet
{
    public interface IStoreConventions
    {
        IJsonConverter JsonConverter { get; }
        ITableInfo TableInfo { get; }
    }
}
