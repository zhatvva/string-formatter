using StringFormatter.Core.Services;

namespace StringFormatter.Tests
{
    public class StringFormatterServiceTests
    {
        private readonly TestClass _testClass = new();

        [Theory]
        [InlineData("{Age}")]
        [InlineData("{AgeField}")]
        public void ValidMemberName(string memberName)
        {
            var result = StringFormatterService.Shared.Format($"{memberName}", _testClass);
            result.Should().Be("10");
        }

        [Fact]
        public void CollectionAccess()
        {
            var result = StringFormatterService.Shared.Format("{AgeArray[0]}", _testClass);
            result.Should().Be("10");
        }

        [Theory]
        [InlineData("{{Age}")]
        [InlineData("{Age}}")]
        public void InvalidCurlyBracesCount(string template)
        {
            var result = () => StringFormatterService.Shared.Format(template, _testClass);
            result.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void IncorrectIdentifier()
        {
            var result = () => StringFormatterService.Shared.Format("{WrongAgeField[0]}", _testClass);
            result.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void InvalidCollectionExpressionIndex()
        {
            var result = () => StringFormatterService.Shared.Format("{AgeField[0a]}", _testClass);
            result.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Escaping()
        {
            var result = StringFormatterService.Shared.Format("{{{AgeArray[0]}}}", _testClass);
            result.Should().Be("{10}");
        }
    }
}