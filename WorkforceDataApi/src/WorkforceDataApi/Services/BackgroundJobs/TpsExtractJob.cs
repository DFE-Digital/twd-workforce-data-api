using WorkforceDataApi.Csv;

namespace WorkforceDataApi.Services.BackgroundJobs;

public class TpsExtractJob
{
    private const string DownloadFolderName = "tps-extract";
    private readonly ITpsExtractRemoteStorageService _tpsExtractRemoteStorageService;
    private readonly ITpsCsvProcessor _tpsCsvProcessor;
    private readonly ILocalFilesystem _localFilesystem;
    private readonly ILogger<TpsExtractJob> _logger;

    public TpsExtractJob(
        ITpsExtractRemoteStorageService tpsExtractRemoteStorageService,
        ITpsCsvProcessor tpsCsvProcessor,
        ILocalFilesystem localFilesystem,
        ILogger<TpsExtractJob> logger)
    {
        _tpsExtractRemoteStorageService = tpsExtractRemoteStorageService;
        _tpsCsvProcessor = tpsCsvProcessor;
        _localFilesystem = localFilesystem;
        _logger = logger;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Executing TPS Extract Job.");

        var filenames = await _tpsExtractRemoteStorageService.GetPendingProcessingTpsExtractFilenames(cancellationToken);
        if (filenames != null && filenames.Length > 0)
        {
            _logger.LogInformation("Found {fileCount} TPS extract files in remote storage which are pending processing.", filenames.Length);

            var basePath = _localFilesystem.GetApplicationDataPath();
            var downloadFolder = Path.Combine(basePath, DownloadFolderName);
            var archiveFolder = Path.Combine(downloadFolder, "archive");

            foreach (var filename in filenames)
            {
                _logger.LogInformation("Downloading {filename} from remote storage.", filename);
                var localFilename = await _tpsExtractRemoteStorageService.DownloadTpsExtractFile(filename, downloadFolder, cancellationToken);
                _logger.LogInformation("Done.");
                
                _logger.LogInformation("Processing {filename}.", localFilename);
                await _tpsCsvProcessor.Process(localFilename);
                _logger.LogInformation("Done.");

                _logger.LogInformation("Archiving {filename} in remote storage.", filename);
                await _tpsExtractRemoteStorageService.ArchiveTpsExtractFile(filename, cancellationToken);
                _logger.LogInformation("Done.");

                _localFilesystem.CreateDirectory(archiveFolder);                
                var archiveFilename = Path.Combine(archiveFolder, Path.GetFileName(localFilename));
                _logger.LogInformation("Archiving {filename} in local storage.", localFilename);
                _localFilesystem.Move(localFilename, archiveFilename, true);
                _logger.LogInformation("Done.");
            }            
        }
        else
        {
            _logger.LogInformation("No TPS extract files found in remote storage which are pending processing.");
        }

        _logger.LogInformation("Finished executing TPS Extract Job.");
    }
}
