using Elephanet.Conventions;
using Elephanet.Serialization;
namespace Elephanet
{
    public interface IStoreConventions
    {
        IJsonConverter JsonConverter { get; }
        ITableInfo TableInfo { get; }
        EntityNotFoundBehavior EntityNotFoundBehavior { get; }
    }
}
