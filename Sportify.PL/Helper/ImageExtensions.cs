using System;
using System.Collections.Generic;
using System.Linq;

namespace Sportify
{
    public static class ImageExtensions
    {
        public static string GetFirstImageUrl(this string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return "/Images/no-image.png";

            var first = imageUrl.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return string.IsNullOrWhiteSpace(first) ? "/Images/no-image.png" : first;
        }

        public static List<string> GetImageUrls(this string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return new List<string> { "/Images/no-image.png" };

            var list = imageUrl.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
            return list.Any() ? list : new List<string> { "/Images/no-image.png" };
        }
    }
}
