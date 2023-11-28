public class PaymentUpdateDto
{
    public Guid? UserId { get; set; }

    /// <summary>
    /// With [JsonIgnore] the value would not be sent to Kafka
    /// </summary>
    //[JsonIgnore]
    public string Firstname { get; set; }

    /// <summary>
    /// With [JsonIgnore] the value would not be sent to Kafka
    /// </summary>
    //[JsonIgnore]
    public string Lastname { get; set; }

    /// <summary>
    /// eg: DieterMücke
    /// </summary>
    public string Username { get; set; }
}

