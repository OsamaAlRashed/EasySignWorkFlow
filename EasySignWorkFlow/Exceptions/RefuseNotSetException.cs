namespace EasySignWorkFlow.Exceptions;

public class RefuseNotSetException : Exception
{
    public RefuseNotSetException() : base("You should set your cancel status to use 'Refuse' method") { }
}
