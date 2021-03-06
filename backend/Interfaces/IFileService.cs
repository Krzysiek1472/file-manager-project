﻿using backend.Controllers;
using backend.Models;
using backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace backend.Interfaces
{
    public interface IFileService
    {
        Task<List<FileViewModel>> GetFiles();
        Task<bool> UploadFiles(IFormFileCollection files, string userData, int creatorId, int folderId);
        Task<FileStream> DownloadFile(int id, FileController fileController);
        Task UpdateFile(FileViewModel file);
        Task<List<FileViewModel>> GetFilesInsideFolder(int folderId);
        Task DeleteFiles(int[] ids, string userData);
        Task DeleteFiles(List<FileModel> files);
        Task<Guid> CreateShareableLink(int fileId, string filePassword);
        Task<ShareableLinkModel> GetShareableLink(Guid linkId);
        Task<FileStream> ShowFilePreview(int fileId, FileController fileController);
        Task<long> GetSpaceOccupiedByFiles();
    }
}
