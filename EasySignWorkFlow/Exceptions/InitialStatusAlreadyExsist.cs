namespace EasySignWorkFlow.Exceptions;

internal class InitialStatusAlreadyExist : Exception
{
    public InitialStatusAlreadyExist() : base("Request already has an initial status.") { }
}
