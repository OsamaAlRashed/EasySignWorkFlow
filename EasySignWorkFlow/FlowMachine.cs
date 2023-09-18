﻿using EasySignWorkFlow.Models;

namespace EasySignWorkFlow
{
    public class FlowMachine<TRequest, TKey, TStatus>
        where TKey : IEquatable<TKey>
        where TStatus : Enum

    {
        public Dictionary<TStatus, Func<TRequest, TStatus>> Map { get; private set; }

        private TStatus _initialStatus;
        private TStatus _currentStatus;
        private TStatus _canceledStatus;
        private TStatus _refusedStatus;
        private TStatus _acceptedStatus;

        public FlowMachine()
        {
            Map = new Dictionary<TStatus, Func<TRequest, TStatus>>();
        }

        public FlowMachine<TRequest, TKey, TStatus> StartWith(TStatus status)
        {
            _currentStatus = status;
            _initialStatus = status;

            return this;
        }

        public FlowMachine<TRequest, TKey, TStatus> Then(TStatus nextStatus)
        {
            Map[_currentStatus] = (request) => nextStatus;
            return this;
        }

        public FlowMachine<TRequest, TKey, TStatus> SetNextIf(TStatus currentStatus, Func<TRequest, bool> predicate,
            TStatus trueSide, TStatus falseSide, Action<TRequest> action)
        {
            Map[currentStatus] = ConvertToFunc(predicate, trueSide, falseSide);
            return this;
        }


        static Func<TRequest, TStatus> ConvertToFunc<TRequest, TStatus>(
            Func<TRequest, bool> predicate, TStatus trueSide, TStatus falseSide)
        {
            return request => predicate(request) ? trueSide : falseSide;
        }

        static Func<TRequest, TStatus> ConvertToFunc<TRequest, TStatus>(
            params (TStatus Status, Func<TRequest, bool> Predicate)[] paths)
        {
            return request =>
            {
                foreach (var (status, predicate) in paths)
                {
                    if (predicate(request))
                    {
                        return status;
                    }
                }

                throw new InvalidOperationException("No predicate matched.");
            };
        }


        public FlowMachine<TRequest, TKey, TStatus> SetNextIf(TStatus currentStatus,
            params (TStatus Status, Func<TRequest, bool> Predicate)[] paths)
        {
            Map[currentStatus] = ConvertToFunc(paths);
            return this;
        }

        public FlowMachine<TRequest, TKey, TStatus> SetNextIf(TStatus currentStatus, Func<TRequest, TStatus> func)
        {
            Map[currentStatus] = func;
            return this;
        }

        public FlowMachine<TRequest, TKey, TStatus> SetCancelState(TStatus status)
        {
            _canceledStatus = status;
            return this;
        }

        public FlowMachine<TRequest, TKey, TStatus> SetRefuseState(TStatus status)
        {
            _refusedStatus = status;
            return this;
        }

        public FlowMachine<TRequest, TKey, TStatus> SetAcceptState(TStatus status)
        {
            _acceptedStatus = status;
            return this;
        }
    }
}


public class FlowMachine2<TRequest, TStatus> where TStatus : notnull
{
    public Dictionary<TStatus, List<MyState<TRequest, TStatus>>> Map { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task> _onTransaction;

    private FlowMachine2()
    {
        Map = new Dictionary<TStatus, List<MyState<TRequest, TStatus>>>();
    }

    private FlowMachine2(IEqualityComparer<TStatus> equalityComparer)
    {
        Map = new Dictionary<TStatus, List<MyState<TRequest, TStatus>>>(equalityComparer);
    }

    public static FlowMachine2<TRequest, TStatus> Create()
    {
        return new();
    }    
    public static FlowMachine2<TRequest, TStatus> Create(IEqualityComparer<TStatus> equalityComparer)
    {
        return new(equalityComparer);
    }

    public FlowMachine2<TRequest, TStatus> SetTransaction(Action<TRequest, TStatus, TStatus> action)
    {
        _onTransaction = (request, current, next) =>
        {
            action(request, current, next);
            return Task.CompletedTask;
        };
        return this;
    }

    public FlowMachine2<TRequest, TStatus> SetTransactionAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onTransaction = action;
        return this;
    }

    public MyState<TRequest, TStatus> When(TStatus currentStatus)
    {
        var stat = new MyState<TRequest, TStatus>(currentStatus);
        if (Map.TryGetValue(currentStatus, out var a))
        {
            a.Add(stat);
        }

        return stat;
    }

    public async ValueTask<bool> FireAsync(TRequest request, Func<TRequest, TStatus> current)
    {
        return await FireAsync(request, current(request));
    }

    public async ValueTask<bool> FireAsync(TRequest request, TStatus current)
    {
        foreach (var state in Map[current])
        {
            if (await state.Can(request))
            {
                await _onTransaction(request, current, state.Next);
                await state.DoOnSetAsync(request, current);
            }

            return true;
        }

        return false;
    }

    public bool Fire(TRequest request, Func<TRequest, TStatus> current)
    {
        return FireAsync(request, current).GetAwaiter().GetResult();
    }

    public bool Fire(TRequest request, TStatus current)
    {
        return FireAsync(request, current).GetAwaiter().GetResult();
    }
}

public class MyState<TRequest, TStatus>
{
    private Func<TRequest, Task<bool>> _predicate;

    public TStatus Next { get; private set; }

    private Func<TRequest, TStatus, TStatus, Task>? _onSetAsync;

    public MyState(TStatus current)
    {
    }

    public MyState<TRequest, TStatus> If(Func<TRequest, bool> predicate)
    {
        this._predicate = request => Task.FromResult(predicate(request));
        return this;
    }
    public MyState<TRequest, TStatus> IfAsync(Func<TRequest, Task<bool>> predicate)
    {
        this._predicate = predicate;
        return this;
    }

    public MyState<TRequest, TStatus> Set(TStatus next, Action<TRequest, TStatus, TStatus>? action = null)
    {
        Next = next;
        return this;
    }

    public MyState<TRequest, TStatus> OnSet(Action<TRequest, TStatus, TStatus> action)
    {
        _onSetAsync = (request, status, arg3) =>
        {
            action(request, status, arg3);
            return Task.CompletedTask;
        };
        return this;
    }

    public MyState<TRequest, TStatus> OnSetAsync(Func<TRequest, TStatus, TStatus, Task> action)
    {
        _onSetAsync = action;
        return this;
    }

    internal async ValueTask<bool> Can(TRequest request)
    {
        return await _predicate(request);
    }

    internal async Task DoOnSetAsync(TRequest request, TStatus current) =>
        await _onSetAsync?.Invoke(request, current, Next)!;
}