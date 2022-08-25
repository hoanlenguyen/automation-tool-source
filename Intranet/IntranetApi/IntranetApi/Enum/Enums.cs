namespace IntranetApi.Enum
{
    public enum UserType
    {
        Employee = 0,
        SuperAdmin = 10
    }

    public enum StaffRecordType
    {
        ExtraPay = 0,
        Deduction = 1,
        PaidOffs = 2,
        PaidMCs = 3
    }

    public enum StaffRecordDetailType
    {
        ExtraPayOTs = 0,
        ExtraPayCoverShift = 2,
        DeductionLate = 4,
        DeductionUnpaidLeave = 8,
        PaidOffs = 16,
        PaidMCs = 32
    }
}