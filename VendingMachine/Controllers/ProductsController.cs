﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VendingMachine.Contexts;
using VendingMachine.Hubs;
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

        /// <summary>
        /// The SignalR hub to send messages through.
        /// </summary>
        private readonly MessageHub _hub;

        #endregion

        #region Constructor

        /// <summary>
        /// The constructor for the controller.
        /// </summary>
        /// <param name="data">The data context to use.</param>
        /// <param name="hub">The SignalR hub to use for updates to the frontend.</param>
        public ProductsController(DataContext data, MessageHub hub)
        {
            _data = data;
            _hub = hub;
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
        public IActionResult SellProduct(int id)
        {
            var product = _data.Products.FirstOrDefault(p => p.ID == id);

            if (product == null)
            {
                return BadRequest("Product not found");
            }

            var transaction = _data.Transactions.LastOrDefault();

            if (transaction == null)
            {
                return BadRequest("Transaction not found");
            }

            if (transaction.Status != TransactionStatus.Created)
            {
                return BadRequest("Invalid transaction - Transaction already processed");
            }

            transaction.LastUpdatedDateTime = DateTime.Now;
            transaction.ProductID = product.ID;

            if (product.CurrentStock == 0)
            {
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
                    _hub.Clients.All?.SendAsync("CashAmountUpdated", _data.CashAmount);
                    _hub.Clients.All?.SendAsync("TotalAmountUpdated", _data.CashAmount + _data.CardAmount);
                    break;
                case "Card":
                    _data.CardAmount += transaction.Amount;
                    _hub.Clients.All?.SendAsync("CardAmountUpdated", _data.CardAmount);
                    _hub.Clients.All?.SendAsync("TotalAmountUpdated", _data.CashAmount + _data.CardAmount);
                    break;
            }
        }

        #endregion
    }
}
