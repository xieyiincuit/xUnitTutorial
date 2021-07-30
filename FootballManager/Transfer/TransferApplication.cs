namespace FootballManager.Transfer
{
    public class TransferApplication
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int PlayAge { get; set; }
        public decimal TransferFee { get; set; }
        public decimal AnnualSalary { get; set; }
        public int ContractYears { get; set; }
        public bool IsSupperStar { get; set; }

        public int PlayerStrength { get; set; }
        public int PlayerSpeed { get; set; }

    }
}