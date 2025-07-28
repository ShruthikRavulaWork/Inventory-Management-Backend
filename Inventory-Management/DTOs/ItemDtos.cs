using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class ItemCreateDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [Range(0.01, 1000000)]
        public decimal Price
        {
            get; set;
        }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity
        {
            get; set;
        }
        [Required]
        public int SupplierID
        {
            get; set;
        }
        public IFormFile? Image
        {
            get; set;
        }
    }

    public class ItemUpdateDto : ItemCreateDto
    {
        // Inherits all properties from ItemCreateDto
    }

    public class SupplierItemUpdateDto
    {
        [Required]
        [Range(0.01, 1000000)]
        public decimal Price
        {
            get; set;
        }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity
        {
            get; set;
        }
    }

    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new List<T>();
        public int TotalCount
        {
            get; set;
        }
    }
}