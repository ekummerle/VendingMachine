using Microsoft.AspNetCore.Mvc;
using VendingMachine.Contexts;
using VendingMachine.Controllers;
using VendingMachine.Models;

namespace VendingMachine.Test
{
    /// <summary>
    /// A class for handling tests against the products controller.
    /// </summary>
    public class ProductsControllerTests
    {
        private static DataContext context;
        private ProductsController productsController;

        public ProductsControllerTests()
        {
            context = new DataContext();
            productsController = new ProductsController(context);
        }

        [Fact]
        public void CheckGetProducts()
        {
            var result = productsController.GetProducts() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var products = result.Value as Product[];

            Assert.NotNull(products);
            Assert.Equal(10, products.Length);
        }

        [Fact]
        public void CheckRestockWhenFull()
        {
            var result = productsController.RestockProduct(1, 1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var product = result.Value as Product;

            Assert.NotNull(product);
            Assert.Equal(20, product.CurrentStock);
        }

        [Fact]
        public void CheckRestockInvalidProduct()
        {
            var result = productsController.RestockProduct(20) as BadRequestObjectResult;

            Assert.NotNull(result);
        }

        [Fact]
        public void CheckSellInvalidProduct()
        {
            var result = productsController.SellProduct(20, 1) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Product not found", result.Value.ToString());
        }

        [Fact]
        public void CheckSellInvalidTransaction()
        {
            var result = productsController.SellProduct(1, 1) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Transaction not found", result.Value.ToString());
        }

        [Fact]
        public void CheckUsedTransaction()
        {
            // TODO
        }

        [Fact]
        public void CheckNoStock()
        {
            // TODO
        }

        [Fact]
        public void CheckInvalidPaymentMethod()
        {
            // TODO
        }

        [Fact]
        public void CheckSuccessfullSale()
        {
            // TODO
        }

        [Fact]
        public void CheckRestockAll()
        {
            var result = productsController.RestockAll() as OkObjectResult;

            Assert.NotNull(result);

            foreach (var product in context.Products)
            {
                Assert.Equal(product.StockLimit, product.CurrentStock);
            }
        }
    }
}
