using Microsoft.AspNetCore.Mvc;
using VendingMachine.Contexts;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    /// <summary>
    /// A controller for handling actions against the products.
    /// </summary>
    public class ProductsController : Controller
    {
        #region Variables

        /// <summary>
        /// The data context to get the data from.
        /// </summary>
        private readonly DataContext _data;

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor for the controller.
        /// </summary>
        /// <param name="data">The data context to use.</param>
        public ProductsController(DataContext data)
        {
            _data = data;
        }

        #endregion

        #region Functions

        /// <summary>
        /// A function to get all the products in the system.
        /// </summary>
        /// <returns>All the products in the system.</returns>
        [HttpGet]
        [Route("GetProducts")]
        public IActionResult GetProducts()
        {
            return Ok(_data.Products);
        }

        /// <summary>
        /// A function to sell a product.
        /// </summary>
        /// <param name="id">The id of thee product to sell.</param>
        /// <param name="transactionId">The id of the transaction to log the purchase against.</param>
        /// <returns>The update product.</returns>
        [HttpPost]
        [Route("SellProduct/{id}")]
        public IActionResult SellProduct(int id, [FromBody]int transactionId)
        {
            var product = _data.Products.FirstOrDefault(p => p.ID == id);

            if (product == null)
            {
                return BadRequest("Product not found");
            }

            var transaction = _data.Transactions.FirstOrDefault(t => t.ID == transactionId);

            if (transaction == null)
            {
                return BadRequest("Transaction not found");
            }

            if (transaction.ProductID.HasValue)
            {
                return BadRequest("Invalid transaction - Transaction already processed");
            }

            transaction.LastUpdatedDateTime = DateTime.Now;
            transaction.ProductID = product.ID;
            transaction.Amount = product.Price;

            if (product.CurrentStock == 0)
            {
                transaction.Status = TransactionStatus.Failed;
                return BadRequest("Out of stock");
            }

            try
            {
                ProcessPayment(transaction);
                transaction.Status = TransactionStatus.Success;
            }
            catch (Exception e)
            {
                transaction.Status = TransactionStatus.Failed;
                return BadRequest(e.Message);
            }

            product.CurrentStock--;

            return Ok(product);
        }

        /// <summary>
        /// A function to restock a product.
        /// </summary>
        /// <param name="id">The id of the product to restock.</param>
        /// <param name="quantity">The quantity to restock the product with.</param>
        /// <returns>The update product.</returns>
        [HttpPost]
        [Route("RestockProduct/{id}")]
        public IActionResult RestockProduct(int id, [FromBody]int? quantity = null)
        {
            var product = _data.Products.FirstOrDefault(p => p.ID == id);

            if (product == null)
            {
                return BadRequest("Product not found");
            }
            else
            {
                if (!quantity.HasValue || product.CurrentStock + quantity > product.StockLimit)
                {
                    product.CurrentStock = product.StockLimit;
                }
                else
                {
                    product.CurrentStock += quantity.Value;
                }

                return Ok(product);
            }
        }

        /// <summary>
        /// A function to restock all products.
        /// </summary>
        /// <returns>A success status.</returns>
        [HttpPost]
        [Route("RestockAll")]
        public IActionResult RestockAll()
        {
            foreach (var product in _data.Products)
            {
                product.CurrentStock = product.StockLimit;
            }

            return Ok();
        }

        #endregion

        #region Helper Functions

        /// <summary>
        /// A function to process the payment.
        /// </summary>
        /// <param name="transaction">The transaction to process.</param>
        /// <exception cref="Exception">The exception outlining thee details of any errors encountered.</exception>
        private void ProcessPayment(Transaction transaction)
        {
            if (transaction.PaymentType == "Card")
            {
                try
                {
                    // Process card payment with bank/financial institution.
                }
                catch
                {
                    // Log the true error here and return a user friendly one.
                    throw new Exception("Card payment failed");
                }
            }

            switch (transaction.PaymentType)
            {
                case "Cash":
                    _data.CashAmount += transaction.Amount;
                    break;
                case "Card":
                    _data.CardAmount += transaction.Amount;
                    break;
            }
        }

        #endregion
    }
}
