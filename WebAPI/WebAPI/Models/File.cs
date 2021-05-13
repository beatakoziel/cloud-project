using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebAPI.Models
{
    public class File
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonElement("name")]
        public string Name { get; set; }
        [BsonElement("contentType")]
        public string ContentType { get; set; }
        [BsonElement("source")]
        public string Path { get; set; }
        [BsonElement("path")]
        public string Source { get; set; }
        [BsonElement("isCurrent")]
        public bool IsCurrent { get; set; }
        [BsonElement("parentId")]
        public string ParentId { get; set; }
        [BsonElement("createdDate")]
        public BsonDateTime CreatedDate { get; set; }
    }
}
