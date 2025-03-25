using Newtonsoft.Json;

namespace ConsoleApp1.Dto;


    public class ApiResponce
    {
        public int AllowCountPreReg { get; set; }
        public int AllowCountTerminal { get; set; }
        public string Building { get; set; }
        public int CityId { get; set; }
        public string CityName { get; set; }
        public int CommunityId { get; set; }
        public string CommunityName { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int DistrictId { get; set; }
        public string DistrictName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Office { get; set; }
        public int RegionId { get; set; }
        public string RegionName { get; set; }
        public int SrvCenterId { get; set; }
        public string SrvCenterName { get; set; }
        public string Street { get; set; }
    }
