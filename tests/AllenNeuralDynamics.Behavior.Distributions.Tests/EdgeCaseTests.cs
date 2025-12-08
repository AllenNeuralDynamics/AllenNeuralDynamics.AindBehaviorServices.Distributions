using System;
using Xunit;
using AllenNeuralDynamics.Behavior.Distributions;

namespace AllenNeuralDynamics.Behavior.Distributions.Tests
{
    public class EdgeCaseTests
    {
        [Fact]
        public void NormalDistribution_WithLargeStd_StillWorks()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1000 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.True(!double.IsInfinity(sample) && !double.IsNaN(sample));
        }

        [Fact]
        public void ExponentialDistribution_WithVerySmallRate_StillWorks()
        {
            var distribution = new ExponentialDistribution
            {
                DistributionParameters = new ExponentialDistributionParameters { Rate = 0.001 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.True(sample >= 0 && !double.IsInfinity(sample) && !double.IsNaN(sample));
        }

        [Fact]
        public void ScalingAndTruncation_CombinedCorrectly()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 },
                ScalingParameters = new ScalingParameters { Scale = 10.0, Offset = 50.0 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = 40, 
                    Max = 60,
                    TruncationMode = TruncationParametersTruncationMode.Clamp
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 40 && sample <= 60);
            }
        }

        [Fact]
        public void BinomialDistribution_WithZeroTrials_ReturnsZero()
        {
            var distribution = new BinomialDistribution
            {
                DistributionParameters = new BinomialDistributionParameters { P = 0.5, N = 0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(0, sample);
        }

        [Fact]
        public void UniformDistribution_WithNegativeRange_WorksCorrectly()
        {
            var distribution = new UniformDistribution
            {
                DistributionParameters = new UniformDistributionParameters { Min = -10.0, Max = -5.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= -10.0 && sample <= -5.0);
            }
        }

        [Fact]
        public void BetaDistribution_WithSymmetricParameters_ApproximatesUniform()
        {
            var distribution = new BetaDistribution
            {
                DistributionParameters = new BetaDistributionParameters { Alpha = 1.0, Beta = 1.0 }
            };
            var random = new Random(42);
            var samples = new double[100];
            
            for (int i = 0; i < 100; i++)
            {
                samples[i] = distribution.SampleDistribution(random);
                Assert.True(samples[i] >= 0 && samples[i] <= 1);
            }
        }

        [Fact]
        public void GammaDistribution_WithSmallShape_StillWorks()
        {
            var distribution = new GammaDistribution
            {
                DistributionParameters = new GammaDistributionParameters { Shape = 0.1, Rate = 1.0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.True(sample >= 0 && !double.IsInfinity(sample) && !double.IsNaN(sample));
        }

        [Fact]
        public void LogNormalDistribution_WithNegativeMean_StillPositive()
        {
            var distribution = new LogNormalDistribution
            {
                DistributionParameters = new LogNormalDistributionParameters { Mean = -2, Std = 1 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample > 0);
            }
        }

        [Fact]
        public void TruncationClamp_WithVeryNarrowRange_WorksCorrectly()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 10 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = 0.0, 
                    Max = 0.01,
                    TruncationMode = TruncationParametersTruncationMode.Clamp
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= 0.0 && sample <= 0.01);
            }
        }

        [Fact]
        public void NegativeScaling_WorksCorrectly()
        {
            var distribution = new UniformDistribution
            {
                DistributionParameters = new UniformDistributionParameters { Min = 0, Max = 1 },
                ScalingParameters = new ScalingParameters { Scale = -10.0, Offset = 5.0 }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= -5.0 && sample <= 5.0);
            }
        }

        [Fact]
        public void PdfDistribution_WithAllZeroWeights_ThrowsOrHandlesGracefully()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new System.Collections.Generic.List<double> { 0.0, 0.0, 0.0 },
                    Index = new System.Collections.Generic.List<double> { 1.0, 2.0, 3.0 }
                }
            };
            var random = new Random(42);
            
            var sample = distribution.SampleDistribution(random);
            Assert.Equal(3.0, sample);
        }

        [Fact]
        public void PoissonDistribution_WithVeryHighRate_StillWorks()
        {
            var distribution = new PoissonDistribution
            {
                DistributionParameters = new PoissonDistributionParameters { Rate = 100.0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.True(sample >= 0 && !double.IsInfinity(sample) && !double.IsNaN(sample));
        }
    }
}
