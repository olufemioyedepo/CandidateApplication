using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Data.DTOs
{
    public class ApplicantFormData
    {
        [Required]
        [MaxLength(30)]
        public string FirstName { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
        
        [Required]
        [RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Email address format is invalid!")]
        [MaxLength(80)]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Nationality { get; set; }
        public string CountryOfResidence { get; set; }
        public string IdNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string DataCategory { get; set; }
        [Required]
        public string ApplicationProgrammeId { get; set; }
        [JsonProperty("id")]
        public string Id { get; set; }
        [Required]
        public List<ApplicantFormDataQuestion> AnswersToQuestions { get; set; }
    }
}
