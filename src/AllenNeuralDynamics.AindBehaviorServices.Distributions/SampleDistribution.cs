using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using MathNet.Numerics.Distributions;

namespace AllenNeuralDynamics.AindBehaviorServices.Distributions
{
    /// <summary>
    /// Samples values from distribution objects in an observable sequence.
    /// </summary>
    [Combinator]
    [Description("Samples a value for a known distribution.")]
    [WorkflowElementCategory(ElementCategory.Transform)]
    public class SampleDistribution
    {
        private Random randomSource;

        /// <summary>
        /// Gets or sets the random number generator to use for sampling.
        /// </summary>
        public Random RandomSource
        {
            get { return randomSource; }
            set { randomSource = value; }
        }

        /// <summary>
        /// Processes an observable sequence of distributions by sampling a value from each.
        /// </summary>
        /// <param name="source">The source observable sequence of distributions.</param>
        /// <returns>An observable sequence of sampled values.</returns>
        public IObservable<double> Process(IObservable<Distribution> source)
        {
            return source.Select(value => value.SampleDistribution(RandomSource));
        }
    }
}
