using System;

namespace FootballManager.Transfer
{
    public class TransferPolicyEvaluator
    {
        public virtual bool IsInTransferPeriod()
        {
            throw new NotImplementedException();
        }

        protected virtual bool IsBannedFromTransferring()
        {
            throw new NotImplementedException();
        }
    }
}