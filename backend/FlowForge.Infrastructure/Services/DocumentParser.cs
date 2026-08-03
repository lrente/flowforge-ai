using System.Text;
using DocumentFormat.OpenXml.Packaging;
using FlowForge.Application.Interfaces;
using FlowForge.Domain.Entities;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace FlowForge.Infrastructure.Services;

public sealed class DocumentParser : IDocumentParser
{
    public Task<string> ExtractTextAsync(KnowledgeDocument document, CancellationToken cancellationToken = default)
    {
        var extension = Path.GetExtension(document.FileName).ToLowerInvariant();

        return extension switch
        {
            ".pdf" => Task.FromResult(ExtractPdfText(document.StoragePath)),
            ".docx" => Task.FromResult(ExtractDocxText(document.StoragePath)),
            ".txt" => Task.FromResult(File.ReadAllText(document.StoragePath)),
            ".md" => Task.FromResult(File.ReadAllText(document.StoragePath)),
            _ => Task.FromResult(string.Empty)
        };
    }

    private static string ExtractPdfText(string path)
    {
        try
        {
            using var reader = new PdfReader(path);
            using var pdf = new PdfDocument(reader);
            var text = new StringBuilder();

            for (var page = 1; page <= pdf.GetNumberOfPages(); page++)
            {
                var pageText = PdfTextExtractor.GetTextFromPage(pdf.GetPage(page));
                if (!string.IsNullOrWhiteSpace(pageText))
                {
                    text.AppendLine(pageText);
                }
            }

            return text.ToString();
        }
        catch
        {
            return string.Empty;
        }
    }

    private static string ExtractDocxText(string path)
    {
        try
        {
            using var document = WordprocessingDocument.Open(path, false);
            var body = document.MainDocumentPart?.Document.Body;
            if (body is null)
            {
                return string.Empty;
            }

            return string.Join(Environment.NewLine, body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>().Select(t => t.Text));
        }
        catch
        {
            return string.Empty;
        }
    }
}
