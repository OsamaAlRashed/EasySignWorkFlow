using EasySignWorkFlow.Models;
using EasySignWorkFlow.Tests.Models;
using NSubstitute;
using Xunit;

namespace EasySignWorkFlow.Tests.UnitTests;

public class FlowMachineUnitTests
{
    [Fact]
    public void Test()
    {
        var request = new MyRequest();

        Assert.True(request.Statuses.Count == 0);
        Assert.True(request.CurrentState is null);
        Assert.True(request.CurrentStatus is null);
        Assert.True(request.PreviousStatus is null);
        Assert.True(request.PreviousStatusName is null);
        Assert.True(request.LastSignBy == default);
        Assert.True(request.LastSignDate is null);
    }
}
