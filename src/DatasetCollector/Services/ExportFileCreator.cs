using System.Globalization;
using Coravel.Invocable;
using DatasetCollector.DataBases;

namespace DatasetCollector.Services;

public class ExportFileCreator : IInvocable
{
    private readonly AppDbContext _context;
    private readonly string _directoryPath;

    public ExportFileCreator(AppDbContext context)
    {
        _context = context;
        _directoryPath = Environment.GetEnvironmentVariable("DirectoryForFilesPath")!;
    }
    public async Task Invoke()
    {

        var fileName = DateTime.Now.ToString("dd-MM-yyyy-hh") + ".csv";
        var fullPath = _directoryPath + fileName;


        using (var writer = new StreamWriter(fullPath))
        using (var csvWriter = new CsvHelper.CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            await csvWriter.WriteRecordsAsync(_context.Matches);
        }

        var di = new DirectoryInfo(_directoryPath);
        foreach (var file in di.GetFiles())
        {
            if (file.FullName != fileName)
            {
                file.Delete();
            }
        }
    }
}