﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HirePersonality.API.DataContract.Personality
{
    public class PersonalityCreateRequest
    {
        public int PersonalityNumber { get; set; }
        public Guid UserId { get; set; }
    }
}
