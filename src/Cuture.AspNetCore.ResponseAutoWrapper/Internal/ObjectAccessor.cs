namespace Cuture.AspNetCore.ResponseAutoWrapper.Internal;

internal class ObjectAccessor<T>
{
    #region Public 属性

    public T? Value { get; set; }

    #endregion Public 属性

    #region Public 构造函数

    public ObjectAccessor()
    {
    }

    public ObjectAccessor(T? value)
    {
        Value = value;
    }

    #endregion Public 构造函数
}
