using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using AllenNeuralDynamics.Behavior.Distributions;

namespace AllenNeuralDynamics.Behavior.Distributions.Tests
{
    public class ScalarTests
    {
        [Fact]
        public void SampleDistribution_ReturnsScalarValue()
        {
            var distribution = new Scalar
            {
                DistributionParameters = new ScalarDistributionParameter { Value = 42.0 }
            };
            var random = new Random();
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(42.0, sample);
        }

        [Fact]
        public void SampleDistribution_ConsistentAcrossMultipleCalls()
        {
            var distribution = new Scalar
            {
                DistributionParameters = new ScalarDistributionParameter { Value = 3.14 }
            };
            var random = new Random();
            
            for (int i = 0; i < 10; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.Equal(3.14, sample);
            }
        }

        [Fact]
        public void SampleDistribution_WithNegativeValue_ReturnsNegativeValue()
        {
            var distribution = new Scalar
            {
                DistributionParameters = new ScalarDistributionParameter { Value = -10.5 }
            };
            var random = new Random();
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(-10.5, sample);
        }

        [Fact]
        public void SampleDistribution_WithZero_ReturnsZero()
        {
            var distribution = new Scalar
            {
                DistributionParameters = new ScalarDistributionParameter { Value = 0 }
            };
            var random = new Random();
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(0, sample);
        }
    }

    public class PdfDistributionTests
    {
        [Fact]
        public void SampleDistribution_ReturnsSingleIndexForSinglePdf()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 1.0 },
                    Index = new List<double> { 42.0 }
                }
            };
            var random = new Random(42);
            var sample = distribution.SampleDistribution(random);
            
            Assert.Equal(42.0, sample);
        }

        [Fact]
        public void SampleDistribution_ReturnsValueFromIndex()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 1.0, 1.0, 1.0 },
                    Index = new List<double> { 10.0, 20.0, 30.0 }
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.Contains(sample, new[] { 10.0, 20.0, 30.0 });
            }
        }

        [Fact]
        public void SampleDistribution_WithUnequalWeights_FavorsHigherWeights()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 1.0, 99.0 },
                    Index = new List<double> { 1.0, 2.0 }
                }
            };
            var random = new Random(42);
            var samples = new double[1000];
            
            for (int i = 0; i < 1000; i++)
            {
                samples[i] = distribution.SampleDistribution(random);
            }
            
            var countOfTwo = samples.Count(s => s == 2.0);
            Assert.True(countOfTwo > 900);
        }

        [Fact]
        public void SampleDistribution_MismatchedLengths_ThrowsException()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 1.0, 1.0 },
                    Index = new List<double> { 10.0 }
                }
            };
            var random = new Random(42);
            
            Assert.Throws<ArgumentException>(() => distribution.SampleDistribution(random));
        }

        [Fact]
        public void SampleDistribution_NormalizesPdf()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 2.0, 2.0, 2.0 },
                    Index = new List<double> { 1.0, 2.0, 3.0 }
                }
            };
            var random = new Random(42);
            var samples = new double[300];
            
            for (int i = 0; i < 300; i++)
            {
                samples[i] = distribution.SampleDistribution(random);
            }
            
            var count1 = samples.Count(s => s == 1.0);
            var count2 = samples.Count(s => s == 2.0);
            var count3 = samples.Count(s => s == 3.0);
            
            Assert.True(Math.Abs(count1 - count2) < 50);
            Assert.True(Math.Abs(count2 - count3) < 50);
        }

        [Fact]
        public void SampleDistribution_WithZeroWeight_NeverReturnsIndex()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 1.0, 0.0, 1.0 },
                    Index = new List<double> { 1.0, 2.0, 3.0 }
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 100; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.NotEqual(2.0, sample);
            }
        }

        [Fact]
        public void SampleDistribution_ReturnsLastIndexForEdgeCase()
        {
            var distribution = new PdfDistribution
            {
                DistributionParameters = new PdfDistributionParameters
                {
                    Pdf = new List<double> { 0.0, 0.0, 1.0 },
                    Index = new List<double> { 1.0, 2.0, 3.0 }
                }
            };
            var random = new Random(42);
            
            for (int i = 0; i < 10; i++)
            {
                var sample = distribution.SampleDistribution(random);
                Assert.Equal(3.0, sample);
            }
        }
    }
}
