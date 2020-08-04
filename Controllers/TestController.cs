using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using testAspOracle01;
using testAspOracle01.Models;
using testAspOracle01.persistence.Helpers;

namespace testAspOracle01.Controllers {
    [Route ("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase {
        private readonly IMapper _mapper;
        public TestController (IMapper mapper) => _mapper = mapper;

        // GET api/test
        [HttpGet ("")]
        public ActionResult<IEnumerable<string>> Getstrings () {
            return new List<string> { };
        }

        // POST api/test
        [HttpPost ("")]
        public async Task<ActionResult<testModel>> Poststring (UserInputModel model) {
            List<Param> param = new List<Param> () {
                new Param () { ParamName = "AS_USER_ID", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.user_id },
                new Param () { ParamName = "AS_MENU_GRP", ParamType = ParamMeterTypeEnum.STRING, ParamValue = model.group_id },
            };
            var results = await new DataContext (_mapper).CallStoredProcudure (DataBaseHostEnum.KPR, "KPDBA.SP_GET_APP_USER_TOOL", param);
            if (results == null)
                return BadRequest (new { message = "Something was wrong!!" });

            var result = _mapper.Map<IEnumerable<AppUserToolModel>> (results);
            var resultReal = _mapper.Map<IEnumerable<AppUserToolModel>, IEnumerable<testModel>> (result).ToList ();
            return Ok (resultReal);

        }
    }

}