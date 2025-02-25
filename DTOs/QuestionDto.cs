using System;
using adeeb.Models;

namespace AdeebBackend.DTOs;

public class QuestionDto
{

    public int Id { get; set; }
    public string Text { get; set; }
    public QuestionType QuestionType { get; set; }

}
