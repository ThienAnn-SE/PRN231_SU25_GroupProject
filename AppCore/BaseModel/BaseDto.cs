namespace AppCore.BaseModel
{
    /// <summary>
    /// Base data transfer object with common properties
    /// </summary>
    public class BaseDto
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public Guid Id { get; set; }
        
        /// <summary>
        /// When the entity was created
        /// </summary>
        public DateTime CreatedAt { get; set; }
        
        /// <summary>
        /// When the entity was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; }
        
        /// <summary>
        /// Identifier of the creator (if available)
        /// </summary>
        public Guid? CreatorId { get; set; }
        
        /// <summary>
        /// Identifier of the last editor (if available)
        /// </summary>
        public Guid? EditorId { get; set; }
    }
}