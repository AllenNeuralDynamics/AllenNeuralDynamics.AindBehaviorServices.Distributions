using System;
using System.Linq;
using System.Reactive.Linq;
using MathNet.Numerics.Distributions;

namespace AllenNeuralDynamics.AindBehaviorServices.Distributions
{

    /// <summary>
    /// Provides functionality for sampling and manipulating values from statistical distributions, including support
    /// for scaling and truncation operations.
    /// </summary>
    /// <remarks>The Distribution class serves as a base for working with probability distributions, allowing
    /// users to sample values, apply scaling factors, and enforce truncation constraints. It is designed to be extended
    /// for specific distribution types and supports integration with custom random number generators. Thread safety and
    /// performance characteristics may depend on the implementation of derived classes and the underlying distribution
    /// objects.</remarks>
    partial class Distribution
    {
        private const uint SampleSize = 1000;

        /// <summary>
        /// Samples a value from the distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the distribution.</returns>
        public virtual double SampleDistribution(Random random)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the underlying distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>An <see cref="IDistribution"/> instance.</returns>
        public virtual IDistribution GetDistribution(Random random)
        {
            throw new NotImplementedException();
        }

        private static double ApplyScaleAndOffset(double value, ScalingParameters scalingParameters)
        {
            return scalingParameters == null ? value : value * scalingParameters.Scale + scalingParameters.Offset;
        }

        /// <summary>
        /// Draws a sample from the distribution with optional scaling and truncation.
        /// </summary>
        /// <param name="distribution">The distribution to sample from.</param>
        /// <param name="scalingParameters">Optional scaling parameters to apply to the sample.</param>
        /// <param name="truncationParameters">Optional truncation parameters to apply to the sample.</param>
        /// <returns>A sample value with scaling and truncation applied.</returns>
        public double DrawSample(IDistribution distribution, ScalingParameters scalingParameters, TruncationParameters truncationParameters)
        {
            if (truncationParameters == null)
            {
                return ApplyScaleAndOffset(distribution.Sample(), scalingParameters);
            }

            ValidateTruncationParameters(truncationParameters);

            switch (truncationParameters.TruncationMode)
            {
                case TruncationParametersTruncationMode.Clamp:
                    var sample = ApplyScaleAndOffset(distribution.Sample(), scalingParameters);
                    return Math.Min(Math.Max(sample, truncationParameters.Min), truncationParameters.Max);
                case TruncationParametersTruncationMode.Exclude:
                    double[] samples = new double[SampleSize];
                    distribution.Samples(samples);
                    var scaledSamples = samples.Select(x => ApplyScaleAndOffset(x, scalingParameters)).ToArray();
                    return ValidateTruncationExcludeMode(scaledSamples, truncationParameters);
                default:
                    throw new ArgumentException("Invalid truncation mode.");
            }
        }

        private static double ValidateTruncationExcludeMode(double[] drawnSamples, TruncationParameters truncationParameters)
        {
            double outValue;
            var average = drawnSamples.Average();
            var truncatedSamples = drawnSamples.Where(x => x >= truncationParameters.Min && x <= truncationParameters.Max);

            if (truncatedSamples.Count() <= 0)
            {
                if (average <= truncationParameters.Min)
                {
                    outValue = truncationParameters.Min;
                }
                else if (average >= truncationParameters.Max)
                {
                    outValue = truncationParameters.Max;
                }
                else
                {
                    throw new ArgumentException("Truncation heuristic has failed. Please check your truncation parameters.");
                }
            }
            else
            {
                outValue = truncatedSamples.First();
            }
            return outValue;
        }

        private static void ValidateTruncationParameters(TruncationParameters truncationParameters)
        {
            if (truncationParameters == null) { return; }
            if (truncationParameters.Min > truncationParameters.Max)
            {
                throw new ArgumentException("Invalid truncation parameters. Min must be lower than Max");
            }
        }
    }

    partial class PdfDistribution
    {
        /// <summary>
        /// Samples a value from the custom probability density function.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A value sampled from the PDF.</returns>
        public override double SampleDistribution(Random random)
        {
            var pdf = DistributionParameters.Pdf;
            var index = DistributionParameters.Index;
            if (pdf.Count != index.Count)
            {
                throw new ArgumentException("Pdf and Index must have the same length.");
            }
            var pdf_normalized = pdf.Select(x => x / pdf.Sum()).ToArray();
            var coin = random.NextDouble();
            double sum = 0;
            for (int i = 0; i < pdf_normalized.Length; i++)
            {
                sum += pdf_normalized[i];
                if (coin < sum)
                {
                    return index[i];
                }
            }
            return index.Last();
        }
    }

    partial class Scalar
    {
        /// <summary>
        /// Returns the scalar value.
        /// </summary>
        /// <param name="random">The random number generator (ignored).</param>
        /// <returns>The scalar value.</returns>
        public override double SampleDistribution(Random random)
        {
            return DistributionParameters.Value;
        }
    }

    partial class NormalDistribution
    {
        /// <summary>
        /// Gets the normal distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A normal distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new ContinuousDistributionWrapper(new Normal(DistributionParameters.Mean, DistributionParameters.Std, random));
        }

