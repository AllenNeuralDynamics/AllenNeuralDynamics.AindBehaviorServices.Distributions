using System;
using System.Linq;
using Xunit;
using AllenNeuralDynamics.Behavior.Distributions;

namespace AllenNeuralDynamics.Behavior.Distributions.Tests
{
    public class BinomialDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsBoundedValue()
        {
            var distribution = new BinomialDistribution
            {
                DistributionParameters = new BinomialDistributionParameters { P = 0.5, N = 10 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 0 && sample <= 10);
            }
        }

        [Fact]
        public void SampleDistribution_WithProbabilityZero_ReturnsZero()
        {
            var distribution = new BinomialDistribution
            {
                DistributionParameters = new BinomialDistributionParameters { P = 0.0, N = 10 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(0, sample);
        }

        [Fact]
        public void SampleDistribution_WithProbabilityOne_ReturnsN()
        {
            var distribution = new BinomialDistribution
            {
                DistributionParameters = new BinomialDistributionParameters { P = 1.0, N = 10 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(10, sample);
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new BinomialDistribution
            {
                DistributionParameters = new BinomialDistributionParameters { P = 0.5, N = 10 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<DiscreteDistributionWrapper>(dist);
        }
    }

    public class PoissonDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsNonNegativeValue()
        {
            var distribution = new PoissonDistribution
            {
                DistributionParameters = new PoissonDistributionParameters { Rate = 5.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 0);
            }
        }

        [Fact]
        public void SampleDistribution_WithLowRate_ReturnsSmallValues()
        {
            var distribution = new PoissonDistribution
            {
                DistributionParameters = new PoissonDistributionParameters { Rate = 0.1 }
            };
            var random = new Random(42);
            var samples = new double[100];
            
            for (int i = 0; i < 100; i++)
            {
                samples[i] = distribution.SampleDistribution(random);
            }
            
            var average = samples.Sum() / samples.Length;
            Assert.True(average < 1.0);
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new PoissonDistribution
            {
                DistributionParameters = new PoissonDistributionParameters { Rate = 5.0 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<DiscreteDistributionWrapper>(dist);
        }
    }
}
