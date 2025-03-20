using ecomWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ecomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public CartController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Add Item to Cart
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(CartDTO cartDTO)
        {
            var product = _dbContext.Products.FirstOrDefault(p => p.ProductId == cartDTO.ProductId);
            if (product == null)
                return NotFound("Product not found");

            var cartItem = _dbContext.Carts.FirstOrDefault(c => c.UserId == cartDTO.UserId && c.ProductId == cartDTO.ProductId);

            if (cartItem == null)
            {
                cartItem = new Cart
                {
                    UserId = cartDTO.UserId,
                    ProductId = cartDTO.ProductId,
                    Quantity = cartDTO.Quantity,
                    TotalPrice = (decimal)(cartDTO.Quantity * product.Price)
                };
                _dbContext.Carts.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += cartDTO.Quantity;
                cartItem.TotalPrice = (decimal)(cartItem.Quantity * product.Price);
                _dbContext.Carts.Update(cartItem);
            }

            _dbContext.SaveChanges();

            return Ok(new { message = "Product added to cart successfully", cartItem });
        }


        // Get Cart Items for a User
        [HttpGet("GetCart/{userId}")]
        public IActionResult GetCart(int userId)
        {
            var cartItems = _dbContext.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
                return NotFound("Cart is empty");

            return Ok(cartItems);
        }

        // Remove Item from Cart
        [HttpDelete("RemoveFromCart/{cartId}")]
        public IActionResult RemoveFromCart(int cartId)
        {
            var cartItem = _dbContext.Carts.FirstOrDefault(c => c.CartId == cartId);
            if (cartItem == null)
                return NotFound("Item not found in cart");

            _dbContext.Carts.Remove(cartItem);
            _dbContext.SaveChanges();

            return Ok(new { message = "Item removed from cart successfully" });
        }

        // Update Cart Item Quantity
        [HttpPut("UpdateCart")]
        public IActionResult UpdateCart(CartDTO cartDTO)
        {
            var cartItem = _dbContext.Carts.FirstOrDefault(c => c.UserId == cartDTO.UserId && c.ProductId == cartDTO.ProductId);
            if (cartItem == null)
                return NotFound("Cart item not found");

            cartItem.Quantity = cartDTO.Quantity;
            cartItem.TotalPrice = (decimal)(cartDTO.Quantity * _dbContext.Products.FirstOrDefault(p => p.ProductId == cartDTO.ProductId)!.Price);

            _dbContext.Carts.Update(cartItem);
            _dbContext.SaveChanges();

            return Ok(new { message = "Cart item updated successfully" });
        }

        // Clear Cart
        [HttpDelete("ClearCart/{userId}")]
        public IActionResult ClearCart(int userId)
        {
            var cartItems = _dbContext.Carts.Where(c => c.UserId == userId).ToList();
            if (!cartItems.Any())
                return NotFound("Cart is already empty");

            _dbContext.Carts.RemoveRange(cartItems);
            _dbContext.SaveChanges();

            return Ok("Cart cleared successfully");
        }
    }
}
