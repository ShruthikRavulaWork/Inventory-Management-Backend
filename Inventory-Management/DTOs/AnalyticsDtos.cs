namespace InventoryAPI.DTOs
{
    public class LeastStockItemDto
    {
        public string Name { get; set; } = string.Empty;
        public int Quantity
        {
            get; set;
        }
    }

    public class SupplierStockDto
    {
        public string Username { get; set; } = string.Empty;
        public int TotalQuantity
        {
            get; set;
        }
    }
}