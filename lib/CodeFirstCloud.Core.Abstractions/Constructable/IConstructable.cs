namespace CodeFirstCloud.Constructable;

public interface IConstructable<in TIn, out TOut>
{
    static abstract TOut CreateTestable(TIn input);
}