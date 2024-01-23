using AutoMapper;
using familyMart.Connection;
using familyMart.Interface;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.IO;
using System.Threading.Tasks;
using familyMart.Model;

namespace familyMart.Repository;

public class S3Repository : IS3Repository
{
    protected string _source;
    private readonly BasicDatabase _dbContext;
    private readonly IMapper _mapper;

    public S3Repository(BasicDatabase dbContext, IMapper mapper)
    {
        _source = GetType().Name;
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _mapper = mapper;
    }


    public async Task<string> UploadPhotoToS3(UploadS3Data data)
    {
        var credentials = new BasicAWSCredentials(data.AccesKey, data.SecretKey);
        var config = new AmazonS3Config
        {
            ServiceURL = data.ServiceUrl,
        };

        using (var s3Client = new AmazonS3Client(credentials, config))
        {
            var fileTransferUtility = new TransferUtility(s3Client);

            try
            {
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    InputStream = data.FileStream,
                    Key = data.FileName,
                    BucketName = data.BucketName,
                    CannedACL = S3CannedACL.PublicRead
                };

                await fileTransferUtility.UploadAsync(fileTransferUtilityRequest);

                var fileUrl = $"{data.BucketName}/{data.FileName}";
                return fileUrl;
            }
            catch (AmazonS3Exception ex)
            {
                throw ex;
            }
        }
    }
}