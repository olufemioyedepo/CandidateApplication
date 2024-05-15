using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Data.DTOs
{
    public class ApplicantFormDataQuestion
    {
        [Required]
        public string QuestionId { get; set; }
        [Required]
        public string Answer { get; set; }

    }
}
