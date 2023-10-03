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
        Assert.True(request.CurrentStatus is null);
        Assert.True(request.PreviousStatus is null);
        Assert.True(request.LastSignBy == default);
        Assert.True(request.LastSignDate is null);
    }

    [Fact]
    public void Test2()
    {
        var request = new MyRequest();

        Guid firstUserId = Guid.NewGuid();
        DateTime firstDateTime = DateTime.Now;
        string firstNote = "firstNote";


        request.Add(new State<Guid, MyRequestStatus>(MyRequestStatus.Draft, firstDateTime, firstUserId, firstNote));

        Assert.True(request.Statuses.Count == 1);
        Assert.True(request.CurrentState is not null);
        Assert.True(request.CurrentState.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.Draft)));
        Assert.True(request.PreviousStatus is null);
        Assert.True(request.PreviousState is null);
        Assert.True(request.LastSignBy == firstUserId);
        Assert.True(request.LastSignDate == firstDateTime);
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

        request.Add(new State<Guid, MyRequestStatus>(MyRequestStatus.Draft, firstDateTime, firstUserId, firstNote));
        request.Add(new State<Guid, MyRequestStatus>(MyRequestStatus.WaitingForManager1, secondDateTime, secondUserId, secondNote));

        Assert.True(request.Statuses.Count == 2);
        Assert.True(request.CurrentStatus == MyRequestStatus.WaitingForManager1);
        Assert.True(request.CurrentState!.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.WaitingForManager1)));
        Assert.True(request.PreviousStatus == MyRequestStatus.Draft);
        Assert.True(request.PreviousState!.Equals(new State<Guid, MyRequestStatus>(MyRequestStatus.Draft)));
        Assert.True(request.PreviousState!.Note == firstNote);
        Assert.True(request.CurrentState!.Note == secondNote);
        Assert.True(request.LastSignBy == secondUserId);
        Assert.True(request.LastSignDate == secondDateTime);
    }
}
