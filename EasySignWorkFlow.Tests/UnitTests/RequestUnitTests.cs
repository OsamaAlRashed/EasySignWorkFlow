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

        Assert.Empty(request.States);
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

        Assert.Single(request.States);
        Assert.True(request.CurrentState is not null);
        Assert.True(request.CurrentState.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.Draft)));
        Assert.Equal(firstUserId, request.CurrentState.SignedBy);
        Assert.Equal(firstDateTime, request.CurrentState.DateSigned);
        Assert.Equal(firstNote, request.CurrentState!.Note);
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

        Assert.Equal(2, request.States.Count);
        Assert.Equal(MyRequestStatus.WaitingForManager1, request.CurrentState!.Status);
        Assert.True(request.CurrentState!.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.WaitingForManager1)));
        Assert.Equal(secondNote, request.CurrentState!.Note);
        Assert.Equal(secondUserId, request.CurrentState.SignedBy);
        Assert.Equal(secondDateTime, request.CurrentState.DateSigned);
    }
}
