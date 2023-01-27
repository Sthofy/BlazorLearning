namespace BlazorEcommerce.Client.Services.ProductService
{
    public interface IProductService
    {
        string Message { get; set; }
        int CurrentPage { get; set; }
        int PageCount { get; set; }
        string LastSearchText { get; set; }
        event Action ProductChanged;
        List<Product> Products { get; set; }
        Task GetProducts(string? categoryUrl = null);
        Task<ServiceResponse<Product>> GetProductAsync(int productId);
        Task SearchProducts(string searchText, int page);
        Task<List<string>> GetProductSeachSuggestions(string searchText);
    }
}
