/// <summary>
/// Deserialized JSON data class for a math block derived from the API
/// </summary>
public class MathBlockData
{
    public int id { get; set; }
    public string subject { get; set; }
    public string grade { get; set; }
    public int mastery { get; set; }
    public string domainid { get; set; }
    public string domain { get; set; }
    public string cluster { get; set; }
    public string standardid { get; set; }
    public string standarddescription { get; set; }
}
