namespace Application;

public interface IOfficeConverter
{
    string ConvertHtml(string fileName);
    void ConvertPdf(string fileName);
}