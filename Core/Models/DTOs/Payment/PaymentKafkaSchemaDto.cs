public class PaymentKafkaSchemaDto
{
    public String Source { get; set; }
    public long Timestamp { get; set; }
    public string Type { get; set; }
    public PaymentDto Payment { get; set; }
}