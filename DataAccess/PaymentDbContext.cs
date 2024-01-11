public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<Payment> Payments { get; set; }
    public DbSet<Payment> Status { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        

    }
}