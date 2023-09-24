using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests;

// Todo: rename the tests
public class FlowMachineTests
{
    private FlowMachine<MyRequest, Guid, MyRequestStatus> _flowMachine;
    private MyRequest _request;

    public FlowMachineTests()
    {
        _request = new MyRequest();
        _flowMachine = FlowMachine<MyRequest, Guid, MyRequestStatus>
           .Create(MyRequestStatus.Draft);
    }

    [Fact]
    public void Test1()
    {
        // Arrange

        // Act

        //Assert
        Assert.True(_request.CurrentStatus is null);
    }

    [Fact]
    public void Test2()
    {
        // Arrange

        // Act
        _request.OnCreate(_flowMachine);

        //Assert
        Assert.False(_request.CurrentStatus is null);
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test3()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
           .Set(MyRequestStatus.WaitingForManager1);

        // Act
        _request.OnCreate(_flowMachine);

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        //Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
    }

    [Fact]
    public void Test4()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        _request.OnCreate(_flowMachine);

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        //Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
    }

    [Fact]
    public void Test5()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 1)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        _request.OnCreate(_flowMachine);

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        //Assert
        Assert.False(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test6()
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

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        // Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.False(_request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test7()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 1)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        _request.OnCreate(_flowMachine);

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        //Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.False(_request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test8()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 1)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        _request.OnCreate(_flowMachine);

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        //Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.False(_request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test9()
    {
        // Arrange
        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        _flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        _request.OnCreate(_flowMachine);

        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);
        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        //Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.Accepted);
    }

    [Fact]
    public void Test10()
    {
        // Arrange
        MyRequestStatus? currentStatus = null;
        MyRequestStatus? nextStatus = null;

        _flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1)
            .OnExecute((request, current, next) =>
            {
                currentStatus = current;
                nextStatus = next;
                request.Value = 1;
            });

        // Act
        _request.OnCreate(_flowMachine);
        _flowMachine.Fire(_request, _request.CurrentStatus!.Value);

        // Assert
        Assert.True(_request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.True(currentStatus!.Value == MyRequestStatus.Draft);
        Assert.True(nextStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.True(_request.Value == 1);
    }
}
