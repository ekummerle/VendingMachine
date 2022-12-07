namespace VendingMachine.Models
{
    /// <summary>
    /// A class to handle the transactions.
    /// </summary>
    public class Transaction
    {
        #region Properties

        /// <summary>
        /// The id of the transaction.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The id of the product purchased.
        /// Relates to the id of a product.
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// The type of the payment.
        /// Accepts string values of Cash or Card.
        /// There will need to be error handling for any other values provided.
        /// </summary>
        public string PaymentType { get; set; }

        /// <summary>
        /// The date and time that the transaction was created.
        /// This relates to when the money was inserted into the machine or when the card was tapped/swiped.
        /// </summary>
        public DateTime CreatedDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// The date and time that the transaction was last updated.
        /// This relates to when the status changes due to something going wrong, a transaction being cancelled, or the transaction being successfully completed.
        /// </summary>
        public DateTime LastUpdatedDateTime { get; set; } = DateTime.Now;

        /// <summary>
        /// The amount of the transaction.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The status of the transaction.
        /// </summary>
        public TransactionStatus Status { get; set; } = TransactionStatus.Created;

        #endregion
    }
}
