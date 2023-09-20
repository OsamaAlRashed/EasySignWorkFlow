namespace Demo.Enums;

public enum CashRequestStatus
{
    Draft = 1,
    WaitingForFinanceAccountantApproval = 2,
    WaitingForCashierOfficerApproval = 3,
    WaitingForFinanceManagerApproval = 4,
    WaitingForBankPayerApproval = 5,
    WaitingForDeputyManagerApproval = 6,
    Accepted = 7,
    Refused = 8,
    Canceled = 9,
}