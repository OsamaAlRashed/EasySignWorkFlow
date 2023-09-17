using EasySignWorkFlow.Models;

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

        public FlowMachine<TRequest, TKey, TStatus> SetNextIf(TStatus currentStatus,Func<TRequest, bool> predicate, TStatus trueSide, TStatus falseSide,Action<TRequest> action)
        {
            Map[currentStatus] = ConvertToFunc(predicate, trueSide, falseSide);
            return this;
        }
             public FlowMachine<TRequest, TKey, TStatus> SetNextIf(TStatus currentStatus,Func<TRequest, bool> predicate, TStatus trueSide, TStatus falseSide,Action<TRequest> action)
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
            params (TStatus Status, Func<TRequest, bool> Predicate)[] pats)
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


        public FlowMachine<TRequest, TKey, TStatus> SetNextIf(TStatus currentStatus, params (TStatus Status, Func<TRequest, bool> Predicate)[] paths)
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

public class FlowMachine2<TRequest,TStatus,TTrigger>
    where TStatus : Enum
    {
        public Dictionary<TStatus, MyState<TStatus,TTrigger,TRequest>[]> Map { get; private set; }

        public FlowMachine2<TRequest,TStatus,TTrigger> Config(TStatus currentStatus,Func<TRequest, bool> predicate,Func<TTrigger> trueSide,Action<TRequest>? trueSideAction = null)    
        {
            var a = Map[currentStatus];
            
            return this;
        }

        public TTrigger Fire(TRequest request,Func<TRequest,TStatus> current)
        {
                var currnetStat =  current(request);
               
                foreach (var check in Map[currnetStat])
                {
                    if(check.Go(request) is {} a){
                        return a;
                    }
                };
                throw new Exception("end");
                
        }
    }

public class MyState<TStatus,TTrigger,TRequest>
{
    private Func<TRequest, bool> predicate;
        
    private Func<TTrigger> trueSide;
    
    private   Action<TRequest>? trueSideAction;

    public MyState(Func<TRequest, bool> predicate)
    {
        this.predicate = predicate;
    }

    public MyState<TStatus,TTrigger,TRequest> TriggerCreation(Func<TTrigger> trigger)
    {
        trueSide = trigger;
        return this;
    }
    
    public MyState<TStatus,TTrigger,TRequest> WhatDo(Action<TRequest> action)
    {
        trueSideAction = action;
        return this;
    }

     public TTrigger? Go(TRequest request){
        if(predicate(request)){
            if(trueSideAction is not null)
                trueSideAction(request);
            return trueSide();
        }

        return default;
     }
 
            
}
        