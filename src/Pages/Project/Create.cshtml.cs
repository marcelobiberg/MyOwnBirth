﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyPortfolio.Data;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using MyPortfolio.Services.Upload;
using Microsoft.AspNetCore.Http;
using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;

namespace MyPortfolio.Pages.Project
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;

        public CreateModel(ApplicationDbContext context, IHostingEnvironment environment)
        {
            _environment = environment;
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [Required(ErrorMessage = "Título obrigatório")]
        [Display(Name = "Título")]
        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres!")]
        public string Title { get; set; }

        [MaxLength(600, ErrorMessage = "Máximo 600 caracteres!")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        [MaxLength(300, ErrorMessage = "Máximo 300 caracteres!")]
        [Display(Name = "Curta descrição")]
        public string ShortDescription { get; set; }

        [MaxLength(150, ErrorMessage = "Máximo 150 caracteres!")]
        [Display(Name = "Imagem")]
        public string ImagePath { get; set; }

        [MaxLength(150, ErrorMessage = "Máximo 150 caracteres!")]
        [Display(Name = "Url do projeto")]
        public string UrlProject { get; set; }

        [MaxLength(100, ErrorMessage = "Máximo 100 caracteres!")]
        [Display(Name = "Url do Github")]
        public string UrlGithub { get; set; }

        [BindProperty]
        public Data.Project Project { get; set; }

        public async Task UploadFileToS3(IFormFile file)
        {
            using (var client = new AmazonS3Client("ASIAUMDIRDA44BKH3D5V", "HClz97bFcQk0Hza5bkaf1g7FhOpmFPEEQ4H25gvi", RegionEndpoint.USEast1))
            {
                using (var newMemoryStream = new MemoryStream())
                {
                    file.CopyTo(newMemoryStream);

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = file.FileName,
                        BucketName = "bukectmyportfolio",
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
            }
        }

        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var newFileName = string.Empty;
            //Se maior que zero, user selecionou File ( Img )
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                var fileName = string.Empty;
                string PathDB = string.Empty;

                var files = HttpContext.Request.Form.Files;

                foreach (var file in files)
                {
                    if (file.Length > 0)
                    {

                        //Getting FileName
                        fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');

                        //Assigning Unique Filename (Guid)
                        var myUniqueFileName = Convert.ToString(Guid.NewGuid());

                        //Getting file Extension
                        var FileExtension = Path.GetExtension(fileName);

                        // concating  FileName + FileExtension
                        newFileName = myUniqueFileName + FileExtension;

                        // Combines two strings into a path.
                        fileName = Path.Combine(_environment.WebRootPath, "img/Project") + $@"\{newFileName}";

                        // if you want to store path of folder in database
                        PathDB = "img/Project/" + newFileName;
                        Project.ImagePath = PathDB;

                        //await UploadFileToS3(file);

                        using (FileStream fs = System.IO.File.Create(fileName))
                        {
                            file.CopyTo(fs);
                            fs.Flush();
                        }
                    }
                }
            }
            else
            {
                Project.ImagePath = "img/home.jpg";
            }

            _context.Project.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}