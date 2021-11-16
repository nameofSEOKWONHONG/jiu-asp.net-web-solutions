using System.ComponentModel;
using System.Reflection.Metadata;

namespace SharedLibrary.Enums
{
    public enum UPLOAD_TYPE : byte
    {
        [Description(@"Images\Products")]
        Product,

        [Description(@"Images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document
    }
}