using System.Linq.Expressions;
using EasySignWorkFlow.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Demo.Enums;

public enum CashRequestStatus
{
    Draft = 1,
    WaitingForFinanceAccountantApproval = 2,
    WaitingForCashierOfficerApproval = 3,
    WaitingForFinanceManagerApproval = 4,
    Accepted = 5,
    Refused = 6,
    Canceled = 7,
    WaitingForBankPayerApproval = 8,
    WaitingForDeputyManagerApproval = 9,
}

public class CashRequestStatusC : State<Guid, CashRequestStatus>
{
    public static CashRequestStatusC Draft = new(CashRequestStatus.Draft);

    public static CashRequestStatusC WaitingForFinanceAccountantApproval =
        new(CashRequestStatus.WaitingForFinanceAccountantApproval);

    public static CashRequestStatusC WaitingForCashierOfficerApproval =
        new(CashRequestStatus.WaitingForCashierOfficerApproval);

    public static CashRequestStatusC WaitingForFinanceManagerApproval =
        new(CashRequestStatus.WaitingForFinanceManagerApproval);

    public CashRequestStatusC(CashRequestStatus status) : base(status)
    {
    }
}

public class CashRequestStatusCEqualityComparer : ValueComparer<CashRequestStatusC>
{
    public static CashRequestStatusCEqualityComparer Instance = new();
    public CashRequestStatusCEqualityComparer() : base((x, y) => x.Status == y.Status, x => HashCode.Combine(x.Status))
    {
    }


}