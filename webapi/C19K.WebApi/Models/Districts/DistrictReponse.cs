using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace C19K.WebApi.Models.Districts
{
    public class DistrictReponse
    {
        public DistrictReponse(List<string> districtNames)
        {
            Districts =  districtNames.Select(x=> new District { Name = x }).ToList();
        }
        public List<District> Districts { get; set; }

        public class District
        {
            public string Name { get; set; }
        }
      
    }
}