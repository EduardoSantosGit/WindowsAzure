﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Expressions
{
    public sealed class ConstrantEvaluatorTests
    {
        // ReSharper disable ConvertToConstant.Local

        [Fact]
        public void ClosureOnLocalVariableTest()
        {
            // Arrange
            string value = "String value";
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => value;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(((ConstantExpression) result).Value, value);
        }

        [Fact]
        public void ClosureOnSubstringOfLocalVariableTest()
        {
            // Arrange
            string value = "String value";
            var evaluator = new ExpressionEvaluator();
            const int substringIndex = 7;
            Expression<Func<string>> lambda = () => value.Substring(substringIndex);

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(((ConstantExpression) result).Value, value.Substring(substringIndex));
        }

        [Fact]
        public void ClosureOnEnumValueTest()
        {
            // Arrange
            var country = Countries.Finland;
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => country.ToString();

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(((ConstantExpression) result).Value, country.ToString());
        }

        [Fact]
        public void ClosureOnLocalVariableTwiceTest()
        {
            // Arrange
            string value = "String value";
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => value;

            // Act
            Expression result1 = evaluator.Evaluate(lambda.Body);
            Expression result2 = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result1);
            Assert.IsType<ConstantExpression>(result2);
            Assert.Equal(((ConstantExpression) result1).Value, value);
            Assert.Equal(((ConstantExpression) result2).Value, value);
        }

        // ReSharper restore ConvertToConstant.Local

        [Fact]
        public void ClosureOnLocalConstantTest()
        {
            // Arrange
            const long value = 0xfefefe;
            var evaluator = new ExpressionEvaluator();
            Expression<Func<long>> lambda = () => value;

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(((ConstantExpression) result).Value, value);
        }

        [Fact]
        public void ClosureOnForeachLoopValueTest()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            var data = new byte[] {0x11, 0x22, 0x33};
            var values = new List<object>(data.Length);

            // Act
            // ReSharper disable AccessToForEachVariableInClosure
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (byte value in data)

            {
                Expression<Func<byte>> lambda = () => value;
                var result = (ConstantExpression) evaluator.Evaluate(lambda.Body);
                values.Add(result.Value);
            }
            // ReSharper restore LoopCanBeConvertedToQuery
            // ReSharper restore AccessToForEachVariableInClosure

            // Assert
            Assert.Equal(values.Count, data.Length);
            foreach (byte value in data)
            {
                Assert.Contains(value, values);
            }
        }

        [Fact]
        public void ClosureOnFunctionValueTest()
        {
            // Arrange
            var evaluator = new ExpressionEvaluator();
            Expression<Func<string>> lambda = () => GetTestString();

            // Act
            Expression result = evaluator.Evaluate(lambda.Body);

            // Assert
            Assert.IsType<ConstantExpression>(result);
            Assert.Equal(((ConstantExpression) result).Value, GetTestString());
        }

        private string GetTestString()
        {
            return "Test string";
        }
    }
}