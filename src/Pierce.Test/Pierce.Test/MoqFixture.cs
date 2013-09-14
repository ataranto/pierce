using System;
using Moq;

namespace Pierce.Test
{
    public class MoqFixture
    {
        private MockRepository _repository;

        public MoqFixture()
        {
            _repository = new MockRepository(MockBehavior.Default);
            _repository.DefaultValue = DefaultValue.Mock;
        }

        public Mock<T> CreateMock<T>(MockBehavior behavior = MockBehavior.Default)
            where T : class
        {
            return _repository.Create<T>(behavior);
        }

        public void Dispose()
        {
            _repository.VerifyAll();
        }
    }
}

