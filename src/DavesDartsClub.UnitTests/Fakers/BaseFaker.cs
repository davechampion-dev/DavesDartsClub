using Bogus;

namespace DavesDartsClub.UnitTests.Fakers
{
    public abstract class BaseFaker<T> where T : class
    {
        protected readonly Faker Faker;

        protected BaseFaker()
        {
            Faker = new Faker();
        }

        public abstract Faker<T> CreateFaker();

        public T GenerateOne() => CreateFaker().Generate();
        public IEnumerable<T> GenerateMany(int count) => CreateFaker().Generate(count);
    }
}
