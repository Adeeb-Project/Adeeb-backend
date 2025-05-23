using System.Text.Json.Serialization;
using AdeebBackend.Models;

namespace adeeb.Models
{


    public class Question
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string Text { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public QuestionType QuestionType { get; set; }


        public Survey Survey { get; set; }
        public ICollection<QuestionMcqOption> Options { get; set; }

    }

    public enum QuestionType
    {
        TextQuestion,
        RatingQuestion,
        MultipleChoiceQuestion,
    }


}