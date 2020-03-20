using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using C19K.WebApi.Dto;
using C19K.WebApi.Models.Districts;

namespace C19K.WebApi.Controllers
{
    public class GeneralController : Controllerbase
    {
        public DistrictReponse GetDistrictNames()
        {
            return new DistrictReponse(Enum.GetNames(typeof(Districts)).ToList());
        }
    }

    
}