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
                ID = 10,
                Amount = 2.40m,
                PaymentType = "Cash",
                ProductID = 1,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 11,
                Amount = 2.40m,
                PaymentType = "Cash",
                ProductID = 2,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 12,
                Amount = 2.40m,
                PaymentType = "Cash",
                ProductID = 1,
                Status = TransactionStatus.Failed
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 13,
                PaymentType = "Cash",
                Status = TransactionStatus.Cancelled
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 14,
                PaymentType = "Cash",
                Status = TransactionStatus.Created
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 15,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 1,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 16,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 2,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 17,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 3,
                Status = TransactionStatus.Success
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 18,
                PaymentType = "Card",
                Status = TransactionStatus.Cancelled
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 19,
                Amount = 2.40m,
                PaymentType = "Card",
                ProductID = 1,
                Status = TransactionStatus.Failed
            });

            context.Transactions.Add(new Transaction()
            {
                ID = 20,
                PaymentType = "Card",
                Status = TransactionStatus.Created
            });

            context.CardAmount = 0;
            context.CashAmount = 0;
        }

        [Fact]
        public void CheckGetCashPayments()
        {
            var result = transactionsController.GetPayments("Cash") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var cashPayments = result.Value as IEnumerable<Transaction>;

            Assert.NotNull(cashPayments);
            Assert.Equal(5, cashPayments.Count());
        }

        [Fact]
        public void CheckGetCardPayments()
        {
            var result = transactionsController.GetPayments("Card") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var cashPayments = result.Value as IEnumerable<Transaction>;

            Assert.NotNull(cashPayments);
            Assert.Equal(6, cashPayments.Count());
        }

        [Fact]
        public void CheckGetAllPayments()
        {
            var result = transactionsController.GetPayments(null) as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var cashPayments = result.Value as IEnumerable<Transaction>;

            Assert.NotNull(cashPayments);
            Assert.Equal(5, cashPayments.Count());
        }

        // TODO write the rest of the tests.
    }
}
