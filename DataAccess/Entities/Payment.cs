
using System.ComponentModel.DataAnnotations;

public class Payment
{
    [Key]
    public Guid UserId { get; set; }

    /// <summary>
    /// Durch [JsonIgnore] würde der Value nicht an Kafka gesendet werden
    /// </summary>
    //[JsonIgnore]
    public string Firstname { get; set; }

    /// <summary>
    /// Durch [JsonIgnore] würde der Value nicht an Kafka gesendet werden
    /// </summary>
    //[JsonIgnore]
    public string Lastname { get; set; }

    /// <summary>
    /// Bsp: DieterMücke
    /// </summary>
    public string Username { get; set; }
}
