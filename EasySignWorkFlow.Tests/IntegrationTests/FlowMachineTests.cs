using EasySignWorkFlow.Extensions;
using EasySignWorkFlow.Tests.Models;
using Xunit;

namespace EasySignWorkFlow.Tests.IntegrationTests
{
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
        public void Given_NoSetup_When_OnCreatedNotCalled_Then_CurrentStatusIsNull()
        {
            // Arrange

            // Act

            // Assert
            Assert.True(_request.CurrentState is null);
        }

        [Fact]
        public void When_OnCreateCalled_Then_CurrentStatusIsDraft()
        {
            // Arrange

            // Act
            _request.OnCreate(_flowMachine);

            // Assert
            Assert.True(_request.CurrentState is not null &&
                        _request.CurrentState.Status == MyRequestStatus.Draft);
        }

        [Fact]
        public void Given_RequestIsDraft_When_FiredWithNoTransition_Then_CurrentStatusRemainsDraft()
        {
            // Arrange

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState.Status == MyRequestStatus.Draft);
        }

        [Fact]
        public void Given_RequestIsDraft_When_Fired_Then_CurrentStatusIsWaitingForManager1()
        {
            // Arrange
            _flowMachine.When(MyRequestStatus.Draft)
               .Set(MyRequestStatus.WaitingForManager1);

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
        }

        [Fact]
        public void Given_RequestIsDraft_When_ValueIsZero_Then_CurrentStatusIsWaitingForManager1()
        {
            // Arrange
            _flowMachine.When(MyRequestStatus.Draft)
                .If(request => request.Value == 0)
                .Set(MyRequestStatus.WaitingForManager1);

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
        }

        [Fact]
        public void Given_RequestIsDraft_When_ValueIsOne_Then_CurrentStatusRemainsDraft()
        {
            // Arrange
            _flowMachine.When(MyRequestStatus.Draft)
                .If(request => request.Value == 1)
                .Set(MyRequestStatus.WaitingForManager1);

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.False(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.Draft);
        }

        [Fact]
        public void Given_RequestIsDraft_When_ValueIsZero_Then_CurrentStatusIsWaitingForManager2()
        {
            // Arrange
            _flowMachine.When(MyRequestStatus.Draft)
                .If(request => request.Value != 0)
                .Set(MyRequestStatus.WaitingForManager1);

            _flowMachine.When(MyRequestStatus.Draft)
                .If(request => request.Value == 0)
                .Set(MyRequestStatus.WaitingForManager2);

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager2);
            Assert.False(_request.CurrentState!.Status == MyRequestStatus.Draft);
        }

        [Fact]
        public void Given_RequestIsDraft_When_ValueIsOneOrZero_Then_CurrentStatusIsWaitingForManager1()
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
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
            Assert.False(_request.CurrentState!.Status == MyRequestStatus.Draft);
        }

        [Fact]
        public void Given_RequestIsDraft_When_ValueIsZeroOrOne_Then_CurrentStatusIsWaitingForManager1()
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
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
            Assert.False(_request.CurrentState!.Status == MyRequestStatus.Draft);
        }

        [Fact]
        public void Given_RequestIsDraft_When_TransitionsHappen_Then_CurrentStatusIsAccepted()
        {
            // Arrange
            _flowMachine.When(MyRequestStatus.Draft)
                .Set(MyRequestStatus.WaitingForManager1);

            _flowMachine.When(MyRequestStatus.WaitingForManager1)
                .Set(MyRequestStatus.Accepted);

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.Accepted);
        }

        [Fact]
        public void Given_RequestIsDraft_When_TransitionHappensWithCustomAction_Then_CurrentStatusIsWaitingForManager1AndValueIsSetTo1()
        {
            // Arrange
            MyRequestStatus? currentStatus = null;
            MyRequestStatus? nextStatus = null;

            _flowMachine.When(MyRequestStatus.Draft)
                .Set(MyRequestStatus.WaitingForManager1)
                .OnExecute((request, current, next, _) =>
                {
                    currentStatus = current;
                    nextStatus = next;
                    request.Value = 1;
                });

            // Act
            _request.OnCreate(_flowMachine);
            _flowMachine.Fire(_request, _request.CurrentState!.Status);

            // Assert
            Assert.True(_request.CurrentState!.Status == MyRequestStatus.WaitingForManager1);
            Assert.True(currentStatus!.Value == MyRequestStatus.Draft);
            Assert.True(nextStatus!.Value == MyRequestStatus.WaitingForManager1);
            Assert.True(_request.Value == 1);
        }
    }
}
