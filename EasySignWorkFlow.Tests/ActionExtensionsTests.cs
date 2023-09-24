using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests;

// Todo: rename the tests
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
        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value != 0)
            .Set(MyRequestStatus.WaitingForManager2);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);

        //Assert
        Assert.True(_request.CurrentStatus == MyRequestStatus.WaitingForManager1);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);

        //Assert
        Assert.True(_request.CurrentStatus == MyRequestStatus.WaitingForManager1);
    }
}
