using ecomWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace ecomWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly MyDbContext _dbContext;

        public OrderController(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Place Order (Checkout)
        [HttpPost("PlaceOrder/{userId}/{addressId}")]
        public IActionResult PlaceOrder(int userId, int addressId)
        {
            var cartItems = _dbContext.Carts
                .Include(c => c.Product) // Fetch product details
                .Where(c => c.UserId == userId)
                .ToList();

            if (!cartItems.Any())
                return BadRequest("Cart is empty");

            var addressExists = _dbContext.Addresses.Any(a => a.AddressId == addressId && a.UserId == userId);
            if (!addressExists)
                return BadRequest("Invalid address");

            var order = new Order
            {
                UserId = userId,
                AddressId = addressId,
                TotalAmount = cartItems.Sum(c => c.TotalPrice),
                OrderItems = cartItems.Select(cartItem => new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.TotalPrice / cartItem.Quantity  // Price per unit at order time
                }).ToList()
            };

            _dbContext.Orders.Add(order);
            _dbContext.Carts.RemoveRange(cartItems);
            _dbContext.SaveChanges();

            // Convert Order to OrderDTO
            var orderDTO = new OrderDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                AddressId = order.AddressId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = _dbContext.Products.FirstOrDefault(p => p.ProductId == oi.ProductId)?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            };

            return Ok(new { message = "Order placed successfully", order = orderDTO });
        }

        [HttpGet("GetOrderById/{orderId}")]
        public async Task<IActionResult> GetOrderById(int orderId)
        {
            var order = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include related Product details
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound(new { message = "Order not found" });
            }

            return Ok(order);
        }

                [HttpGet("GetOrdersByUserId/{userId}")]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            var orders = await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include product details
                .OrderByDescending(o => o.OrderDate) // Order by latest orders first
                .ToListAsync();

            if (orders == null || !orders.Any())
                return NotFound(new { message = "No orders found for this user" });

            var orderDTOs = orders.Select(order => new OrderDTO
            {
                OrderId = order.OrderId,
                UserId = order.UserId,
                AddressId = order.AddressId,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                OrderDate = order.OrderDate,
                OrderItems = order.OrderItems.Select(oi => new OrderItemDTO
                {
                    ProductId = oi.ProductId,
                    ProductName = oi.Product?.Name ?? "Unknown",
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
            }).ToList();

            return Ok(orderDTOs);
        }


    }
}
