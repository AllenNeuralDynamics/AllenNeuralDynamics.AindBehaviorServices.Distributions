using System;
using System.Linq;
using Xunit;
using AllenNeuralDynamics.Behavior.Distributions;

namespace AllenNeuralDynamics.Behavior.Distributions.Tests
{
    public class NormalDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsValueInExpectedRange()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 10, Std = 0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.True(sample >= 0 && sample <= 20);
        }

        [Fact]
        public void SampleDistribution_WithZeroStd_ReturnsOnlyMean()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 10, Std = 0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(10, sample);
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<ContinuousDistributionWrapper>(dist);
        }
    }

    public class ExponentialDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsPositiveValue()
        {
            var distribution = new ExponentialDistribution
            {
                DistributionParameters = new ExponentialDistributionParameters { Rate = 1.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 0);
            }
        }

        [Fact]
        public void SampleDistribution_WithHighRate_ReturnsSmallValues()
        {
            var distribution = new ExponentialDistribution
            {
                DistributionParameters = new ExponentialDistributionParameters { Rate = 10.0 }
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
            var distribution = new ExponentialDistribution
            {
                DistributionParameters = new ExponentialDistributionParameters { Rate = 1.0 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<ContinuousDistributionWrapper>(dist);
        }
    }

    public class LogNormalDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsPositiveValue()
        {
            var distribution = new LogNormalDistribution
            {
                DistributionParameters = new LogNormalDistributionParameters { Mean = 0, Std = 1 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample > 0);
            }
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new LogNormalDistribution
            {
                DistributionParameters = new LogNormalDistributionParameters { Mean = 0, Std = 1 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<ContinuousDistributionWrapper>(dist);
        }
    }

    public class GammaDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsPositiveValue()
        {
            var distribution = new GammaDistribution
            {
                DistributionParameters = new GammaDistributionParameters { Shape = 2.0, Rate = 1.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 0);
            }
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new GammaDistribution
            {
                DistributionParameters = new GammaDistributionParameters { Shape = 2.0, Rate = 1.0 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<ContinuousDistributionWrapper>(dist);
        }
    }

    public class BetaDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsBoundedValue()
        {
            var distribution = new BetaDistribution
            {
                DistributionParameters = new BetaDistributionParameters { Alpha = 2.0, Beta = 5.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 0 && sample <= 1);
            }
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new BetaDistribution
            {
                DistributionParameters = new BetaDistributionParameters { Alpha = 2.0, Beta = 5.0 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<ContinuousDistributionWrapper>(dist);
        }
    }

    public class UniformDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsBoundedValue()
        {
            var distribution = new UniformDistribution
            {
                DistributionParameters = new UniformDistributionParameters { Min = 5.0, Max = 10.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 5.0 && sample <= 10.0);
            }
        }

        [Fact]
        public void SampleDistribution_WithEqualMinMax_ReturnsConstant()
        {
            var distribution = new UniformDistribution
            {
                DistributionParameters = new UniformDistributionParameters { Min = 7.0, Max = 7.0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(7.0, sample, 5);
        }

        [Fact]
        public void GetDistribution_ReturnsValidDistribution()
        {
            var distribution = new UniformDistribution
            {
                DistributionParameters = new UniformDistributionParameters { Min = 0, Max = 1 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            
            Assert.NotNull(dist);
            Assert.IsType<ContinuousDistributionWrapper>(dist);
        }
    }
}
