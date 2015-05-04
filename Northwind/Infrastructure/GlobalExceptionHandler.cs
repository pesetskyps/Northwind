using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Northwind.Infrastructure
{
    class GlobalExceptionHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            Logger.Instance.Error(error);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            if (error is FaultException)
            {
                return;
            }
            var newEx = new FaultException(
                string.Format("General exception occured in module {0}",
                              error.TargetSite.Name));

            var msgFault = newEx.CreateMessageFault();
            fault = Message.CreateMessage(version, msgFault, newEx.Action);
        }
    }
}
