using Bogus;

namespace DavesDartsClub.Fakers;

public abstract class BaseFaker<T> where T : class
{
    public abstract Faker<T> CreateFaker();

    public T GenerateOne() => CreateFaker().Generate();
    public IEnumerable<T> GenerateMany(int count) => CreateFaker().Generate(count);
}
