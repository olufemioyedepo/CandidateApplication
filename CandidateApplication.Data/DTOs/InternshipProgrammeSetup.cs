using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Data.DTOs
{
    public class InternshipProgrammeSetup
    {
        [MaxLength(100, ErrorMessage = "Programme Title can not be empty!")]
        [Required]
        public string Title { get; set; }

        [MaxLength(1000, ErrorMessage = "Description can not be blank!")]
        [Required]
        public string Description { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }
        public string DataCategory { get; set; }
        
        public bool IsPhoneInternal { get; set; }
        public bool IsPhoneVisible { get; set; }

        public bool IsNationalityInternal { get; set; }
        public bool IsNationalityVisible { get; set; }

        public bool IsCurrentResidenceInternal { get; set; }
        public bool IsCurrentResidenceVisible { get; set; }

        public bool IsIDNumberInternal { get; set; }
        public bool IsIDNumberVisible { get; set; }

        public bool IsDateOfBirthInternal { get; set; }
        public bool IsDateOfBirthVisible { get; set; }

        public bool IsGenderInternal { get; set; }
        public bool IsGenderVisible { get; set; }

        [Required]
        [MaxLength(ErrorMessage = "At keast one question must be added!")]
        public List<InternshipProgrammeSetupQuestion> Questions { get; set; }
    }
}
