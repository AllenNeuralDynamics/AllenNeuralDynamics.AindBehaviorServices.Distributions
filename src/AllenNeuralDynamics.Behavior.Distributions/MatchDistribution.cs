using System;
using System.Reactive.Linq;
using System.Reactive;

namespace AllenNeuralDynamics.Behavior.Distributions
{
    /// <summary>
    /// Partial class for MatchDistribution to eager deserialization behavior.
    /// </summary>
    partial class MatchDistribution
    {
        private static System.IObservable<TResult> Process<TResult>(System.IObservable<object> source)
                   where TResult : Distribution
        {
            return System.Reactive.Linq.Observable.Create<TResult>(observer =>
            {
                var sourceObserver = System.Reactive.Observer.Create<object>(
                    value =>
                    {
                        var match = value as TResult;
                        if (match != null) observer.OnNext(match);
                    },
                    observer.OnError,
                    observer.OnCompleted);
                return System.ObservableExtensions.SubscribeSafe(source, sourceObserver);
            });
        }
    }
}