        /// <summary>
        /// Samples a value from the normal distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the normal distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class ExponentialDistribution
    {
        /// <summary>
        /// Gets the exponential distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>An exponential distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new ContinuousDistributionWrapper(new Exponential(DistributionParameters.Rate, random));
        }

        /// <summary>
        /// Samples a value from the exponential distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the exponential distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class LogNormalDistribution
    {
        /// <summary>
        /// Gets the log-normal distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A log-normal distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new ContinuousDistributionWrapper(new LogNormal(DistributionParameters.Mean, DistributionParameters.Std, random));
        }

        /// <summary>
        /// Samples a value from the log-normal distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the log-normal distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class GammaDistribution
    {
        /// <summary>
        /// Gets the gamma distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A gamma distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new ContinuousDistributionWrapper(new Gamma(DistributionParameters.Shape, DistributionParameters.Rate, random));
        }

        /// <summary>
        /// Samples a value from the gamma distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the gamma distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class BetaDistribution
    {
        /// <summary>
        /// Gets the beta distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A beta distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new ContinuousDistributionWrapper(new Beta(DistributionParameters.Alpha, DistributionParameters.Beta, random));
        }

        /// <summary>
        /// Samples a value from the beta distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the beta distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class UniformDistribution
    {
        /// <summary>
        /// Gets the uniform distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A uniform distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new ContinuousDistributionWrapper(new ContinuousUniform(DistributionParameters.Min, DistributionParameters.Max, random));
        }

        /// <summary>
        /// Samples a value from the uniform distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the uniform distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class BinomialDistribution
    {
        /// <summary>
        /// Gets the binomial distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A binomial distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new DiscreteDistributionWrapper(new Binomial(DistributionParameters.P, DistributionParameters.N, random));
        }

        /// <summary>
        /// Samples a value from the binomial distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the binomial distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }

    partial class PoissonDistribution
    {
        /// <summary>
        /// Gets the Poisson distribution object.
        /// </summary>
        /// <param name="random">The random number generator to use.</param>
        /// <returns>A Poisson distribution instance.</returns>
        public override IDistribution GetDistribution(Random random)
        {
            return new DiscreteDistributionWrapper(new Poisson(DistributionParameters.Rate, random));
        }

        /// <summary>
        /// Samples a value from the Poisson distribution.
        /// </summary>
        /// <param name="random">The random number generator to use for sampling.</param>
        /// <returns>A sampled value from the Poisson distribution.</returns>
        public override double SampleDistribution(Random random)
        {
            return DrawSample(GetDistribution(random), ScalingParameters, TruncationParameters);
        }
    }


    /// <summary>
    /// Interface for distribution sampling.
    /// </summary>
    public interface IDistribution
    {
        /// <summary>
        /// Generates a single sample from the distribution.
        /// </summary>
        /// <returns>A sampled value.</returns>
        double Sample();

        /// <summary>
        /// Fills an array with samples from the distribution.
        /// </summary>
        /// <param name="arr">The array to fill with samples.</param>
        /// <returns>The array filled with samples.</returns>
        double[] Samples(double[] arr);
    }

    /// <summary>
    /// Wrapper for continuous distributions that implements the <see cref="IDistribution"/> interface.
    /// </summary>
    public class ContinuousDistributionWrapper : IDistribution
    {
        private readonly IContinuousDistribution _distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="ContinuousDistributionWrapper"/> class.
        /// </summary>
        /// <param name="distribution">The continuous distribution to wrap.</param>
        public ContinuousDistributionWrapper(IContinuousDistribution distribution)
        {
            _distribution = distribution;
        }

        /// <summary>
        /// Generates a single sample from the continuous distribution.
        /// </summary>
        /// <returns>A sampled value.</returns>
        public double Sample()
        {
            return _distribution.Sample();
        }

        /// <summary>
        /// Fills an array with samples from the continuous distribution.
        /// </summary>
        /// <param name="arr">The array to fill with samples.</param>
        /// <returns>The array filled with samples.</returns>
        public double[] Samples(double[] arr)
        {
            _distribution.Samples(arr);
            return arr;
        }
    }

    /// <summary>
    /// Wrapper for discrete distributions that implements the <see cref="IDistribution"/> interface.
    /// </summary>
    public class DiscreteDistributionWrapper : IDistribution
    {
        private readonly IDiscreteDistribution _distribution;

        /// <summary>
        /// Initializes a new instance of the <see cref="DiscreteDistributionWrapper"/> class.
        /// </summary>
        /// <param name="distribution">The discrete distribution to wrap.</param>
        public DiscreteDistributionWrapper(IDiscreteDistribution distribution)
        {
            _distribution = distribution;
        }

        /// <summary>
        /// Generates a single sample from the discrete distribution.
        /// </summary>
        /// <returns>A sampled value converted to double.</returns>
        public double Sample()
        {
            return (double)_distribution.Sample();
        }

        /// <summary>
        /// Fills an array with samples from the discrete distribution.
        /// </summary>
        /// <param name="arr">The array to fill with samples.</param>
        /// <returns>The array filled with samples converted to double.</returns>
        public double[] Samples(double[] arr)
        {
            int[] intSamples = new int[arr.Length];
            _distribution.Samples(intSamples);
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (double)intSamples[i];
            }
            return arr;
        }
    }

}
