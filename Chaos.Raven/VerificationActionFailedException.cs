using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chaos.Raven
{
    public class VerificationActionFailedException : Exception
    {
        private readonly string actionType;
        private readonly long elapsed;

        public VerificationActionFailedException(string actionType, long elapsed)
            : base($"Action '{actionType}' has failed after {elapsed}ms of execution")
        {
            this.elapsed = elapsed;
            this.actionType = actionType;
        }

        public string ActionType
        {
            get
            {
                return actionType;
            }
        }

        public long Elapsed
        {
            get
            {
                return elapsed;
            }
        }
    }
}
