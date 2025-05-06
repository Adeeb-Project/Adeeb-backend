using System;
using adeeb.Models;

namespace AdeebBackend.Models;

public class QuestionMcqOption
{
    public int Id { get; set; }
    public int QuestionId { get; set; }
    public string OptionText { get; set; }


    public Question Question { get; set; }

}
