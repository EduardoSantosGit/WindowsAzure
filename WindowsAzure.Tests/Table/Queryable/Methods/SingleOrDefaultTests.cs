﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WindowsAzure.Table.Queryable.Expressions;
using WindowsAzure.Table.Queryable.Expressions.Methods;
using WindowsAzure.Tests.Samples;
using Xunit;

namespace WindowsAzure.Tests.Table.Queryable.Methods
{
    public sealed class SingleOrDefaultTests
    {
        private readonly IQueryable<Country> _countries;
        private readonly Dictionary<string, string> _nameChanges;

        public SingleOrDefaultTests()
        {
            _countries = new EnumerableQuery<Country>(new Country[] {});
            _nameChanges = new Dictionary<string, string>
                {
                    {"Continent", "PartitionKey"},
                    {"Name", "RowKey"}
                };
        }

        [Fact]
        public void SingleOrDefault()
        {
            // Arrange
            var translator = new SingleOrDefaultTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.SingleOrDefault(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        // ReSharper disable ReplaceWithSingleCallToSingleOrDefault

        [Fact]
        public void SingleOrDefaultAfterWhere()
        {
            // Arrange
            var translator = new SingleOrDefaultTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.Where(p => !p.IsExists).SingleOrDefault();
            var translation = new TranslationResult();

            // Act
            translator.Translate((MethodCallExpression) query.Body, translation);

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.NotNull(translation.TableQuery.FilterString);
            Assert.Equal("not IsExists", translation.TableQuery.FilterString);
        }

        // ReSharper restore ReplaceWithSingleCallToSingleOrDefault

        [Fact]
        public void SingleOrDefaultWithInvalidMethod()
        {
            // Arrange
            var translator = new SingleOrDefaultTranslator(_nameChanges);
            Expression<Func<Country>> query = () => _countries.Single(p => !p.IsExists);
            var translation = new TranslationResult();

            // Act
            Assert.Throws<ArgumentOutOfRangeException>(() => translator.Translate((MethodCallExpression) query.Body, translation));

            // Assert
            Assert.NotNull(translation.TableQuery);
            Assert.Null(translation.TableQuery.FilterString);
            Assert.Null(translation.PostProcessing);
        }
    }
}