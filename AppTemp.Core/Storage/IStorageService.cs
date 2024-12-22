using AppTemp.Core.Storage.File;
using AppTemp.Core.Storage.File.Features;

namespace AppTemp.Core.Storage;

public interface IStorageService
{
    public Task<Uri> UploadAsync<T>(FileUploadCommand? request, FileType supportedFileType, CancellationToken cancellationToken = default)
    where T : class;

    public void Remove(Uri? path);
}
