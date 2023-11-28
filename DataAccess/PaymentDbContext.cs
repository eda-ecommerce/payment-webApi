public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

    }
}