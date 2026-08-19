using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sportify.Models;

namespace Sportify.BLL.Services
{
    public class OrderStatsResult
    {
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal ThisMonthRevenue { get; set; }
        public decimal LastMonthRevenue { get; set; }
        public int ThisMonthOrders { get; set; }
        public int LastMonthOrders { get; set; }
        public List<MonthlyChartData> MonthlyChart { get; set; } = new List<MonthlyChartData>();
    }

    public class MonthlyChartData
    {
        public string Label { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
    }

    public interface IOrderService
    {
        Task<OrderStatsResult> GetOrderStatsAsync();
        Task<List<Order>> GetRecentOrdersAsync(int count);
        Task<List<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task CreateOrderAsync(Order order);
        Task UpdateOrderStatusAsync(int orderId, Sportify.Confiuration.Enums.OrderStatus status);
        Task<bool> IsVariantInAnyOrderAsync(int productVariantId);
        Task<bool> AreVariantsInAnyOrderAsync(List<int> productVariantIds);
        Task<ShippingAddress> CreateShippingAddressAsync(ShippingAddress address);
        Task<Order> PlaceOrderAsync(int userId, int shippingAddressId, string? note, bool fastShipping, string paymentMethod);
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<bool> CancelOrderAsync(int orderId, int userId);
        Task<bool> AdminCancelOrderAsync(int orderId);
        Task<bool> UpdateOrderPaidStatusAsync(int orderId, bool isPaid);
    }
}
