namespace EasySignWorkFlow.Exceptions;

internal class CurrentStateNullException : Exception
{
    public CurrentStateNullException() : base("Current State can not be null,maybe you missed use 'OnCreate' method.") { }
}
