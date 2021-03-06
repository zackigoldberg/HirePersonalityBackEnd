﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using HirePersonality.API.DataContract.Personality;
using HirePersonality.Business.DataContract.Personality;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HirePersonality.API.Controllers.Personality
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]

    public class PersonalityController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IPersonalityManager _manager;

        public PersonalityController(IMapper mapper, IPersonalityManager manager) 
        {
                _mapper = mapper;
                _manager = manager;
        }
       
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> PostPersonality(CreatePersonalityRequest request)
        {
            var dto = _mapper.Map<CreatePersonalityDTO>(request);
            dto.UserId = GetUserId();
            if (await _manager.CreatePersonality(dto))
                return StatusCode(201);
            
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("Display")]
        [ActionName("Display")]
        public async Task<IActionResult> GetPersonality()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            } 

            var dto = await _manager.GetPersonality();

            var request = 
                _mapper.Map<IEnumerable<ReceivePersonalityRequest>>(dto);

            return Ok(request);
        }

        [HttpGet]
        [Route("id")]
        [ActionName("id")]
        public async Task<IActionResult> GetPersonalityById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var dto = await _manager.GetPersonality(id);

            var request =
                _mapper.Map<ReceivePersonalityRequest>(dto);

            return Ok(request);
        }
 
        [HttpPut]
        public async Task<IActionResult> UpdatePersonality(UpdatePersonalityRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var dto = _mapper.Map<UpdatePersonalityDTO>(request);

            if (await _manager.UpdatePersonality(dto))
                return StatusCode(202);
            
            else
                return StatusCode(303);
        }
        [Authorize]
        [HttpDelete]
        [Route("Delete")]
        public IActionResult DeletePersonality()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_manager.DeletePersonality(userId))
                return StatusCode(217);
            else
                return StatusCode(303);
        }

        private int GetUserId()
        {
            var userId = int.Parse(User.Identity.GetUserId());
            return userId;
        } 

        [HttpGet]
        [Route("Type")]
        public async Task<int> GetPersonalityType()
        {
            var ownerId = GetUserId();

            return await _manager.GetPersonalityType(ownerId);
        }
    }
}