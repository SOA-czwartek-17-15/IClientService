using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

using Contracts;

namespace Mockups
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    class CanTransferMoney : ICanTransferMoney
    {
        public bool TransferMoney(Guid idFrom, Guid idTo, long amount)
        {
            return true;
        }

    }
}
