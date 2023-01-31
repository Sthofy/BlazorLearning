﻿namespace BlazorEcommerce.Server.Services.CartService
{
    public interface ICartService
    {
        Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems);
        Task<ServiceResponse<List<CartProductResponse>>> StoreCartItem(List<CartItem> cartItems);
        Task<ServiceResponse<int>> GetCartItemsCount();
        Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts();
    }
}
 