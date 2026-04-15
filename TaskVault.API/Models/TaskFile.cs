using System;

namespace TaskVault.API.Models;

public class TaskFile
{
    public int Id { get; set; }
    public int TaskId { get; set; }

    public string OriginalName { get; set; } = string.Empty;
    public string StoredName {  get; set; } = string.Empty;
    public int FileSize { get; set; }
    public string MimeType { get; set; } = string.Empty;

}
