using IntranetApi.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IntranetApi.Models
{
    public class StaffRecord : BaseAuditEntity
    {
        public int EmployeeId { get; set; }
        public int DepartmentId { get; set; }

        [MaxLength(200)]
        public string? OtherDepartment { get; set; }

        public StaffRecordType RecordType { get; set; }
        public StaffRecordDetailType RecordDetailType { get; set; }
        public int LateAmount { get; set; }

        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfDays { get; set; }
        public int NumberOfHours { get; set; }
        public decimal Fine { get; set; }
        public decimal CalculationAmount { get; set; }
        [MaxLength(200)]
        public string? Remarks { get; set; }

        public virtual ICollection<StaffRecordDocument> StaffRecordDocuments { get; set; }= new HashSet<StaffRecordDocument>();

        [ForeignKey(nameof(EmployeeId))]
        public virtual User Employee { get; set; }

        public void UpdateCalculationAmount(int salary, int workingHours)
        {
            NumberOfDays = 0;
            NumberOfHours = 0;
            CalculationAmount = 0;
            switch (RecordDetailType)
            {
                case StaffRecordDetailType.PaidMCs:
                case StaffRecordDetailType.PaidOffs:
                case StaffRecordDetailType.ExtraPayCoverShift:
                case StaffRecordDetailType.DeductionUnpaidLeave:
                    {
                        NumberOfDays = (EndDate - StartDate).Days + 1;
                        if (RecordDetailType == StaffRecordDetailType.ExtraPayCoverShift)
                        {
                            CalculationAmount = NumberOfDays * (salary / 365);
                        }

                        if (RecordDetailType == StaffRecordDetailType.DeductionUnpaidLeave)
                        {
                            CalculationAmount = -NumberOfDays * (salary / 365);
                        }
                        break;
                    }
                case StaffRecordDetailType.ExtraPayOTs:
                    {
                        NumberOfHours = (int)Math.Round((EndDate - StartDate).TotalHours);
                        if (RecordDetailType == StaffRecordDetailType.ExtraPayOTs && workingHours > 0)
                        {
                            CalculationAmount = NumberOfHours * (salary / (365 * workingHours));
                        }
                        break;
                    }
                case StaffRecordDetailType.DeductionLate:
                    {
                        CalculationAmount = -LateAmount;
                        break;
                    }
                default: break;
            }
        }
    }
}