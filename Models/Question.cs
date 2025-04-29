using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace adeeb.Models
{
    public enum QuestionType
    {
        Text,
        MultipleChoice,
        Rating,
        YesNo
    }

    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string Text { get; set; }

        [Required]
        public QuestionType Type { get; set; }

        public string? Options { get; set; }

        public int SurveyId { get; set; }
        [ForeignKey("SurveyId")]
        public Survey? Survey { get; set; }

        public List<SurveyResponse> Responses { get; set; } = new();
    }
}