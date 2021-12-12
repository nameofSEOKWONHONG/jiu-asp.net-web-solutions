using Domain.Enums;

namespace Application.Request
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public ENUM_UPLOAD_TYPE UploadType { get; set; }
        public byte[] Data { get; set; }
    }
}