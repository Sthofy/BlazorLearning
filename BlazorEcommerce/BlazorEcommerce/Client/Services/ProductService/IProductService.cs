namespace BlazorEcommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        public string Message { get; set; }
        event Action ProductChanged;
        List<Product> Products { get; set; }
        Task GetProducts(string? categoryUrl = null);
        Task<ServiceResponse<Product>> GetProductAsync(int productId);
        Task SearchProducts(string searchText);
        Task<List<string>> GetProductSeachSuggestions(string searchText);
    }
}
