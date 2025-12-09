using System;
using Xunit;
using AllenNeuralDynamics.AindBehaviorServices.Distributions;
using MathNet.Numerics.Distributions;

namespace AllenNeuralDynamics.AindBehaviorServices.Distributions.Tests
{
    public class DistributionWrapperTests
    {
        [Fact]
        public void ContinuousDistributionWrapper_Sample_ReturnsValue()
        {
            var random = new Random(42);
            var normalDist = new Normal(0, 1, random);
            var wrapper = new ContinuousDistributionWrapper(normalDist);
            
            var sample = wrapper.Sample();
            Assert.True(sample >= -10 && sample <= 10);
        }

        [Fact]
        public void ContinuousDistributionWrapper_Samples_FillsArray()
        {
            var random = new Random(42);
            var normalDist = new Normal(0, 1, random);
            var wrapper = new ContinuousDistributionWrapper(normalDist);
            
            var samples = new double[100];
            var result = wrapper.Samples(samples);
            
            Assert.Equal(100, result.Length);
            Assert.All(result, s => Assert.True(s >= -10 && s <= 10));
        }

        [Fact]
        public void DiscreteDistributionWrapper_Sample_ReturnsValue()
        {
            var random = new Random(42);
            var binomialDist = new Binomial(0.5, 10, random);
            var wrapper = new DiscreteDistributionWrapper(binomialDist);
            
            var sample = wrapper.Sample();
            Assert.True(sample >= 0 && sample <= 10);
        }

        [Fact]
        public void DiscreteDistributionWrapper_Samples_FillsArray()
        {
            var random = new Random(42);
            var binomialDist = new Binomial(0.5, 10, random);
            var wrapper = new DiscreteDistributionWrapper(binomialDist);
            
            var samples = new double[100];
            var result = wrapper.Samples(samples);
            
            Assert.Equal(100, result.Length);
            Assert.All(result, s => Assert.True(s >= 0 && s <= 10));
        }

        [Fact]
        public void DiscreteDistributionWrapper_ConvertsIntToDouble()
        {
            var random = new Random(42);
            var poissonDist = new Poisson(5.0, random);
            var wrapper = new DiscreteDistributionWrapper(poissonDist);
            
            var sample = wrapper.Sample();
            Assert.IsType<double>(sample);
        }
    }
}
