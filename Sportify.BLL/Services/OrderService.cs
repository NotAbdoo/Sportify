using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _db;

        public OrderService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<OrderStatsResult> GetOrderStatsAsync()
        {
            var result = new OrderStatsResult();

            result.TotalOrders = await _db.Orders.CountAsync();

            var now = DateTime.UtcNow;
            var thisMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = thisMonthStart.AddMonths(-1);

            result.TotalRevenue = await _db.Orders
                .Where(o => o.Status != Sportify.Confiuration.Enums.OrderStatus.Cancelled
                         && o.Status != Sportify.Confiuration.Enums.OrderStatus.Refunded)
                .SumAsync(o => (decimal?)o.PaymentAmount) ?? 0m;

            result.ThisMonthRevenue = await _db.Orders
                .Where(o => o.CreatedAt >= thisMonthStart
                         && o.Status != Sportify.Confiuration.Enums.OrderStatus.Cancelled
                         && o.Status != Sportify.Confiuration.Enums.OrderStatus.Refunded)
                .SumAsync(o => (decimal?)o.PaymentAmount) ?? 0m;

            result.LastMonthRevenue = await _db.Orders
                .Where(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < thisMonthStart
                         && o.Status != Sportify.Confiuration.Enums.OrderStatus.Cancelled
                         && o.Status != Sportify.Confiuration.Enums.OrderStatus.Refunded)
                .SumAsync(o => (decimal?)o.PaymentAmount) ?? 0m;

            result.ThisMonthOrders = await _db.Orders
                .CountAsync(o => o.CreatedAt >= thisMonthStart);

            result.LastMonthOrders = await _db.Orders
                .CountAsync(o => o.CreatedAt >= lastMonthStart && o.CreatedAt < thisMonthStart);

            // Monthly chart data (last 7 months)
            var sevenMonthsAgo = thisMonthStart.AddMonths(-6);
            var monthlyOrders = await _db.Orders
                .Where(o => o.CreatedAt >= sevenMonthsAgo)
                .GroupBy(o => new { o.CreatedAt.Year, o.CreatedAt.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count(), Revenue = g.Sum(o => o.PaymentAmount) })
                .ToListAsync();

            for (int i = 6; i >= 0; i--)
            {
                var m = thisMonthStart.AddMonths(-i);
                var entry = monthlyOrders.FirstOrDefault(x => x.Year == m.Year && x.Month == m.Month);
                
                result.MonthlyChart.Add(new MonthlyChartData
                {
                    Label = m.ToString("MMM yyyy"),
                    Count = entry?.Count ?? 0,
                    Revenue = entry?.Revenue ?? 0m
                });
            }

            return result;
        }

        public async Task<List<Order>> GetRecentOrdersAsync(int count)
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .OrderByDescending(o => o.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task CreateOrderAsync(Order order)
        {
            order.CreatedAt = DateTime.UtcNow;
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateOrderStatusAsync(int orderId, Sportify.Confiuration.Enums.OrderStatus status)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> IsVariantInAnyOrderAsync(int productVariantId)
        {
            return await _db.OrderItems.AnyAsync(oi => oi.ProductVariantID == productVariantId);
        }

        public async Task<bool> AreVariantsInAnyOrderAsync(List<int> productVariantIds)
        {
            if (productVariantIds == null || !productVariantIds.Any())
                return false;

            return await _db.OrderItems.AnyAsync(oi => productVariantIds.Contains(oi.ProductVariantID));
        }

        public async Task<ShippingAddress> CreateShippingAddressAsync(ShippingAddress address)
        {
            _db.ShippingAddresses.Add(address);
            await _db.SaveChangesAsync();
            return address;
        }

        public async Task<Order> PlaceOrderAsync(int userId, int shippingAddressId, string? note, bool fastShipping, string paymentMethod)
        {
            var cartItems = await _db.CartItems
                .Include(ci => ci.ProductVariant)
                .Where(ci => ci.Cart.UserID == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            decimal subtotal = cartItems.Sum(item => item.ProductVariant.Price * item.Quantity);
            decimal shipping = (subtotal > 0 && subtotal < 100) ? 10 : 0;
            decimal total = subtotal + shipping;

            var order = new Order
            {
                UserID = userId,
                ShippingAddressID = shippingAddressId,
                Note = note,
                FastShiping = fastShipping,
                Status = Sportify.Confiuration.Enums.OrderStatus.Pending,
                PaymentAmount = total,
                PaymentMethod = paymentMethod,
                ShipmentStatus = Sportify.Confiuration.Enums.ShippingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in cartItems)
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductVariantID = item.ProductVariantId,
                    Quantity = item.Quantity
                });

                item.ProductVariant.StockQuantity -= item.Quantity;
            }

            _db.Orders.Add(order);
            _db.CartItems.RemoveRange(cartItems);
            await _db.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                        .ThenInclude(pv => pv.Product)
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> CancelOrderAsync(int orderId, int userId)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserID == userId);

            if (order == null || order.Status != Sportify.Confiuration.Enums.OrderStatus.Pending)
                return false;

            order.Status = Sportify.Confiuration.Enums.OrderStatus.Cancelled;
            order.ShipmentStatus = Sportify.Confiuration.Enums.ShippingStatus.Cancelled;

            foreach (var item in order.OrderItems)
            {
                if (item.ProductVariant != null)
                {
                    item.ProductVariant.StockQuantity += item.Quantity;
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AdminCancelOrderAsync(int orderId)
        {
            var order = await _db.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.ProductVariant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null || order.Status != Sportify.Confiuration.Enums.OrderStatus.Pending)
                return false;

            order.Status = Sportify.Confiuration.Enums.OrderStatus.Cancelled;
            order.ShipmentStatus = Sportify.Confiuration.Enums.ShippingStatus.Cancelled;

            foreach (var item in order.OrderItems)
            {
                if (item.ProductVariant != null)
                {
                    item.ProductVariant.StockQuantity += item.Quantity;
                }
            }

            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateOrderPaidStatusAsync(int orderId, bool isPaid)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order == null) return false;

            order.PaidAt = isPaid ? DateTime.UtcNow : null;
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
