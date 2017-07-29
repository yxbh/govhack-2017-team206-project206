using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace Utilities
{
    public class TrafficDataJoiner
    {
        const string FinesAndOffencesCsvPath = "/Users/benhuang/Downloads/Traffic_camera_offences_and_fines.csv";
        const string CameraLocationCsvPath = "/Users/benhuang/Downloads/Traffic_speed_camera_locations.csv";
        const string OutputCsvPath = "/Users/benhuang/Downloads/traffic_join_output.csv";

        public TrafficDataJoiner()
        {
        }

		//Sum_Pen_Amt Sum_Inf_Count   Sum_With_Amt    Sum_With_Count
		public void Run()
        {
            // load
            var finesAndOffencesData = File.ReadAllLines(FinesAndOffencesCsvPath);
            var fineAndOffences = new List<FineAndOffenceDataItem>();
            for (int i = 1; i < finesAndOffencesData.Count(); ++i)
            {
                var data = finesAndOffencesData[i].Split(new char[] { ',' });

                string OffenceMonth = data[0].Trim();
                string RegoState = data[1].Trim();
                string CltCatg = data[2].Trim();
                string CameraType = data[3].Trim();
                string LocationCode = data[4].Trim();
                string LocationDesc = data[5].Trim();
                string OffenceDesc = data[6].Trim();
                string SumPenAmt = data[7].Trim(); //Convert.ToInt32(data[7]);
                string SumInfCount = data[8].Trim(); //Convert.ToInt32(data[8]);
                string SumWithAmt = string.IsNullOrWhiteSpace(data[9]) ? "0" : data[9].Trim() ; //Convert.ToInt32(data[9]);
                string SumWithCount = data[10].Trim(); //Convert.ToInt32(data[10]);

                var item = new FineAndOffenceDataItem()
                {
                    OffenceMonth    = OffenceMonth,
                    RegoState       = RegoState,
                    CltCatg         = CltCatg,
                    CameraType      = CameraType,
                    LocationCode    = LocationCode,
                    LocationDesc    = LocationDesc,
					OffenceDesc     = OffenceDesc,
					SumPenAmt       = SumPenAmt,
					SumInfCount     = SumInfCount,
					SumWithAmt      = SumWithAmt,
                    SumWithCount    = SumWithCount
                };
                fineAndOffences.Add(item);
            }

            var locationData = File.ReadAllLines(CameraLocationCsvPath);
            var locationMap = new Dictionary<string, SpeedCameraDataItem>();
            for (int i = 1; i < locationData.Count(); ++i)
            {
				// CAMERA TYPE  CAMERA LOCATION CODE    LOCATION_CODE   LATITUDE    LONGITUDE   LOCATION DESCRIPTION    Location    Decommissioned Camera_Date
				var data = locationData[i].Split(new char[] { ',' });
				var locationCode = Regex.Replace(data[1].Trim(), "[A-Za-z ]", "");
				var item = new SpeedCameraDataItem()
				{
					CameraType          = data[0].Trim(),
					CameraLocationCode  = data[1].Trim(),
					Latitude            = data[2].Trim(), //Convert.ToSingle(data[2]),
					Longitude           = data[3].Trim(), //Convert.ToSingle(data[3]),
					LocationDescription = data[4].Trim()
				};
                locationMap[locationCode] = item;
            }

            // merge
            foreach (var fineAndOffenceItem in fineAndOffences)
            {
                if (!locationMap.ContainsKey(fineAndOffenceItem.LocationCode))
                {
                    Console.WriteLine(string.Format("Could not find location code: '{0}'", fineAndOffenceItem.LocationCode));
                    continue;
                }
                var location = locationMap[fineAndOffenceItem.LocationCode];
                fineAndOffenceItem.CameraLocationCode   = location.CameraLocationCode;
				fineAndOffenceItem.Latitude             = location.Latitude;
				fineAndOffenceItem.Longitude            = location.Longitude;
				fineAndOffenceItem.LocationDescription  = location.LocationDescription;
            }

            // dump
            using (var fileStream = new FileStream(OutputCsvPath, FileMode.Create))
			using (var outputFile = new System.IO.StreamWriter(fileStream))
            {
				outputFile.WriteLine(
						string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
									  "OffenceMonth",
									  "RegoState",
									  "CltCatg",
									  "CameraType",
									  "LocationCode",
									  "LocationDesc",
									  "OffenceDesc",
									  "SumPenAmt",
									  "SumInfCount",
									  "SumWithAmt",
									  "SumWithCount",
									  "CameraLocationCode",
									  "Latitude",
									  "Longitude",
									  "LocationDescription"));
				foreach (var fineAndOffenceItem in fineAndOffences)
				{
                    outputFile.WriteLine(
                        string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14}",
                                      fineAndOffenceItem.OffenceMonth.ToString(),
                                      fineAndOffenceItem.RegoState,
                                      fineAndOffenceItem.CltCatg,
                                      fineAndOffenceItem.CameraType,
                                      fineAndOffenceItem.LocationCode,
                                      fineAndOffenceItem.LocationDesc,
                                      fineAndOffenceItem.OffenceDesc,
                                      fineAndOffenceItem.SumPenAmt,
                                      fineAndOffenceItem.SumInfCount,
                                      fineAndOffenceItem.SumWithAmt,
                                      fineAndOffenceItem.SumWithCount,
                                      fineAndOffenceItem.CameraLocationCode,
                                      fineAndOffenceItem.Latitude,
                                      fineAndOffenceItem.Longitude,
                                      fineAndOffenceItem.LocationDescription));
				}
            }
        }

    }

    public class FineAndOffenceDataItem
    {
        public string OffenceMonth;
        public string RegoState;
        public string CltCatg;
        public string CameraType;
        public string LocationCode;
        public string LocationDesc;
        public string OffenceDesc;
        public string SumPenAmt;
        public string SumInfCount;
        public string SumWithAmt;
        public string SumWithCount;

		public string CameraLocationCode;
		public string Latitude;
		public string Longitude;
		public string LocationDescription;
    }
	
    public class SpeedCameraDataItem
    {
        public string CameraType;
        public string CameraLocationCode;
        public string Latitude;
        public string Longitude;
        public string LocationDescription;
    }


}
