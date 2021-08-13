using Cuture.AspNetCore.ResponseAutoWrapper;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// ResponseAutoWrapperOptions拓展
    /// </summary>
    public static class ResponseAutoWrapperOptionsExtensions
    {
        /// <summary>
        /// 使用指定的 <see cref="IWrapper"/>
        /// </summary>
        /// <typeparam name="TWrapper"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ResponseAutoWrapperOptions UseWrapper<TWrapper>(this ResponseAutoWrapperOptions options)
            where TWrapper : IWrapper
        {
            options.Wrappers.Add(typeof(TWrapper));
            return options;
        }
    }
}