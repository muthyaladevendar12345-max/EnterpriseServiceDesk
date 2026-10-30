using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EnterpriseServiceDesk.Core.Dtos
{
    public sealed class AgentQuestionRequest
    {
        [Required, StringLength(2000, MinimumLength = 2)]
        public string Message { get; set; } = string.Empty;
    }
}
