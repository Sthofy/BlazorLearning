﻿using System.Security.Claims;

namespace BlazorEcommerce.Server.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly DataContext _dataContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(DataContext dataContext, IHttpContextAccessor httpContextAccessor)
        {
            _dataContext = dataContext;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId() => int.Parse(_httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

        public async Task<ServiceResponse<List<CartProductResponse>>> GetCartProducts(List<CartItem> cartItems)
        {
            var result = new ServiceResponse<List<CartProductResponse>>
            {
                Data = new List<CartProductResponse>(),
            };

            foreach (var item in cartItems)
            {
                var product = await _dataContext.Products
                    .Where(p => p.Id == item.ProductId)
                    .FirstOrDefaultAsync();

                if (product == null)
                {
                    continue;
                }

                var productVariant = await _dataContext.ProductVariants
                    .Where(v => v.ProductId == item.ProductId && v.ProductTypeId == item.ProductTypeId)
                    .Include(v => v.ProductType)
                    .FirstOrDefaultAsync();

                if (productVariant == null)
                {
                    continue;
                }

                var cartProduct = new CartProductResponse
                {
                    ProductId = product.Id,
                    Title = product.Title,
                    ImageUrl = product.ImageUrl,
                    Price = productVariant.Price,
                    ProductType = productVariant.ProductType.Name,
                    ProductTypeId = productVariant.ProductTypeId,
                    Quantity = item.Quantity,
                };

                result.Data.Add(cartProduct);
            }

            return result;
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> StoreCartItem(List<CartItem> cartItems)
        {
            var userId = GetUserId();

            foreach (var cartItem in cartItems)
            {
                cartItem.UserId = userId;

                var sameItem = await _dataContext.CartItems
                    .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId && ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == cartItem.UserId);

                if (sameItem == null)
                {
                    _dataContext.CartItems.Add(cartItem);
                }
                else
                {
                    sameItem.Quantity += cartItem.Quantity;
                }

            }

            await _dataContext.SaveChangesAsync();
            return await GetDbCartProducts();
        }

        public async Task<ServiceResponse<int>> GetCartItemsCount()
        {
            var count = (await _dataContext.CartItems.Where(ci => ci.UserId == GetUserId()).ToListAsync()).Count;

            return new ServiceResponse<int> { Data = count, };
        }

        public async Task<ServiceResponse<List<CartProductResponse>>> GetDbCartProducts()
        {
            return await GetCartProducts(await _dataContext.CartItems
                .Where(ci => ci.UserId == GetUserId()).ToListAsync());
        }

        public async Task<ServiceResponse<bool>> AddToCart(CartItem cartItem)
        {
            cartItem.UserId = GetUserId();

            var sameItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId && ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == cartItem.UserId);

            if (sameItem == null)
            {
                _dataContext.CartItems.Add(cartItem);
            }
            else
            {
                sameItem.Quantity += cartItem.Quantity;
            }

            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };
        }

        public async Task<ServiceResponse<bool>> UpdateQuantity(CartItem cartItem)
        {
            var dbCarItem = await _dataContext.CartItems
                .FirstOrDefaultAsync(ci => ci.ProductId == cartItem.ProductId && ci.ProductTypeId == cartItem.ProductTypeId && ci.UserId == GetUserId());

            if (dbCarItem == null)
            {
                return new ServiceResponse<bool>
                {
                    Data = false,
                    Message = "Cart item does not exist.",
                    Success = false,
                };
            }

            dbCarItem.Quantity = cartItem.Quantity;
            await _dataContext.SaveChangesAsync();

            return new ServiceResponse<bool> { Data = true };

        }
    }
}
