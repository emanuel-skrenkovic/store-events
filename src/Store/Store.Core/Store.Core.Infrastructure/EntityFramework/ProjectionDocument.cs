using System;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class ProjectionDocument
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string Data { get; set; }
    }
}