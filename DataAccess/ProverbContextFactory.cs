//public class PaymentContextFactory : IDesignTimeDbContextFactory<PaymentDbContext>
//{
//    // lokal
//    //private readonly string _connectionString = "Data Source=MC\\MZENZENSQLSERVER;Initial Catalog=Proverb;Integrated Security=True";

//    // server
//    private readonly string _connectionString = "Data Source=localhost,1433;Initial Catalog=Payment;Persist Security Info=True;User ID=SA;Password=SuperSave123!";

//    public PaymentDbContext CreateDbContext(string[] args)
//    {
//        var optionsBuilder = new DbContextOptionsBuilder<PaymentDbContext>();
//        optionsBuilder.UseSqlServer(_connectionString);

//        return new PaymentDbContext(optionsBuilder.Options);
//    }
//}