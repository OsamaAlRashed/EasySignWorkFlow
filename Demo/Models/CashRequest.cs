using Demo.Enums;
using EasySignWorkFlow.Models;

namespace Demo.Models;

public class CashRequest : Request<Guid, CashRequestStatus>
{
    public Guid Id { get; set; }
    public int DirId { get; set; }
    public int SectorId { get; set; }
    public int CenterId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public CashRequestType? RequestType { get; set; }
}

public enum PaymentMethod
{
    Bank_Transfer = 1,
    Cash = 2,
}

public enum CashRequestType
{
    Withdrawal = 1,
    Payment = 2,
    Transfer = 3,
    Exchange = 4,
}
