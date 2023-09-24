using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests;

public class ActionExtensionsTests
{
    private FlowMachine<MyRequest, Guid, MyRequestStatus> _flowMachine;
    private MyRequest _request;

    public ActionExtensionsTests()
    {
        _request = new MyRequest();
        _flowMachine = FlowMachine<MyRequest, Guid, MyRequestStatus>
           .Create(MyRequestStatus.Draft)
           .SetRefuseState(MyRequestStatus.Refused)
           .SetCancelState(MyRequestStatus.Canceled);
    }

    [Fact]
    public void Test1()
    {
        // Arrange

        // Act
        _request.OnCreate(_flowMachine);

        //Assert
        Assert.False(_request.CurrentStatus is null);
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test2()
    {

    }
}
