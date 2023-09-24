using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests;

public class FlowMachineTests
{
    [Fact]
    public void Test1()
    {
        // Arrange
        var request = new MyRequest();

        // Act

        //Assert
        Assert.True(request.CurrentStatus is null);
    }

    [Fact]
    public void Test2()
    {
        // Arrange
        var request = new MyRequest();
        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
           = new(MyRequestStatus.Draft);

        // Act
        request.OnCreate(flowMachine);

        //Assert
        Assert.False(request.CurrentStatus is null);
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test3()
    {
        // Arrange
        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
           = new(MyRequestStatus.Draft);
        var request = new MyRequest();

        flowMachine.When(MyRequestStatus.Draft)
           .Set(MyRequestStatus.WaitingForManager1);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);

        //Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
    }

    [Fact]
    public void Test4()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);

        //Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
    }

    [Fact]
    public void Test5()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 1)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);

        //Assert
        Assert.False(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test6()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value != 0)
            .Set(MyRequestStatus.WaitingForManager2);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);

        // Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.False(request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test7()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 1)
            .Set(MyRequestStatus.WaitingForManager1);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);

        //Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.False(request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test8()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 0)
            .Set(MyRequestStatus.WaitingForManager1);

        flowMachine.When(MyRequestStatus.Draft)
            .If(request => request.Value == 1)
            .Set(MyRequestStatus.WaitingForManager1);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);

        //Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.False(request.CurrentStatus!.Value == MyRequestStatus.Draft);
    }

    [Fact]
    public void Test9()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1);

        flowMachine.When(MyRequestStatus.WaitingForManager1)
            .Set(MyRequestStatus.Accepted);

        // Act
        request.OnCreate(flowMachine);

        flowMachine.Fire(request, request.CurrentStatus!.Value);
        flowMachine.Fire(request, request.CurrentStatus!.Value);

        //Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.Accepted);
    }

    [Fact]
    public void Test10()
    {
        // Arrange
        var request = new MyRequest();

        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
          = new(MyRequestStatus.Draft);

        MyRequestStatus? currentStatus = null;
        MyRequestStatus? nextStatus = null;

        flowMachine.When(MyRequestStatus.Draft)
            .Set(MyRequestStatus.WaitingForManager1)
            .OnExecute((request, current, next) =>
            {
                currentStatus = current;
                nextStatus = next;
                request.Value = 1;
            });

        // Act
        request.OnCreate(flowMachine);
        flowMachine.Fire(request, request.CurrentStatus!.Value);

        // Assert
        Assert.True(request.CurrentStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.True(currentStatus!.Value == MyRequestStatus.Draft);
        Assert.True(nextStatus!.Value == MyRequestStatus.WaitingForManager1);
        Assert.True(request.Value == 1);
    }
}
