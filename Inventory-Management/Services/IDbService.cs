using System.Data;

namespace InventoryAPI.Services
{
    public interface IDbService
    {
        IDbConnection CreateConnection();
    }
}