using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests;

public class FlowMachineTest
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
        FlowMachine<MyRequest, Guid, MyRequestStatus> flowMachine
           = new(MyRequestStatus.Draft);
        var request = new MyRequest();

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

        //Assert
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

}
