namespace EasySignWorkFlow.Exceptions;

internal class CurrentStatusNullException : Exception
{
    public CurrentStatusNullException() : base("Current Status can not be null,maybe you missed use 'OnCreate' method.")
    {
        
    }
}
