﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HirePersonality.Business.DataContract.Personality
{
    public class ReceivePersonalityDTO
    {
        public int PersonalityNumber { get; set; }
        public Guid UserId { get; set; }
    }
}