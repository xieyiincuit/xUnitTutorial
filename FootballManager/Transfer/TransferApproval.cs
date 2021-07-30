using System;
using FootballManager.Services;

namespace FootballManager.Transfer
{
    public class TransferApproval
    {
        private readonly IPhysicalExamination _physicalExamination;
        private const int RemainingTotalBudget = 300; //剩余预算

        public TransferApproval(IPhysicalExamination physicalExamination)
        {
            _physicalExamination = physicalExamination ?? throw new ArgumentNullException();
        }

        public TransferResult Evaluate(TransferApplication transfer)
        {

            _physicalExamination.IsHealthy(transfer.PlayAge, transfer.PlayerStrength, transfer.PlayerSpeed, out var isHealthy);
            if (!isHealthy)
            {
                return TransferResult.Rejected;
            }

            var totalTransferFee = transfer.TransferFee + transfer.ContractYears * transfer.AnnualSalary;
            if (RemainingTotalBudget < totalTransferFee)
            {
                return TransferResult.Rejected;
            }
            if (transfer.PlayAge < 30)
            {
                return TransferResult.Approved;
            }

            if (transfer.IsSupperStar)
            {
                return TransferResult.ReferredToBoss;
            }

            return TransferResult.Rejected;
        }
    }
}