using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MyPortfolio.Models;
using Microsoft.AspNetCore.Identity;

namespace MyPortfolio.Data
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(100, ErrorMessage = "M�ximo de 100 caracteres")]
        [Display(Name = "Atua��o")]
        public string Role { get; set; }

        [MaxLength(800, ErrorMessage = "M�ximo de 800 caracteres")]
        [Display(Name = "Descri��o")]
        public string AboutDescription { get; set; }

        [MaxLength(300, ErrorMessage = "M�ximo de 300 caracteres")]
        [Display(Name = "Github")]
        public string UrlGithub { get; set; }


        //Properties navigations
        public virtual List<Skill> Skills { get; set; }
        public virtual List<Project> Projects { get; set; }
    }
}
