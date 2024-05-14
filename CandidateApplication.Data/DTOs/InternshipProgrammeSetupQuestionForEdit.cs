using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CandidateApplication.Data.DTOs
{
    public class InternshipProgrammeSetupQuestionForEdit
    {
        [Required]
        [JsonProperty("id")]
        public string Id { get; set; }
        [Required]
        public List<InternshipProgrammeSetupQuestion> Questions { get; set; }
    }
}
