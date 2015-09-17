using System;
using Raven.Client;
using System.Diagnostics;

namespace Chaos.Raven.Common
{
    public abstract class BaseVerificationAction : BaseAction
    {
        public bool VerifyAction(IDocumentStore store, out long elapsedMilliseconds)
        {
            GenerateData();
            var sw = Stopwatch.StartNew();
            try
            {
                return DoAction(store);
            }
            catch (Exception e)
            {
                OnExceptionThrown(e);
                return false;
            }
            finally
            {
                elapsedMilliseconds = sw.ElapsedMilliseconds;
            }
        }

        protected virtual void GenerateData() { }

        protected abstract bool DoAction(IDocumentStore store);
    }
}
