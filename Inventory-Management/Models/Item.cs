namespace InventoryAPI.Models
{
    public class Item
    {
        public int ItemID { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string? ImagePath { get; set; }
        public int SupplierID { get; set; }
        public string? SupplierName { get; set; } 
        public int TotalCount { get; set; } 
    }
}