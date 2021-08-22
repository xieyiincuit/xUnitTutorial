using System;
using FootballManager.Physical;
using FootballManager.Services;

namespace FootballManager.Transfer
{
    public class TransferApproval
    {
        private readonly IPhysicalExamination _physicalExamination;
        private readonly TransferPolicyEvaluator _transferPolicyEvaluator;
        private const int RemainingTotalBudget = 300; //剩余预算

        public TransferApproval(IPhysicalExamination physicalExamination, TransferPolicyEvaluator transferPolicyEvaluator)
        {
            _physicalExamination = physicalExamination ?? throw new ArgumentNullException();
            _transferPolicyEvaluator = transferPolicyEvaluator;
        }

        public TransferResult Evaluate(TransferApplication transfer)
        {
            if (!_transferPolicyEvaluator.IsInTransferPeriod())
            {
                return TransferResult.Postponed;
            }

            if (_physicalExamination.MedicalRoom.Status.IsAvailable == "close")
            {
                return TransferResult.Postponed;
            }
            try
            {
                _physicalExamination.IsHealthy(
                    transfer.PlayAge, transfer.PlayerStrength, transfer.PlayerSpeed, out var isHealthy);
                if (!isHealthy)
                {
                    _physicalExamination.PhysicalGrade = PhysicalGrade.Failed;
                    return TransferResult.Rejected;
                }
                else
                {
                    if (transfer.PlayAge < 25)
                    {
                        _physicalExamination.PhysicalGrade = PhysicalGrade.Superb;
                    }
                    else
                    {
                        _physicalExamination.PhysicalGrade = PhysicalGrade.Passed;
                    }
                }
            }
            catch (Exception)
            {

                return TransferResult.Postponed;
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