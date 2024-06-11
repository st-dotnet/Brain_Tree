using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using VT.Web.Interfaces; 
namespace VT.Web.Components
{
    public class AmazonFileUpload : IAmazonFileUpload
    {

        #region Field(s)

        private const string BucketName = "verifyteck";
        private const string AccessKey = "AKIAIM5TSADPNSAXTKBA";
        private const string SecretKey = "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";

        #endregion

        public string UploadFile(HttpPostedFileBase file)
        {
            string fileName = string.Format("{0}{1}", Guid.NewGuid(), Path.GetExtension(file.FileName));

            if (file.ContentLength > 0) // accept the file
            {
                var config = new AmazonS3Config
                {
                    RegionEndpoint = RegionEndpoint.USWest2
                };

                using (IAmazonS3 s3Client = new AmazonS3Client(AccessKey,SecretKey, config))             
                {                             
                    var request = new PutObjectRequest
                    {
                        BucketName = BucketName,
                        Key = string.Format("workflowimages/ORG-{0}_EMP-{1}_CUST-{2}_TIME-{3}_{4}", 1,12,34,DateTime.UtcNow.Ticks, fileName),   //{0}", fileName) 
                        CannedACL = S3CannedACL.PublicRead,
                        InputStream = file.InputStream
                    };                   
                    // Make service call and get back the response.                 
                    PutObjectResponse response = s3Client.PutObject(request);  
                } 
            }
            return fileName;
        }


        public bool DeleteMultipleFiles(List<string> keys)
        {
            var flag = false;
            var multiObjectDeleteRequest = new DeleteObjectsRequest { BucketName = BucketName };

            foreach (var key in keys)
            {
                multiObjectDeleteRequest.AddKey(key, null); // version ID is null.
            }
            
            try
            {
                using (IAmazonS3 s3Client = new AmazonS3Client(AccessKey, SecretKey, RegionEndpoint.USWest2))
                {
                    DeleteObjectsResponse response = s3Client.DeleteObjects(multiObjectDeleteRequest);
                    flag = true;
                }
            }
            catch (DeleteObjectsException e)
            {
                // Process exception.
            }
            return flag;
        }
    }
}