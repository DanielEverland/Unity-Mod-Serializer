namespace UMS
{
    /// <summary>
    /// Controls how the reflected converter handles member serialization.
    /// </summary>
    public enum MemberSerialization
    {
        /// <summary>
        /// Only members with [SerializeField] or [Property] attributes are
        /// serialized.
        /// </summary>
        OptIn,

        /// <summary>
        /// Only members with [NotSerialized] or [Ignore] will not be
        /// serialized.
        /// </summary>
        OptOut,

        /// <summary>
        /// The default member serialization behavior is applied.
        /// </summary>
        Default
    }
}