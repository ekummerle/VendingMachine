using VendingMachine.Models;

namespace VendingMachine.Contexts
{
    public class DataContext
    {
        #region Properties

        /// <summary>
        /// The list of products available.
        /// </summary>
        public Product[] Products { get; private set; } = new Product[10];

        /// <summary>
        /// The list of transactions in the system.
        /// </summary>
        public List<Transaction> Transactions { get; private set; } = new List<Transaction>();

        /// <summary>
        /// The total amount of cash payments in the system.
        /// </summary>
        public decimal CashAmount { get; set; } = 0;

        /// <summary>
        /// The total amount of card payments in the system.
        /// </summary>
        public decimal CardAmount { get; set; } = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// The default constructor for the class.
        /// </summary>
        public DataContext()
        {
            InitialiseProducts();
        }

        #endregion

        #region Functions

        /// <summary>
        /// A function to initialise the default list of products for the system.
        /// </summary>
        private void InitialiseProducts()
        {
            Products[0] = new Product()
            {
                ID = 1,
                Name = "Cola",
                Image = "#873E23"
            };

            Products[1] = new Product()
            {
                ID = 2,
                Name = "Raspberry",
                Image = "#D1281F"
            };

            Products[2] = new Product()
            {
                ID = 3,
                Name = "Orange",
                Image = "#F77F07"
            };

            Products[3] = new Product()
            {
                ID = 4,
                Name = "Lemon",
                Image = "#F7F307"
            };

            Products[4] = new Product()
            {
                ID = 5,
                Name = "Blueberry",
                Image = "#070BF7"
            };

            Products[5] = new Product()
            {
                ID = 6,
                Name = "Watermelon",
                Image = "#19470D"
            };

            Products[6] = new Product()
            {
                ID = 7,
                Name = "Strawberry",
                Image = "#470D1E"
            };

            Products[7] = new Product()
            {
                ID = 8,
                Name = "Mango",
                Image = "#C27121"
            };

            Products[8] = new Product()
            {
                ID = 9,
                Name = "Lime",
                Image = "#98F24E"
            };

            Products[9] = new Product()
            {
                ID = 10,
                Name = "Bubblegum",
                Image = "#8C298C"
            };
        }

        #endregion
    }
}
