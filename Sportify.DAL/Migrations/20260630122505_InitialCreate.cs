using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Sportify.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Brands",
                columns: table => new
                {
                    BrandID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    LogoURL = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Brands", x => x.BrandID);
                });

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParentCategoryID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.CategoryID);
                    table.ForeignKey(
                        name: "FK_Categories_Categories_ParentCategoryID",
                        column: x => x.ParentCategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserID);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ImageURL = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false),
                    BrandID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductID);
                    table.ForeignKey(
                        name: "FK_Products_Brands_BrandID",
                        column: x => x.BrandID,
                        principalTable: "Brands",
                        principalColumn: "BrandID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryID",
                        column: x => x.CategoryID,
                        principalTable: "Categories",
                        principalColumn: "CategoryID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Carts",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carts", x => x.CartID);
                    table.ForeignKey(
                        name: "FK_Carts_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ShippingAddresses",
                columns: table => new
                {
                    ShippingAddressId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    AlternativePhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Apartment = table.Column<byte>(type: "tinyint", nullable: false),
                    Floor = table.Column<byte>(type: "tinyint", nullable: false),
                    Area = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Building = table.Column<byte>(type: "tinyint", nullable: false),
                    Governorate = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingAddresses", x => x.ShippingAddressId);
                    table.ForeignKey(
                        name: "FK_ShippingAddresses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductReviews",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false),
                    ReviewID = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<decimal>(type: "DECIMAL(2,1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductReviews", x => new { x.UserID, x.ProductID });
                    table.ForeignKey(
                        name: "FK_ProductReviews_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductReviews_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductVariants",
                columns: table => new
                {
                    ProductVariantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Price = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Size = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    ProductID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariants", x => x.ProductVariantId);
                    table.ForeignKey(
                        name: "FK_ProductVariants_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Note = table.Column<string>(type: "TEXT", nullable: true),
                    FastShiping = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PaymentAmount = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShipmentTrackingNumber = table.Column<int>(type: "int", nullable: false),
                    ShipmentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    ShippingAddressID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderId);
                    table.ForeignKey(
                        name: "FK_Orders_ShippingAddresses_ShippingAddressID",
                        column: x => x.ShippingAddressID,
                        principalTable: "ShippingAddresses",
                        principalColumn: "ShippingAddressId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CartItems",
                columns: table => new
                {
                    CartID = table.Column<int>(type: "int", nullable: false),
                    ProductVariantId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartItems", x => new { x.CartID, x.ProductVariantId });
                    table.ForeignKey(
                        name: "FK_CartItems_Carts_CartID",
                        column: x => x.CartID,
                        principalTable: "Carts",
                        principalColumn: "CartID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartItems_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItems",
                columns: table => new
                {
                    OrderID = table.Column<int>(type: "int", nullable: false),
                    ProductVariantID = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItems", x => new { x.OrderID, x.ProductVariantID });
                    table.ForeignKey(
                        name: "FK_OrderItems_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderItems_ProductVariants_ProductVariantID",
                        column: x => x.ProductVariantID,
                        principalTable: "ProductVariants",
                        principalColumn: "ProductVariantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderReview",
                columns: table => new
                {
                    OrderReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Rating = table.Column<decimal>(type: "DECIMAL(2,1)", nullable: false),
                    TheWantedOrder = table.Column<bool>(type: "bit", nullable: false),
                    OnTime = table.Column<bool>(type: "bit", nullable: false),
                    GoodStatus = table.Column<bool>(type: "bit", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false),
                    OrderID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderReview", x => x.OrderReviewId);
                    table.ForeignKey(
                        name: "FK_OrderReview_Orders_OrderID",
                        column: x => x.OrderID,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "BrandID", "CreatedBy", "Description", "LogoURL", "Name" },
                values: new object[,]
                {
                    { 1, "Admin", "The world’s largest sportswear brand", "https://tse4.mm.bing.net/th/id/OIP.xRP_6PtMGwc6UGslxaK4YAHaEK?rs=1&pid=ImgDetMain&o=7&rm=3", "Nike" },
                    { 2, "Admin", "German sportswear company.", "https://tse3.mm.bing.net/th/id/OIP.aw4ynosen6elgMpjUjaUBwHaEK?rs=1&pid=ImgDetMain&o=7&rm=3", "Adidas" },
                    { 3, "Admin", "German sportswear company.", "https://tse2.mm.bing.net/th/id/OIP.0z2kSI_ehJizOeLLUL77dQHaEK?rs=1&pid=ImgDetMain&o=7&rm=3", "Puma" },
                    { 4, "Admin", "Sportswear", "https://tse3.mm.bing.net/th/id/OIP.tASbMfiMM2xaVjJfhDk3QgHaEK?rs=1&pid=ImgDetMain&o=7&rm=3", "Jordan" },
                    { 5, "Admin", "Electronics manufacturer.", "https://tse3.mm.bing.net/th/id/OIP.w4RV1Nk4yYTvavsPKmXknwHaEK?rs=1&pid=ImgDetMain&o=7&rm=3", "Sony" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "Description", "Name", "ParentCategoryID" },
                values: new object[,]
                {
                    { 1, "All types items for men", "Men", null },
                    { 2, "All types of items for women", "Women's", null },
                    { 3, "Equipments for sports", "Equipments", null },
                    { 4, "Sports", "Sports", null }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserID", "Address", "CreatedAt", "Email", "FirstName", "LastName", "PasswordHash", "Phone", "Role" },
                values: new object[,]
                {
                    { 1, "Bns", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "hager.mahmoud@email.com", "Hager", "Mahmoud", "hashed_pw_1", "01234567891", "Admin" },
                    { 2, "Bns", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "abdelrhman.salah@email.com", "Abdelrhman", "Salah", "hashed_pw_2", "01234567891", "Admin" },
                    { 3, "Bns", new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "osama.tarek@email.com", "Osama", "Tarek", "hashed_pw_3", "01234567891", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "CartID", "UserID" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "CategoryID", "Description", "Name", "ParentCategoryID" },
                values: new object[,]
                {
                    { 5, "Clothing for men.", "Men's Clothing", 1 },
                    { 6, "Footwear for men.", "Men's Footwear", 1 },
                    { 7, "Clothing for women.", "Women's Clothing", 2 },
                    { 8, "Footwear for women.", "Women's Footwear", 2 },
                    { 9, "Earphones and  others", "Headwears", 3 },
                    { 10, "Things like handgrips", "Others", 3 },
                    { 11, "FootBall items", "FootBall", 4 },
                    { 12, "BasketBall items", "BasketBall", 4 },
                    { 13, "Tennis items", "Tennis", 4 },
                    { 14, "Running items", "Running", 4 },
                    { 15, "Gym items", "Gym", 4 }
                });

            migrationBuilder.InsertData(
                table: "ShippingAddresses",
                columns: new[] { "ShippingAddressId", "AlternativePhone", "Apartment", "Area", "Building", "City", "Country", "Floor", "Governorate", "Notes", "Phone", "Street", "UserID" },
                values: new object[,]
                {
                    { 1, "01334567890", (byte)10, "Sidi Gaber", (byte)22, "Al-Wasta", "Egypt", (byte)4, "BeniSuef", null, "01234567891", "Corniche", 1 },
                    { 2, "01198765432", (byte)8, "City Center", (byte)7, "Al-Fashn", "Egypt", (byte)3, "BeniSuef", null, "01234567891", "Main St", 2 }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "OrderId", "CreatedAt", "FastShiping", "Note", "PaidAt", "PaymentAmount", "PaymentMethod", "ShipmentStatus", "ShipmentTrackingNumber", "ShippingAddressID", "Status", "UserID" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Leave at door.", new DateTime(2026, 3, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), 3600.00m, "Card", "Delivered", 321, 1, "Delivered", 1 },
                    { 2, new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Call before delivery.", new DateTime(2026, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2200.00m, "Cash", "Cancelled", 123, 2, "Pending", 2 }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "ProductID", "BrandID", "CategoryID", "CreatedAt", "CreatedBy", "Description", "ImageURL", "Name" },
                values: new object[,]
                {
                    { 1, 1, 6, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Iconic Air Max sneakers.", "https://cdn.example.com/airmax.png", "Nike Air Max" },
                    { 2, 2, 6, new DateTime(2024, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "High performance running shoe.", "https://cdn.example.com/ultraboost.png", "Adidas Ultraboost" },
                    { 3, 3, 5, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Casual slim-fit cotton shirt.", "https://cdn.example.com/shirt.png", "Puma Slim Fit Shirt" },
                    { 4, 5, 9, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Latest Sony flagship speakers.", "https://cdn.example.com/s24.png", "Sony Headphones" },
                    { 5, 4, 5, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Iconic Jordan Shorts.", "https://cdn.example.com/iphone15.png", "Jordan Shorts" },
                    { 6, 2, 10, new DateTime(2024, 2, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "Adidas football", "https://cdn.example.com/iphone15.png", "Adidas ball" }
                });

            migrationBuilder.InsertData(
                table: "OrderReview",
                columns: new[] { "OrderReviewId", "Comment", "GoodStatus", "OnTime", "OrderID", "Rating", "TheWantedOrder" },
                values: new object[,]
                {
                    { 1, "Great experience, fast delivery!", true, true, 1, 4.5m, true },
                    { 2, "Items were correct but arrived late.", true, false, 2, 3.0m, true }
                });

            migrationBuilder.InsertData(
                table: "ProductReviews",
                columns: new[] { "ProductID", "UserID", "Comment", "CreatedAt", "Rating", "ReviewID" },
                values: new object[,]
                {
                    { 1, 1, "Best sneakers I've ever bought!", new DateTime(2024, 3, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.0m, 1 },
                    { 4, 1, "Great phone, amazing camera quality.", new DateTime(2024, 3, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 4.5m, 3 },
                    { 2, 2, "Very comfortable for long runs.", new DateTime(2024, 3, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 4.0m, 2 },
                    { 5, 2, "Incredibly fast laptop, worth every EGP.", new DateTime(2024, 4, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 5.0m, 5 },
                    { 6, 2, "Nice dress but sizing runs small.", new DateTime(2024, 4, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 3.5m, 4 }
                });

            migrationBuilder.InsertData(
                table: "ProductVariants",
                columns: new[] { "ProductVariantId", "Color", "Price", "ProductID", "SKU", "Size", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "White", 1800.00m, 1, "NK-AM-WHT-42", "42", 50 },
                    { 2, "Black", 1800.00m, 1, "NK-AM-BLK-43", "43", 30 },
                    { 3, "Grey", 2200.00m, 2, "AD-UB-GRY-42", "42", 40 },
                    { 4, "Navy", 2200.00m, 2, "AD-UB-NVY-44", "44", 25 },
                    { 5, "White", 350.00m, 3, "PM-SH-WHT-M", "M", 100 },
                    { 6, "Blue", 350.00m, 3, "PM-SH-BLU-L", "L", 80 },
                    { 7, "Phantom Black", 2500.00m, 4, "SH-H200-BLK-256", "N/A", 20 },
                    { 8, "Cream", 2700.00m, 4, "SH-H200-CRM-512", "N/A", 15 },
                    { 9, "Pink", 1800.00m, 5, "JD-SHRT-PNK-128", "L", 25 },
                    { 10, "Black", 1800.00m, 5, "JD-SHRT-BLK-256", "XL", 10 },
                    { 11, "Blue", 2000.00m, 6, "AD-BL-BLU-WC", "N/A", 20 },
                    { 12, "Purple", 2000.00m, 6, "AD-BL-PRPL-WC", "N/A", 15 }
                });

            migrationBuilder.InsertData(
                table: "OrderItems",
                columns: new[] { "OrderID", "ProductVariantID", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 2 },
                    { 1, 4, 1 },
                    { 1, 5, 1 },
                    { 2, 1, 1 },
                    { 2, 2, 1 },
                    { 2, 3, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartItems_ProductVariantId",
                table: "CartItems",
                column: "ProductVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Carts_UserID",
                table: "Carts",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ParentCategoryID",
                table: "Categories",
                column: "ParentCategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItems_ProductVariantID",
                table: "OrderItems",
                column: "ProductVariantID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderReview_OrderID",
                table: "OrderReview",
                column: "OrderID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippingAddressID",
                table: "Orders",
                column: "ShippingAddressID");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserID",
                table: "Orders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductID",
                table: "ProductReviews",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_BrandID",
                table: "Products",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryID",
                table: "Products",
                column: "CategoryID");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariants_ProductID",
                table: "ProductVariants",
                column: "ProductID");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingAddresses_UserID",
                table: "ShippingAddresses",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartItems");

            migrationBuilder.DropTable(
                name: "OrderItems");

            migrationBuilder.DropTable(
                name: "OrderReview");

            migrationBuilder.DropTable(
                name: "ProductReviews");

            migrationBuilder.DropTable(
                name: "Carts");

            migrationBuilder.DropTable(
                name: "ProductVariants");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "ShippingAddresses");

            migrationBuilder.DropTable(
                name: "Brands");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
