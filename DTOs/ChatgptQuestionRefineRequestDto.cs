using System;
using System.ComponentModel.DataAnnotations;

namespace AdeebBackend.DTOs;

public class ChatgptQuestionRefineRequestDto
{


    [Required]
    public string Question { get; set; }

}
