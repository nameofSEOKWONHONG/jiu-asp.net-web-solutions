using System.ComponentModel;
using System.Reflection.Metadata;

namespace Domain.Enums
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