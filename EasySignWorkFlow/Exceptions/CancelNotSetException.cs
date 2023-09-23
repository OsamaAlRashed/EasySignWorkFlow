namespace EasySignWorkFlow.Exceptions;

internal class CancelNotSetException : Exception
{
    public CancelNotSetException() : base("You should set your cancel status to use 'Cancel' method") { }
}
