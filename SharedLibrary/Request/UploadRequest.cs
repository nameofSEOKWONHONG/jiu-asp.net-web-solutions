using SharedLibrary.Enums;

namespace SharedLibrary.Request
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public UPLOAD_TYPE UploadType { get; set; }
        public byte[] Data { get; set; }
    }
}