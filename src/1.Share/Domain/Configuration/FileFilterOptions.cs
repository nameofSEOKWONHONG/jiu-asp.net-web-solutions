namespace Domain.Configuration;

public class FileFilterOptions
{
    public FileFilter[] FileFilters { get; set; }
}

public record FileFilter(string Code, string Name, string[] Extensions);

/*
JsonSetting : 
{
  "FileFilterOptions": {
    "FileFilters": [
      {
        "Code": "PPT"
        "Name": "POWER POINT",
        "Extensions": [
          "ppt",
          "pptx"          
        ]
      },
      {
        "Code": "WORD",
        "Name:" "WORD",
        "Extensions": [
          "doc",
          "docx"
        ]
      },
      {
        "Code": "IMG",
        "Name": "Image",
        "Extensions": [
          "png",
          "jpeg",
          "jpg",
          "tiff",
          "tif",
          "gif"
        ]
      }
    ]
  }    
}
*/