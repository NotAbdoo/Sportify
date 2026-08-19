using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportify.Confiuration.Enums
{
    public enum OrderStatus
    {
        Pending,        // Order placed but not confirmed yet
        Confirmed,      // Payment confirmed / order accepted
        Processing,     // Preparing items
        Shipped,        // Sent to shipment
        Delivered,      // Customer received it
        Cancelled,      // Cancelled by user or system
        Refunded        // Money returned
    }
}
