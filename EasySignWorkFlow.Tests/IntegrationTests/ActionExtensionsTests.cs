using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests.IntegrationTests;

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
    public void Given_RequestIsDraft_When_Approve_Then_CurrentStatusIsWaitingForManager1()
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

    [Fact]
    public void Given_RequestIsDraft_When_ApproveTwice_Then_CurrentStatusIsAccepted()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);
        _request.Approve(_flowMachine);

        //Assert
        Assert.True(_request.CurrentStatus == MyRequestStatus.Accepted);
    }

    [Fact]
    public void Given_RequestIsDraft_When_Approve3Times_Then_CurrentStatusIsAcceptedAndTheLastApproveWillFails()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);
        _request.Approve(_flowMachine);
        var result = _request.Approve(_flowMachine);

        //Assert
        Assert.False(result);
        Assert.True(_request.CurrentStatus == MyRequestStatus.Accepted);
    }

    [Fact]
    public void Given_RequestIsDraft_When_ApproveWithoutCallOnCreate_Then_ApproveMethodThrowsAnException()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act

        //Assert
        Assert.ThrowsAny<Exception>(() => _request.Approve(_flowMachine));
    }

    [Fact]
    public void Given_RequestIsDraft_When_Refuse_Then_CurrentStatusIsRefused()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);
        _request.Refuse(_flowMachine);

        //Assert
        Assert.True(_request.CurrentStatus == MyRequestStatus.Refused);
    }

    [Fact]
    public void Given_RequestIsRefused_When_RefuseAgain_Then_RefuseWillFails()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);
        _request.Refuse(_flowMachine);
        var result = _request.Refuse(_flowMachine);

        //Assert
        Assert.False(result);
        Assert.True(_request.CurrentStatus == MyRequestStatus.Refused);
    }

    [Fact]
    public void Given_RequestIsRefused_When_Reset_Then_CurrentStatusIsDraft()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Refuse(_flowMachine);
        _request.Reset(_flowMachine);

        //Assert
        Assert.True(_request.CurrentStatus == MyRequestStatus.Draft);
    }

    [Fact]
    public void Given_RequestIsWaitingForManager1_When_Reset_Then_ResetWillFails()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);
        _request.Approve(_flowMachine);
        var result = _request.Reset(_flowMachine);

        //Assert
        Assert.False(result);
        Assert.True(_request.CurrentStatus == MyRequestStatus.WaitingForManager1);
    }
}
