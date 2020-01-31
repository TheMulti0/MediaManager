using System.Threading.Tasks;
using Xunit;

namespace MediaManager.Tests
{
    public class PostOperationValidatorTests
    {
        private readonly IPostOperationValidator _validator = new PostOperationValidator();

        [Fact]
        public void TestWithNegativeNumbers()
        {
            _validator.UserOperatedOnPost(-1, -1);
            _validator.UserOperatedOnPost(0, 0);
            _validator.HasUserOperatedOnPost(-1, -1);
            _validator.HasUserOperatedOnPost(0, 0);
        }

        [Fact]
        public void TestUserOperationFlow()
        {
            _validator.UserOperatedOnPost(1, 1);
            Assert.True(_validator.HasUserOperatedOnPost(1, 1));
            Assert.False(_validator.HasUserOperatedOnPost(1, 2));
            Assert.False(_validator.HasUserOperatedOnPost(0, 1));
        }

        [Fact]
        public void TestConcurrency()
        {
            void Operate(long id)
            {
                _validator.HasUserOperatedOnPost(0, 0);
                _validator.UserOperatedOnPost(0, 0);
            }

            Task.Run(() => Operate(1));
            Task.Run(() => Operate(2));
            Task.Run(() => Operate(3));
        }
    }
}