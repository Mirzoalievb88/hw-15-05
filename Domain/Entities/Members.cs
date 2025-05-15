namespace Domain.Entities;

public class Members
{
    public int MemberId { get; set; }
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public DateTime MemberShipDate { get; set; }
}