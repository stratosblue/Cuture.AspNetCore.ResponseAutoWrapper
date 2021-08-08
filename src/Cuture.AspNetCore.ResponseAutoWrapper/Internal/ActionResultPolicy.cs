namespace Microsoft.AspNetCore.Mvc.Filters
{
    /// <summary>
    /// Action Result 处理策略
    /// </summary>
    internal enum ActionResultPolicy
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 处理
        /// </summary>
        Process,

        /// <summary>
        /// 跳过
        /// </summary>
        Skip,
    }
}