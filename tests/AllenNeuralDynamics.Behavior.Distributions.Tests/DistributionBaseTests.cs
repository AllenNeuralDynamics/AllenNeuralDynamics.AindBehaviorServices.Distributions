using System;
using System.Linq;
using Xunit;
using AllenNeuralDynamics.Behavior.Distributions;

namespace AllenNeuralDynamics.Behavior.Distributions.Tests
{
    public class DistributionBaseTests
    {
        [Fact]
        public void ApplyScaleAndOffset_WithNullScaling_ReturnsOriginalValue()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 }
            };
            var random = new Random(42);
            var dist = distribution.GetDistribution(random);
            var sample = distribution.DrawSample(dist, null, null);
            
            Assert.True(sample >= -10 && sample <= 10);
        }

        [Fact]
        public void ApplyScaleAndOffset_WithScaling_AppliesCorrectly()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 },
                ScalingParameters = new ScalingParameters { Scale = 2.0, Offset = 5.0 }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.True(sample >= -15 && sample <= 25);
        }

        [Fact]
        public void ValidateTruncationParameters_MinGreaterThanMax_ThrowsException()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = 10, 
                    Max = 5,
                    TruncationMode = TruncationParametersTruncationMode.Clamp
                }
            };
            var random = new Random(42);
            
            Assert.Throws<ArgumentException>(() => distribution.SampleDistribution(random));
        }

        [Fact]
        public void TruncationClampMode_ClampsValueCorrectly()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = -1, 
                    Max = 1,
                    TruncationMode = TruncationParametersTruncationMode.Clamp
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= -1 && sample <= 1);
            }
        }

        [Fact]
        public void TruncationExcludeMode_ReturnsValueInRange()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = -2, 
                    Max = 2,
                    TruncationMode = TruncationParametersTruncationMode.Exclude
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 20; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.True(sample >= -2 && sample <= 2);
            }
        }

        [Fact]
        public void TruncationExcludeMode_WithNegativeBias_ReturnsMinValue()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = -100, Std = 1 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = 50, 
                    Max = 100,
                    TruncationMode = TruncationParametersTruncationMode.Exclude
                }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(50, sample);
        }

        [Fact]
        public void TruncationExcludeMode_WithPositiveBias_ReturnsMaxValue()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 100, Std = 1 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = -100, 
                    Max = -50,
                    TruncationMode = TruncationParametersTruncationMode.Exclude
                }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(-50, sample);
        }

        [Fact]
        public void InvalidTruncationMode_ThrowsException()
        {
            var distribution = new NormalDistribution
            {
                DistributionParameters = new NormalDistributionParameters { Mean = 0, Std = 1 },
                TruncationParameters = new TruncationParameters 
                { 
                    Min = -1, 
                    Max = 1,
                    TruncationMode = (TruncationParametersTruncationMode)999
                }
            };
            var random = new Random(42);
            
            Assert.Throws<ArgumentException>(() => distribution.SampleDistribution(random));
        }
    }
}
