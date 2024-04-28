using EasySignWorkFlow.Commons;
using EasySignWorkFlow.Enums;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests.UnitTests;

public class ResultTests
{
    [Fact]
    public void SetSuccessTest()
    {
        // Act
        var act = Result<MyRequestStatus>
            .SetSuccess(ActionType.Approve, MyRequestStatus.Draft, MyRequestStatus.WaitingForManager1);

        // Assert
        Assert.NotNull(act);
        Assert.True(act.IsSucceeded);
        Assert.True(string.IsNullOrEmpty(act.Message));
        Assert.Equal(ActionType.Approve, act.ActionType);
        Assert.Equal(MyRequestStatus.Draft, act.PreviousStatus);
        Assert.Equal(MyRequestStatus.WaitingForManager1, act.NewStatus);
        Assert.True(act);
    }

    [Fact]
    public void SetFailedTest()
    {
        // Act
        var act = Result<MyRequestStatus>
            .SetFailed(ActionType.Approve, MyRequestStatus.Draft, MyRequestStatus.Draft, "Error message");

        // Assert
        Assert.NotNull(act);
        Assert.False(act.IsSucceeded);
        Assert.Equal("Error message", act.Message);
        Assert.Equal(ActionType.Approve, act.ActionType);
        Assert.Equal(MyRequestStatus.Draft, act.PreviousStatus);
        Assert.Equal(MyRequestStatus.Draft, act.NewStatus);
        Assert.False(act);
        Assert.False(act);
    }
}
