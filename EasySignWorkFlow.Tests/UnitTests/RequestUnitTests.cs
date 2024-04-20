using EasySignWorkFlow.Models;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests.UnitTests;

public class RequestUnitTests
{
    [Fact]
    public void Test()
    {
        var request = new MyRequest();

        Assert.True(request.Statuses.Count == 0);
        Assert.True(request.CurrentState is null);
        Assert.True(request.CurrentState is null);
    }

    [Fact]
    public void Test2()
    {
        var request = new MyRequest();

        Guid firstUserId = Guid.NewGuid();
        DateTime firstDateTime = DateTime.Now;
        string firstNote = "firstNote";

        var newState = new State<Guid, MyRequestStatus>(MyRequestStatus.Draft, firstDateTime, firstUserId, firstNote);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        Assert.True(request.Statuses.Count == 1);
        Assert.True(request.CurrentState is not null);
        Assert.True(request.CurrentState.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.Draft)));
        Assert.True(request.CurrentState.SignedBy == firstUserId);
        Assert.True(request.CurrentState.DateSigned == firstDateTime);
        Assert.True(request.CurrentState!.Note == firstNote);
    }

    [Fact]
    public void Test3()
    {
        var request = new MyRequest();

        Guid firstUserId = Guid.NewGuid();
        DateTime firstDateTime = DateTime.Now;
        string firstNote = "firstNote";

        Guid secondUserId = Guid.NewGuid();
        DateTime secondDateTime = DateTime.Now;
        string secondNote = "secondNote";

        var newState = new State<Guid, MyRequestStatus>(MyRequestStatus.Draft, firstDateTime, firstUserId, firstNote);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        newState = new State<Guid, MyRequestStatus>(MyRequestStatus.WaitingForManager1, secondDateTime, secondUserId, secondNote);
        request.Add(newState);
        request.UpdateCurrentState(newState);

        Assert.True(request.Statuses.Count == 2);
        Assert.True(request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
        Assert.True(request.CurrentState!.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.WaitingForManager1)));
        Assert.True(request.CurrentState!.Note == secondNote);
        Assert.True(request.CurrentState.SignedBy == secondUserId);
        Assert.True(request.CurrentState.DateSigned == secondDateTime);
    }
}
