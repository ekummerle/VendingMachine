using Microsoft.AspNetCore.Mvc;
using VendingMachine.Contexts;
using VendingMachine.Controllers;
using VendingMachine.Models;

namespace VendingMachine.Test
{
    /// <summary>
    /// A class for handling tests against the transactions controller.
    /// </summary>
    public class TransactionControllerTests
    {
        private DataContext context;
        private TransactionsController transactionsController;

        public TransactionControllerTests()
        {
            context = new DataContext();
            transactionsController = new TransactionsController(context);

            context.Transactions.Add(new Transaction()
            {
                ID = 1,
                Amount = 2.40m,
                PaymentType = "Cash",
                ProductID = 1,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 2,
                Amount = 2.40m,
                PaymentType = "Cash",
                ProductID = 2,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 3,
                Amount = 2.40m,
                PaymentType = "Cash",
                ProductID = 1,
                Status = TransactionStatus.Failed
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 4,
                PaymentType = "Cash",
                Status = TransactionStatus.Cancelled
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 5,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 1,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 6,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 2,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 7,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 3,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 8,
                PaymentType = "Card",
                Status = TransactionStatus.Cancelled
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 9,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 1,
                Status = TransactionStatus.Failed
            });

            context.CardAmount = 24.00m;
            context.CashAmount = 12.00m;
        }

        [Fact]
        public void CheckGetCashPayments()
        {
            var result = transactionsController.GetPayments("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var cashPayments = result.Value as IEnumerable<Transaction>;

            Assert.NotNull(cashPayments);
            Assert.Equal(4, cashPayments.Count());
        }

        [Fact]
        public void CheckGetCardPayments()
        {
            var result = transactionsController.GetPayments("Card") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var cashPayments = result.Value as IEnumerable<Transaction>;

            Assert.NotNull(cashPayments);
            Assert.Equal(5, cashPayments.Count());
        }

        [Fact]
        public void CheckGetAllPayments()
        {
            var result = transactionsController.GetPayments(null) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var cashPayments = result.Value as IEnumerable<Transaction>;

            Assert.NotNull(cashPayments);
            Assert.Equal(9, cashPayments.Count());
        }

        [Fact]
        public void CheckInvalidPaymentType()
        {
            var result = transactionsController.CreateTransaction("AfterPay") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid payment type", result.Value.ToString());
        }

        [Fact]
        public void CheckActiveTransaction()
        {
            var result = transactionsController.CreateTransaction("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var result2 = transactionsController.CreateTransaction("Cash") as BadRequestObjectResult;

            Assert.NotNull(result2);
            Assert.Equal("There is already an active transaction", result2.Value.ToString());
        }

        [Fact]
        public void CheckValidTransaction()
        {
            var result = transactionsController.CreateTransaction("Card") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var transaction = result.Value as Transaction;

            Assert.Equal(context.Transactions.Count(), transaction.ID);
            Assert.Equal("Card", transaction.PaymentType);
        }

        [Fact]
        public void CheckInvalidTransaction()
        {
            context.Transactions.Clear();

            var result = transactionsController.CancelTransaction() as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Transaction not found", result.Value.ToString());
        }

        [Fact]
        public void CheckInactiveCancellation()
        {
            var result = transactionsController.CancelTransaction() as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Transaction not active", result.Value.ToString());
        }

        [Fact]
        public void CheckCancellation()
        {
            var result = transactionsController.CreateTransaction("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var result2 = transactionsController.CancelTransaction() as OkObjectResult;

            Assert.NotNull(result2);
            Assert.Equal(200, result2.StatusCode);

            var transaction = result2.Value as Transaction;

            Assert.NotNull(transaction);
            Assert.Equal(10, transaction.ID);
        }

        [Fact]
        public void CheckClearFunds()
        {
            var result = transactionsController.ClearFunds() as OkResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            Assert.Equal(0, context.CardAmount);
            Assert.Equal(0, context.CashAmount);
        }

        [Fact]
        public void CheckGetCashPaymentAmount()
        {
            var result = transactionsController.GetPaymentAmount("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var amount = (decimal?)result.Value;

            Assert.NotNull(amount);
            Assert.Equal(12.00m, amount);
        }

        [Fact]
        public void CheckGetCardPaymentAmount()
        {
            var result = transactionsController.GetPaymentAmount("Card") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var amount = (decimal?)result.Value;

            Assert.NotNull(amount);
            Assert.Equal(24.00m, amount);
        }

        [Fact]
        public void CheckGetTotalPaymentAmountEmptyString()
        {
            var result = transactionsController.GetPaymentAmount("") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var amount = (decimal?)result.Value;

            Assert.NotNull(amount);
            Assert.Equal(36.00m, amount);
        }

        [Fact]
        public void CheckGetTotalPaymentAmountNull()
        {
            var result = transactionsController.GetPaymentAmount(null) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var amount = (decimal?)result.Value;

            Assert.NotNull(amount);
            Assert.Equal(36.00m, amount);
        }

        [Fact]
        public void CheckGetInvalidPaymentAmount()
        {
            var result = transactionsController.GetPaymentAmount("AfterPay") as BadRequestObjectResult;

            Assert.NotNull(result);
            Assert.Equal("Invalid payment type", result.Value.ToString());
        }
    }
}
