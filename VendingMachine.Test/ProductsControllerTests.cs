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
        private DataContext context;
        private ProductsController productsController;
        private TransactionsController transactionsController;

        public ProductsControllerTests()
        {
            context = new DataContext();
            productsController = new ProductsController(context);
            transactionsController = new TransactionsController(context);
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
            var result = productsController.SellProduct(20) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Product not found", result.Value.ToString());
        }

        [Fact]
        public void CheckSellInvalidTransaction()
        {
            var result = productsController.SellProduct(1) as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Transaction not found", result.Value.ToString());
        }

        [Fact]
        public void CheckUsedTransaction()
        {
            var result = transactionsController.CreateTransaction("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var transaction = result.Value as Transaction;

            result = productsController.SellProduct(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var result2 = productsController.SellProduct(1) as BadRequestObjectResult;

            Assert.NotNull(result2);
            Assert.Equal("Invalid transaction - Transaction already processed", result2.Value.ToString());
        }

        [Fact]
        public void CheckNoStock()
        {
            context.Products[1].CurrentStock = 0;

            var result = transactionsController.CreateTransaction("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var transaction = result.Value as Transaction;

            var result2 = productsController.SellProduct(2) as BadRequestObjectResult;

            Assert.NotNull(result2);
            Assert.Equal("Out of stock", result2.Value.ToString());
        }

        [Fact]
        public void CheckSuccessfullSale()
        {
            var result = transactionsController.CreateTransaction("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var transaction = result.Value as Transaction;

            result = productsController.SellProduct(1) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var result2 = productsController.SellProduct(1) as BadRequestObjectResult;

            Assert.NotNull(result2);
            Assert.Equal("Invalid transaction - Transaction already processed", result2.Value.ToString());
        }

        [Fact]
        public void CheckRestockAll()
        {
            var result = productsController.RestockAll() as OkResult;

            Assert.NotNull(result);

            foreach (var product in context.Products)
            {
                Assert.Equal(product.StockLimit, product.CurrentStock);
            }
        }
    }
}
