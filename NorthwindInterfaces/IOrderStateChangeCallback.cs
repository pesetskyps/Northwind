using System.ServiceModel;


namespace NorthwindInterfaces
{
    public interface IOrderStateChangeCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnOrderStateChange(string message);
    }
}
