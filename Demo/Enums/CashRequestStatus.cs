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
