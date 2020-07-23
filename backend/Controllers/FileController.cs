﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using backend.Interfaces;
using backend.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            try
            {
                List<FileViewModel> files = await _fileService.GetFiles();
                return Ok(files);
            }

            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFilesInsideFolder([FromQuery] int folderId)
        {
            try
            {
                List<FileViewModel> files = await _fileService.GetFilesInsideFolder(folderId);
                return Ok(files);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPost]
        public async Task<IActionResult> UploadFiles([FromForm] IFormFileCollection files, [FromQuery] string userData, [FromQuery] int creatorId, [FromQuery] int folderId)
        {
            try
            {
                await _fileService.UploadFiles(files, userData, creatorId, folderId);             
                return Ok();
            }

            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFiles([FromQuery] int[] fileIds, [FromQuery] string userData)
        {
            try
            {
                await _fileService.DeleteFiles(fileIds, userData);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile([FromQuery] int fileId)
        {
            try
            {
                FileStream file = await _fileService.DownloadFile(fileId);
                return Ok(file);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFile([FromBody] FileViewModel file)
        {
            try
            {
                await _fileService.UpdateFile(file);
                return Ok();
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFilePath([FromQuery] int fileId)
        {
            try
            {
                string filePath = await _fileService.GetFilePath(fileId);
                return Ok(filePath);
            }

            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}