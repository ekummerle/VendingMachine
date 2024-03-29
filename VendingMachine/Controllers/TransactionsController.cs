﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using VendingMachine.Contexts;
using VendingMachine.Hubs;
using VendingMachine.Models;

namespace VendingMachine.Controllers
{
    /// <summary>
    /// A controller for handling actions against the transactions.
    /// </summary>
    public class TransactionsController : Controller
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
        /// The constructor for the class.
        /// </summary>
        /// <param name="data">The data context to use.</param>
        /// <param name="hub">The SignalR hub to use for updates to the frontend.</param>
        public TransactionsController(DataContext data, MessageHub hub)
        {
            _data = data;
            _hub = hub;
        }

        #endregion

        #region Functions

        /// <summary>
        /// A function to return all the payments in the system.
        /// </summary>
        /// <param name="paymentType">The type of payment to return. A null string means to return everything.</param>
        /// <returns>The list of payments with the given payment type.</returns>
        [HttpGet]
        [Route("GetPayments")]
        public IActionResult GetPayments(string paymentType)
        {
            return Ok(_data.Transactions.Where(t => string.IsNullOrEmpty(paymentType) || t.PaymentType == paymentType));
        }

        /// <summary>
        /// A function to get the active transaction.
        /// </summary>
        /// <returns>The active transaction if there is one, or null if there is not.</returns>
        [HttpGet]
        [Route("GetActiveTransaction")]
        public IActionResult GetActiveTransaction()
        {
            return Ok(_data.Transactions.FirstOrDefault(t => t.Status == TransactionStatus.Created));
        }

        /// <summary>
        /// A function to create a transaction.
        /// </summary>
        /// <param name="paymentType">The payment type to create the transaction for.</param>
        /// <returns>The created transaction.</returns>
        [HttpPost]
        [Route("CreateTransaction")]
        public IActionResult CreateTransaction([FromBody]string paymentType)
        {
            if (paymentType != "Cash" && paymentType != "Card")
            {
                return BadRequest("Invalid payment type");
            }

            var lastTrans = _data.Transactions.LastOrDefault();

            if (lastTrans != null && lastTrans.Status == TransactionStatus.Created)
            {
                return BadRequest("There is already an active transaction");
            }

            Transaction transaction = new()
            {
                // When using a DB this would be an auto incrementing id.
                ID = _data.Transactions.Count() + 1,
                PaymentType = paymentType
            };

            _data.Transactions.Add(transaction);

            return Ok(transaction);
        }

        /// <summary>
        /// A function to cancel a transaction.
        /// </summary>
        /// <returns>Thee updated transaction.</returns>
        [HttpPost]
        [Route("CancelTransaction")]
        public IActionResult CancelTransaction()
        {
            var transaction = _data.Transactions.LastOrDefault();

            if (transaction == null)
            {
                return BadRequest("Transaction not found");
            }
            // We can only cancel a transaction that is active, i.e. status == created.
            else if (transaction.Status != TransactionStatus.Created)
            {
                return BadRequest("Transaction not active");
            }
            else
            {
                transaction.LastUpdatedDateTime = DateTime.Now;
                transaction.Status = TransactionStatus.Cancelled;

                return Ok(transaction);
            }
        }

        /// <summary>
        /// A function to reset the transaction amounts.
        /// </summary>
        /// <returns>Returns success when the function completes.</returns>
        [HttpPost]
        [Route("ClearFunds")]
        public IActionResult ClearFunds()
        {
            _data.CardAmount = 0;
            _data.CashAmount = 0;

            _hub.Clients.All?.SendAsync("CashAmountUpdated", _data.CashAmount);
            _hub.Clients.All?.SendAsync("CardAmountUpdated", _data.CardAmount);
            _hub.Clients.All?.SendAsync("TotalAmountUpdated", _data.CashAmount + _data.CardAmount);

            return Ok();
        }

        /// <summary>
        /// A function to get the payment amount for a given payment type.
        /// </summary>
        /// <param name="paymentType">The payment type to return the payment amount for. A null or empty string will return all.</param>
        /// <returns>The total amount for the given payment type.</returns>
        [HttpGet]
        [Route("GetPaymentAmount")]
        public IActionResult GetPaymentAmount(string paymentType)
        {
            if (string.IsNullOrEmpty(paymentType))
            {
                return Ok(_data.CardAmount + _data.CashAmount);
            }
            else
            {
                switch (paymentType)
                {
                    case "Cash":
                        return Ok(_data.CashAmount);
                    case "Card":
                        return Ok(_data.CardAmount);
                    default:
                        return BadRequest("Invalid payment type");
                }
            }
        }

        #endregion
    }
}
