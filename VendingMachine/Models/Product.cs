namespace VendingMachine.Models
{
    /// <summary>
    /// A class for handling the products in the system.
    /// </summary>
    public class Product
    {
        #region Properties

        /// <summary>
        /// The id for the product.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// The name of the product.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The image for the product.
        /// </summary>
        public string Image { get; set; }

        /// <summary>
        /// The stock limit for the product.
        /// Defaults to 20.
        /// </summary>
        public int StockLimit { get; set; } = 20;

        /// <summary>
        /// The current stock level for the product.
        /// Defaults to 20 (full stock).
        /// </summary>
        public int CurrentStock { get; set; } = 20;

        /// <summary>
        /// The price for an individual unit of this product.
        /// Defaults to $2.40.
        /// </summary>
        public decimal Price { get; set; } = 2.40m;

        #endregion
    }
}
